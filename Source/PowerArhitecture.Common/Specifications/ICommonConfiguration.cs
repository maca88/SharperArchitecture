using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Specifications
{
    public interface ICommonConfiguration
    {
        string DefaultCulture { get; }
        string TranslationsByCulturePattern { get; }
        string DefaultTranslationsPath { get; }
        Configuration.NinjectConfiguration Ninject { get; }
    }
}
