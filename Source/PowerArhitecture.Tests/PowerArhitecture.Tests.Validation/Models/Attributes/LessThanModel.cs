﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class LessThanModel
    {
        [LessThan(10)]
        public int Value { get; set; }

        [LessThan(10, IncludePropertyName = true)]
        public int Value2 { get; set; }

    }
}
