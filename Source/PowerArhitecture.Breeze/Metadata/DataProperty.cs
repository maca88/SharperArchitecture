using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PowerArhitecture.Breeze.Extensions;
using Newtonsoft.Json;

namespace PowerArhitecture.Breeze.Metadata
{
    /// <summary>
    /// A single data property, at a minimum you must to define either a 'name' or a 'nameOnServer' and either a 'dataType' or a 'complexTypeName'.
    /// </summary>
    //[JsonConverter(typeof(MetadataJsonConverter))]
    public class DataProperty : BaseProperty
    {
        public DataProperty()
        {
        }

        public DataProperty(PropertyInfo propertyInfo)
        {
            DataType = BreezeTypeHelper.GetDataType(propertyInfo.PropertyType);
            NameOnServer = propertyInfo.Name;
            IsNullable = !propertyInfo.PropertyType.IsPrimitive;
            PropertyInfo = propertyInfo;
            var typeVal = BreezeTypeHelper.GetTypeValidator(propertyInfo.PropertyType);
            if(typeVal != null)
                Validators.Add(typeVal);
        }

        public DataProperty(Dictionary<string, object> dict) : base(dict)
        {
        }

        public PropertyInfo PropertyInfo { get; internal set; }

        #region DataType

        /// <summary>
        /// If present, the complexType name should be omitted.
        /// </summary>
        public DataType DataType
        {
            get { return OriginalDictionary.GetValue<DataType>("dataType"); }
            set { OriginalDictionary["dataType"] = value.ToString(); }
        }

        #endregion

        #region ComplexTypeName

        /// <summary>
        /// If present, this must be the fully qualified name of one of the 'complexTypes' defined within this document, and the 'dataType' property may be omitted
        /// </summary>
        public string ComplexTypeName
        {
            get { return OriginalDictionary.GetValue<string>("complexTypeName"); }
            set { OriginalDictionary["complexTypeName"] = value; }
        }

        #endregion

        #region IsNullable

        /// <summary>
        /// Whether a null can be assigned to this property.
        /// </summary>
        public bool IsNullable
        {
            get { return OriginalDictionary.GetValue<bool>("isNullable"); }
            set { OriginalDictionary["isNullable"] = value; }
        }

        #endregion

        #region DefaultValue

        /// <summary>
        /// The default value for this property if nothing is assigned to it.
        /// </summary>
        public object DefaultValue
        {
            get { return OriginalDictionary.GetValue<object>("defaultValue"); }
            set { OriginalDictionary["defaultValue"] = value; }
        }

        #endregion

        #region IsPartOfKey

        /// <summary>
        /// Whether this property is part of the key for this entity type
        /// </summary>
        public bool IsPartOfKey
        {
            get { return OriginalDictionary.GetValue<bool>("isPartOfKey"); }
            set { OriginalDictionary["isPartOfKey"] = value; }
        }

        #endregion

        #region ConcurrencyMode

        /// <summary>
        /// This determines whether this property is used for concurreny purposes.
        /// </summary>
        public ConcurrencyMode ConcurrencyMode
        {
            get { return OriginalDictionary.GetValue<ConcurrencyMode>("concurrencyMode"); }
            set { OriginalDictionary["concurrencyMode"] = value.ToString(); }
        }

        #endregion

        #region MaxLength

        /// <summary>
        /// Only applicable to 'String' properties. This is the maximum string length allowed.
        /// </summary>
        public int MaxLength
        {
            get { return OriginalDictionary.GetValue<int>("maxLength"); }
            set { OriginalDictionary["maxLength"] = value; }
        }

        #endregion

        #region Validators

        private Validators _validators;

        /// <summary>
        /// A list of the validators (validations) that will be associated with this property
        /// </summary>
        public Validators Validators
        {
            get
            {
                if (_validators != null)
                    return _validators;
                if (!OriginalDictionary.ContainsKey("validators"))
                    OriginalDictionary["validators"] = new List<Dictionary<string, object>>();
                _validators = new Validators(OriginalDictionary["validators"] as List<Dictionary<string, object>>);
                return _validators;
            }
            set
            {
                _validators = value;
                OriginalDictionary["validators"] = value.OriginalList;
            }
        }

        #endregion
    }
}
