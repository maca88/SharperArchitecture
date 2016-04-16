using System;
using System.Collections.Generic;

namespace PowerArhitecture.Validation
{
    public class ValidationRuleSet
    {
        public const string Default = "default";
        public const string Delete = "Delete";
        public const string Insert = "Insert";
        public const string Update = "Update";
        public const string Attribute = "Attribute";

        public static string[] AttributeInsertUpdate { get { return Combine(Attribute, Insert, Update); } }

        public static string[] AttributeInsert { get { return Combine(Attribute, Insert); } }

        public static string[] AttributeUpdate { get { return Combine(Update, Attribute); } }

        public static string[] AttributeInsertUpdateDefault { get { return Combine(Default, Attribute, Insert, Update); } }

        public static string[] InsertUpdate { get { return Combine(Update, Insert); } }

        public static string[] Combine(params string[] rules)
        {
            return rules;
        }
    }
}
