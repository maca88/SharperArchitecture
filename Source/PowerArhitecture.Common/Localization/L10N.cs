﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Internationalization;
using SecondLanguage;

namespace PowerArhitecture.Common.Localization
{
    public class L10N
    {
        static L10N()
        {
            var defaultCulture = AppConfiguration.GetSetting<string>(CommonConfigurationKeys.DefaultCulture);
            DefaultCulture = CultureInfo.GetCultureInfo(defaultCulture);
        }

        public static CultureInfo DefaultCulture { get; private set; }

        public static IList<CultureInfo> GetRegisteredCultures()
        {
            return I18N.Translators.Select(o => CultureInfo.GetCultureInfo(o.Key)).ToList();
        }
    }
}
