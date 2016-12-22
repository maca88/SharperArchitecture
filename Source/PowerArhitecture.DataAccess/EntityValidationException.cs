using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace PowerArhitecture.DataAccess
{
    public class EntityValidationException : ValidationException
    {
        private static readonly FieldInfo MessageField;

        static EntityValidationException()
        {
            MessageField = typeof (Exception).GetField("_message",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (MessageField == null)
                throw new NullReferenceException("System.Exception does not contains internal field _message");
        }

        public EntityValidationException(
            IEnumerable<ValidationFailure> errors,
            object entity,
            Type entityType = null,
            IEnumerable<string> ruleSets = null) : base(errors)
        {
            Entity = entity;
            EntityType = entityType ?? (entity != null ? entity.GetType() : null);
            RuleSets = ruleSets ?? new List<string>();
            BuildMessage();
        }

        public IEnumerable<string> RuleSets { get; private set; }

        public Type EntityType { get; private set; }

        public object Entity { get; private set; }

        public object GetEntity(ValidationFailure error)
        {
            if (Entity == null) return null;
            if (string.IsNullOrEmpty(error.PropertyName)) return Entity;

            var paths = error.PropertyName.Split('.');
            return paths.Length == 1 
                ? Entity 
                : Entity.GetMemberValue(string.Join(".", paths.Take(paths.Length - 1)));
        }

        private void BuildMessage()
        {
            if (EntityType == null) return;

            var msg = string.Format("Validation failed for type '{0}':", EntityType.FullName);

            msg += string.Join("", Errors.Select(x =>
            {
                var atemptVal = x.AttemptedValue;
                if (atemptVal == null)
                    atemptVal = "null";
                var m = "\r\n -- AttemptedValue '{0}'";
                if (!string.IsNullOrEmpty(x.PropertyName))
                    m += ", PropertyName '{1}'";
                m += ": {2}";

                return string.Format(m, atemptVal, x.PropertyName, x.ErrorMessage);
            }));

            MessageField.SetValue(this, msg);
        }
    }
}
