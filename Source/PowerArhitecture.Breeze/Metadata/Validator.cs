using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerArhitecture.Breeze.Extensions;
using Newtonsoft.Json;

namespace PowerArhitecture.Breeze.Metadata
{
    //[JsonConverter(typeof(MetadataJsonConverter))]
    public class Validator : MetadataDictionary
    {
        public Validator()
        {
        }

        public Validator(Dictionary<string, object> dict) : base(dict)
        {
        }

        #region Name

        /// <summary>
        /// On deserialization, this must match the name of some validator already registered on the breeze client.
        /// </summary>
        public string Name
        {
            get { return OriginalDictionary.GetValue<string>("name"); }
            set { OriginalDictionary["name"] = value; }
        }

        #endregion
    }
}
