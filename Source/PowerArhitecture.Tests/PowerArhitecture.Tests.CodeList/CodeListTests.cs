using System;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.CodeList;
using PowerArhitecture.CodeList.Specifications;
using Ninject;
using NUnit.Framework.Internal;
using PowerArhitecture.Authentication;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.DataAccess.Factories;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.CodeList.Entities;
using NUnit.Framework;

namespace PowerArhitecture.Tests.CodeList
{
    [TestFixture]
    public class CodeListTests : DatabaseBaseTest
    {
        public CodeListTests()
        {
            DatabaseConfiguration.AddEntityAssembly(Assembly.GetAssembly(typeof (ICodeList)));
            DatabaseConfiguration.AddEntityAssembly(Assembly.GetAssembly(typeof(IUser)));
            DatabaseConfiguration.AddConventionAssembly(Assembly.GetAssembly(typeof (ICodeList)));
            DatabaseConfiguration.AddConventionAssembly(Assembly.GetAssembly(typeof(IUser)));
            AddMappingStepAssembly(Assembly.GetAssembly(typeof(ICodeList)));
        }

        private void FillData()
        {
            //Languages
            var slvLanguage = "sl-SI";
            var enLanguage =  "en-US";
            var itLanguage = "it-IT";

            var car1 = new CLCarCodeList {Active = true, Code = "BMW"};
            car1.AddName(new CLCarCodeListLocalization
                {
                    LanguageCode = slvLanguage,
                    Name = "BMW Slo"
                });
            car1.AddName(new CLCarCodeListLocalization
                {
                    LanguageCode = enLanguage,
                    Name = "BMW Eng"
                });

            var car2 = new CLCarCodeList { Active = true, Code = "AUDI" };
            car2.AddName(new CLCarCodeListLocalization
            {
                LanguageCode = itLanguage,
                Name = "Audi Ita"
            });

            var car2LocSl = new CLCarCodeListLocalization
                {
                    LanguageCode = slvLanguage,
                    Name = "Audi Slo"
                };
            car2LocSl.SetCodeList(car2);

            //using (var unitOfWork = UnitOfWorkFactory.GetNew())
            //{
            //    unitOfWork.Save(car1, car2);
            //}
        }
    }
}
