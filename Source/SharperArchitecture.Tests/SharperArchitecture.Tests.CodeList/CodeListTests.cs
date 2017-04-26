using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using SharperArchitecture.Authentication.Entities;
using SharperArchitecture.CodeList;
using SharperArchitecture.CodeList.Specifications;
using NUnit.Framework.Internal;
using SharperArchitecture.Authentication;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.DataAccess.Factories;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.CodeList.Entities;
using NUnit.Framework;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Validation;
using SharperArchitecture.Validation.Specifications;
using SharperArchitecture.Tests.Common.Extensions;

namespace SharperArchitecture.Tests.CodeList
{
    [TestFixture]
    public class CodeListTests : DatabaseBaseTest
    {
        private const string SlLanguage = "sl";
        private const string EnLanguage = "en";
        private const string ItLanguage = "it";

        public CodeListTests()
        {
            EntityAssemblies.Add(Assembly.GetAssembly(typeof(ICodeList)));
            EntityAssemblies.Add(Assembly.GetAssembly(typeof(CodeListTests)));
            ConventionAssemblies.Add(Assembly.GetAssembly(typeof(ICodeList)));
            TestAssemblies.Add(typeof(CodeListTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(ICodeList).Assembly);
        }

        protected override void ConfigureDatabaseConfiguration(DatabaseConfiguration configuration)
        {
            configuration.AutoMappingConfiguration.AddStepAssembly(
                Assembly.GetAssembly(typeof(ICodeList)));
        }

        [Test]
        public void QueryingByCodeShouldWork()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                try
                {
                    var cl = unitOfWork.Query<SimpleCodeList>()
                        .First(o => o.Code == "Code1");
                    Assert.NotNull(cl);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void QueryingByIdShouldWork()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                try
                {
                    var cl = unitOfWork.Query<SimpleCodeList>()
                        .First(o => o.Id == "Code1");
                    Assert.NotNull(cl);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void QueryingLocalizableCodeListByIdShouldWork()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                try
                {
                    unitOfWork.DefaultSession.EnableFilter("Language")
                        .SetParameter("Current", "sl")
                        .SetParameter("Fallback", "en");
                    var bmw = unitOfWork.Query<Car>()
                        .First(o => o.Id == "BMW");
                    Assert.NotNull(bmw);

                    bmw = unitOfWork.Query<Car>()
                        .First(o => o.Id == "BMW");
                    Assert.NotNull(bmw);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void IdAndCodeMustBeEqual()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                try
                {
                    var cls = unitOfWork.Query<SimpleCodeList>()
                        .ToList();
                    foreach (var cl in cls)
                    {
                        Assert.AreEqual(cl.Id, cl.Code);
                    }

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void LocalizationWithLanguageFilter()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                try
                {
                    unitOfWork.DefaultSession.EnableFilter("Language")
                        .SetParameter("Current", "sl")
                        .SetParameter("Fallback", "en");
                    var bmw = unitOfWork.Query<Car>()
                        .First(o => o.Code == "BMW");
                    Assert.AreEqual("BMW Slo", bmw.Name);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void LocalizationWithCustomLanguageFilter()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                try
                {
                    unitOfWork.DefaultSession.EnableFilter("Language")
                        .SetParameter("Current", "sl")
                        .SetParameter("Fallback", "en");
                    var cl = unitOfWork.Query<CustomLanguageFilter>()
                        .First(o => o.Code == "Code1");
                    Assert.AreEqual("SL", cl.Name);
                    Assert.AreEqual("CustomSL", cl.Custom);
                    Assert.AreEqual("Custom2SL", cl.CurrentCustom2);

                    // Fallback
                    cl = unitOfWork.Query<CustomLanguageFilter>()
                        .First(o => o.Code == "Code2");
                    Assert.AreEqual("EN", cl.Name);
                    Assert.AreEqual("Custom2EN", cl.CurrentCustom2);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        protected override IFluentDatabaseConfiguration CreateDatabaseConfiguration(string dbName = "foo", string name = null)
        {
            return base.CreateDatabaseConfiguration(dbName, name)
                .Conventions(c => c
                    .HiLoId(o => o.Enabled(false)
                    )
                );
        }

        protected override void FillData(ISessionFactory sessionFactory)
        {
            var entities = new List<IEntity>();

            FillCars(entities);
            FillCustomLanguageFilters(entities);
            FillSimpleCodeList(entities);

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                try
                {
                    unitOfWork.Save(entities.ToArray());

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        #region Car

        private void FillSimpleCodeList(List<IEntity> entities)
        {
            var cl = new SimpleCodeList
            {
                Name = "Code1",
                Code = "Code1"
            };
            entities.Add(cl);

            var cl2 = new SimpleCodeList
            {
                Name = "Code2",
                Code = "Code2"
            };
            entities.Add(cl2);
        }

        #endregion

        #region Car

        private void FillCars(List<IEntity> entities)
        {
            var car1 = new Car { Code = "BMW" };
            car1.AddName(new CarLanguage
            {
                LanguageCode = SlLanguage,
                Name = "BMW Slo"
            });
            car1.AddName(new CarLanguage
            {
                LanguageCode = EnLanguage,
                Name = "BMW Eng"
            });
            entities.Add(car1);

            var car2 = new Car { Code = "AUDI" };
            car2.AddName(new CarLanguage
            {
                LanguageCode = ItLanguage,
                Name = "Audi Ita"
            });
            entities.Add(car2);

            var car2LocSl = new CarLanguage
            {
                LanguageCode = SlLanguage,
                Name = "Audi Slo"
            };
            car2LocSl.SetCodeList(car2);
        }

        #endregion

        #region CustomLanguageFilter

        private void FillCustomLanguageFilters(List<IEntity> entities)
        {
            var cl1 = new CustomLanguageFilter { Code = "Code1" };
            cl1.AddName(new CustomLanguageFilterLanguage
            {
                LanguageCode = SlLanguage,
                Name = "SL",
                Custom = "CustomSL",
                Custom2 = "Custom2SL"
            });
            cl1.AddName(new CustomLanguageFilterLanguage
            {
                LanguageCode = ItLanguage,
                Name = "IT",
                Custom = "CustomIT",
                Custom2 = "Custom2IT"
            });
            entities.Add(cl1);

            var cl2 = new CustomLanguageFilter { Code = "Code2" };
            cl2.AddName(new CustomLanguageFilterLanguage
            {
                LanguageCode = ItLanguage,
                Name = "IT",
                Custom = "CustomIT",
                Custom2 = "Custom2IT"
            });
            cl2.AddName(new CustomLanguageFilterLanguage
            {
                LanguageCode = EnLanguage,
                Name = "EN",
                Custom = "CustomEN",
                Custom2 = "Custom2EN"
            });
            entities.Add(cl2);
        }

        #endregion

    }
}
