using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Internationalization
{
    public class LocalizableAttribute : Attribute
    {
        /// <summary>
        /// Use this when you do not want that a property is not localizable inside a class
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Use this when you want some other name rather than property name or class name
        /// </summary>
        public string MessageId { get; set; }


    }
}
