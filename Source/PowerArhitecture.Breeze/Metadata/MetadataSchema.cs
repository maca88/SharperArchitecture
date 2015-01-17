using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Breeze.Extensions;
using Newtonsoft.Json;

namespace PowerArhitecture.Breeze.Metadata
{
    //[JsonConverter(typeof(MetadataJsonConverter))]
    public class MetadataSchema : MetadataDictionary
    {
        public MetadataSchema(IDictionary<string, object> metadata)
            : base(metadata as Dictionary<string, object>)
        {
        }

        #region MetadataVersion

        public string MetadataVersion
        {
            get { return OriginalDictionary.GetValue<string>("metadataVersion"); }
            set { OriginalDictionary["metadataVersion"] = value; }
        }

        #endregion

        #region NamingConvention

        public string NamingConvention
        {
            get { return OriginalDictionary.GetValue<string>("namingConvention"); }
            set { OriginalDictionary["namingConvention"] = value; }
        }

        #endregion

        #region LocalQueryComparisonOptions

        public string LocalQueryComparisonOptions
        {
            get { return OriginalDictionary.GetValue<string>("localQueryComparisonOptions"); }
            set { OriginalDictionary["localQueryComparisonOptions"] = value; }
        }

        #endregion

        #region StructuralTypes

        private StructuralTypes _structuralTypes;

        public StructuralTypes StructuralTypes
        {
            get
            {
                if (_structuralTypes != null)
                    return _structuralTypes;
                if (!OriginalDictionary.ContainsKey("structuralTypes"))
                    OriginalDictionary["structuralTypes"] = new List<Dictionary<string, object>>();
                _structuralTypes = new StructuralTypes(OriginalDictionary["structuralTypes"] as List<Dictionary<string, object>>);
                return _structuralTypes;
            }
            set
            {
                _structuralTypes = value;
                OriginalDictionary["structuralTypes"] = value.OriginalList;
            }
        }

        #endregion

        #region FkMap

        private FkMap _fkMap;

        public FkMap FkMap
        {
            get
            {
                if (_fkMap != null)
                    return _fkMap;
                if (!OriginalDictionary.ContainsKey("fkMap"))
                    OriginalDictionary["fkMap"] = new Dictionary<string, string>();
                _fkMap = new FkMap(OriginalDictionary["fkMap"] as Dictionary<string, string>);
                return _fkMap;
            }
            set
            {
                _fkMap = value;
                OriginalDictionary["fkMap"] = value.OriginalDictionary;
            }
        }

        #endregion
    }
}
