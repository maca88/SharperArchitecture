﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Validation.Attributes
{
    public abstract class ComparisonAttribute : ValidationAttribute
    {
        public object CompareToValue { get; set; }

        public string ComparsionProperty { get; set; }
    }
}
