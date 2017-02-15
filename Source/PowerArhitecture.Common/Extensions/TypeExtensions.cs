using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;

namespace System
{
    public static class TypeExtensions
    {
        private const BindingFlags AllBinding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

        public static bool IsSimpleType(this Type type)
        {
            return
                type.IsPrimitive ||
                type.IsValueType ||
                type.IsEnum ||
                type == typeof (String);
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Returns the value of the PriorityAttribute if exists, otherwise a default value will be returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static short GetPriority(this Type type)
        {
            return type.GetCustomAttribute<PriorityAttribute>()?.Priority ?? default(short);
        }

        /// <summary>
        /// Check if the given type is assignable to the provided generic type.
        /// Implementation taken from http://stackoverflow.com/a/1075059
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
            if (givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }
            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public static Type GetGenericType(this Type givenType, Type genericType)
        {
            while (true)
            {
                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                {
                    return givenType;
                }
                    
                var type = givenType.GetInterfaces().FirstOrDefault(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType);
                if (type != null)
                {
                    return type;
                }

                var baseType = givenType.BaseType;
                if (baseType == null)
                {
                    return null;
                }
                givenType = baseType;
            }
        }

        public static bool IsEnumerableType(this Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof (IEnumerable));
        }

        /// <summary>
        /// http://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        public static bool IsNumericType(this Type type)
        {
            if (type == null || type.IsEnum)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }

        /// <summary>
        /// More info: http://stackoverflow.com/questions/358835/getproperties-to-return-all-properties-for-an-interface-inheritance-hierarchy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }

        public static PropertyInfo GetPublicProperty(this Type type, string propertyName)
        {
            return type.GetPublicProperties().FirstOrDefault(o => o.Name == propertyName);
        }

        public static MemberInfo GetNestedMember(this Type type, string path)
        {
            var currentType = type;
            MemberInfo currentMember = null;
  
            foreach (var memberName in path.Split('.'))
            {
                var property = GetProperty(currentType, memberName);
                if (property != null)
                {
                    currentMember = property;
                    currentType = property.PropertyType;
                    continue;
                }
                var field = GetField(currentType, memberName);
                if (field != null)
                {
                    currentMember = field;
                    currentType = field.FieldType;
                    continue;
                }
                var method = GetMethod(currentType, memberName);
                if (method != null) //If we found a method just return it
                {
                    return method;
                }
                return null;
            }
            return currentMember;
        }

        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, AllBinding);
        }

        private static FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetField(fieldName, AllBinding);
        }

        private static MethodInfo GetMethod(Type type, string methodName)
        {
            return type.GetMethod(methodName, AllBinding);
        }

        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            type = type.GetTypeInfo().BaseType;
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }
    }
}
