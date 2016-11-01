using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject;
using NUnit.Framework;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestFixture]
    public class EntityValidationTests : DatabaseBaseTest
    {
        public EntityValidationTests()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
        }

        [Test]
        public void VariuosChildSubChildDeletionShouldTriggerRootToValidateAutomatically()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                var entity = new ValidableEntity {Name = "Test"};
                ValidableEntityChild child;
                for (var i = 0; i < 10; i++)
                {
                    child = new ValidableEntityChild {CountryCode = $"Code{i}", ValidableEntity = entity};
                    entity.Children.Add(child);
                    child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                }
                unitOfWork.Save(entity);
                Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);

                entity.Name = "New name";
                unitOfWork.Save(entity);
                unitOfWork.Flush();
                Assert.AreEqual(2, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the child from the collection and set it as deleted
                // OnPreUpdateCollection will trigger the validation
                child = entity.Children.First();
                entity.Children.Remove(child);
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                Assert.AreEqual(3, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the releationship with the child on both sides
                child = entity.Children.First();
                entity.Children.Remove(child);
                child.ValidableEntity = null;
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                Assert.AreEqual(4, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the subchild from the child on both sides
                child = entity.Children.First();
                var subChild = child.Children.First();
                child.Children.Remove(subChild);
                subChild.ValidableEntityChild = null;
                unitOfWork.Flush();
                Assert.AreEqual(5, ValidableEntityValidationContextFiller.FilledCount);
            }
        }

        [Test]
        public void InvalidRootShoudThrowOnAutomaticValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    entity.Name = null;
                }
            });
        }

        [Test]
        public void InvalidChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First();
                    child.CountryCode = "Unknown";
                }
            });
        }

        [Test]
        public void InvalidDeletedChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First();
                    entity.Children.Remove(child);
                    child.ValidableEntity = null;
                    child.CountryCode = "CannotDelete";
                }
            });
        }

        [Test]
        public void InvalidManuallyDeletedChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First();
                    entity.Children.Remove(child);
                    unitOfWork.Delete(child);
                    child.CountryCode = "CannotDelete";
                }
            });
        }

        [Test]
        public void InvalidInsertedChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    entity.Children.Add(new ValidableEntityChild
                    {
                        CountryCode = "CannotInsert",
                        ValidableEntity = entity
                    });
                }
            });
        }

        [Test]
        public void InvalidSubChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First().Children.First();
                    child.Name = null;
                }
            });
        }

        [Test]
        public void InvalidDeletedSubChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First().Children.First();
                    entity.Children.First().Children.Remove(child);
                    child.ValidableEntityChild = null;
                    child.Name = "CannotDelete";
                }
            });
        }

        [Test]
        public void InvalidManuallyDeletedSubChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated");
                    var child = entity.Children.First().Children.First();
                    entity.Children.First().Children.Remove(child);
                    unitOfWork.Delete(child);
                    child.Name = "CannotDelete";
                }
            });
        }

        [Test]
        public void InvalidInsertedSubChildShoudThrowOnAutomaticRootValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated").Children.First();
                    entity.Children.Add(new ValidableEntitySubChild
                    {
                        Name = "CannotInsert",
                        ValidableEntityChild = entity
                    });
                }
            });
        }

        protected override void FillData(ISessionFactory sessionFactory)
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>())
            {
                for (var i = 0; i < 100; i++)
                {
                    unitOfWork.Save(new Country {Code = $"Code{i}", Name = $"Code{i}" });
                }
                unitOfWork.Flush();

                var entity = new ValidableEntity { Name = "Generated" };
                for (var i = 0; i < 10; i++)
                {
                    var child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                    entity.Children.Add(child);
                    child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                }
                unitOfWork.Save(entity);
            }
        }
    }
}
