using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.Common.Settings;
using SecondLanguage;

namespace PowerArhitecture.Common.Localization
{
    public class L10N
    {
        static L10N()
        {
            var defaultCulture = AppConfiguration.GetSetting<string>(CommonSettingKeys.DefaultCulture);
            DefaultCulture = CultureInfo.GetCultureInfo(defaultCulture);
        }

        public static CultureInfo DefaultCulture { get; private set; }

        public static IList<CultureInfo> GetRegisteredCultures()
        {
            return I18N.Translators.Select(o => CultureInfo.GetCultureInfo(o.Key)).ToList();
        }
        /*
        public static Dictionary<string, Dictionary<string, string>> GetAllTranstations(string culture = null)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            result.Add(DefaultCulture.Name, new Dictionary<string, string>());

            foreach (var translation in Translator.Default.TranslationList.OfType<GettextTranslation>())
            {
                foreach (var getTextKey in translation.GetGettextKeys())
                {
                    getTextKey.
                }
                translation.GetGettextKeys()
            }

            I18N.Translators
        }*/


    }
}
