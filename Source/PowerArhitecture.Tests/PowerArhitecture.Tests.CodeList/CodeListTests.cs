using System;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.CodeList;
using PowerArhitecture.CodeList.Specifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using PowerArhitecture.Authentication;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.CodeList.Entities;

namespace PowerArhitecture.Tests.CodeList
{
    [TestClass]
    public class CodeListTests : BaseTest
    {
        private ICodeListCache _codeListCache;

        public CodeListTests()
        {
            EntityAssemblies.Add(Assembly.GetAssembly(typeof(CodeListCache)));
            EntityAssemblies.Add(Assembly.GetAssembly(typeof(User)));
            ConventionAssemblies.Add(Assembly.GetAssembly(typeof(CodeListCache)));
            ConventionAssemblies.Add(Assembly.GetAssembly(typeof(User)));
            AddMappingStepAssembly(Assembly.GetAssembly(typeof(CodeListCache)));
        }

        [TestMethod]
        public void TestMethod1()
        {
            FillData();
            _codeListCache.UpdateOrInsertCodeList<CLCarCodeList>();
            var list = _codeListCache.GetCodeList<CLCarCodeList>();
            Assert.AreEqual(list.Count(), 2);
        }

        protected override void AfterInitialization()
        {
            _codeListCache = Kernel.Get<ICodeListCache>();
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

            using (var unitOfWork = UnitOfWorkFactory.GetNew())
            {
                unitOfWork.Save(car1, car2);
            }
        }
    }
}
