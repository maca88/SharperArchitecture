using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerArhitecture.Breeze.Extensions;

namespace PowerArhitecture.Breeze.Metadata
{
    public abstract class BaseProperty : MetadataDictionary
    {
        protected BaseProperty()
        {
        }

        protected BaseProperty(Dictionary<string, object> dict) : base(dict)
        {
        }


        #region Name

        /// <summary>
        /// The client side name of this property.
        /// </summary>
        public string Name
        {
            get { return OriginalDictionary.GetValue<string>("name"); }
            set { OriginalDictionary["name"] = value; }
        }

        #endregion

        #region NameOnServer

        /// <summary>
        /// The server side side name of this property. Either name or nameOnServer must be specified and either is sufficient.
        /// </summary>
        public string NameOnServer
        {
            get { return OriginalDictionary.GetValue<string>("nameOnServer"); }
            set { OriginalDictionary["nameOnServer"] = value; }
        }

        #endregion
    }
}
