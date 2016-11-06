using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject;
using NUnit.Framework;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.NHEventListeners;
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
                unitOfWork.Commit();
            }
            Assert.AreEqual(5, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public async Task VariuosChildSubChildDeletionShouldTriggerRootToValidateAutomaticallyAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                var entity = new ValidableEntity { Name = "Test" };
                ValidableEntityChild child;
                for (var i = 0; i < 10; i++)
                {
                    child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                    entity.Children.Add(child);
                    child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                }
                await unitOfWork.SaveAsync(entity);
                Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);

                entity.Name = "New name";
                await unitOfWork.SaveAsync(entity);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(2, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the child from the collection and set it as deleted
                // OnPreUpdateCollection will trigger the validation
                child = entity.Children.First();
                entity.Children.Remove(child);
                await unitOfWork.DeleteAsync(child);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(3, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the releationship with the child on both sides
                child = entity.Children.First();
                entity.Children.Remove(child);
                child.ValidableEntity = null;
                await unitOfWork.DeleteAsync(child);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(4, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the subchild from the child on both sides
                child = entity.Children.First();
                var subChild = child.Children.First();
                child.Children.Remove(subChild);
                subChild.ValidableEntityChild = null;
                await unitOfWork.FlushAsync();
                Assert.AreEqual(5, ValidableEntityValidationContextFiller.FilledCount);
                await unitOfWork.CommitAsync();
            }
            Assert.AreEqual(5, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void ShouldNotLeakWhenTheTransactionIsNotCommited()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                var entity = new ValidableEntity { Name = "Test" };
                ValidableEntityChild child;
                for (var i = 0; i < 10; i++)
                {
                    child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                    entity.Children.Add(child);
                    child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                }
                unitOfWork.Save(entity);
                Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            }
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void ShouldNotLeakForNonManagedSession()
        {
            using (var unitOfWork = Kernel.Get<ISessionFactory>().OpenSession())
            {
                var entity = new IdentityValidableEntity { Name = "Test" };
                IdentityValidableChildEntity child;
                for (var i = 0; i < 10; i++)
                {
                    child = new IdentityValidableChildEntity { Name = $"Code{i}", IdentityValidableEntity = entity };
                    entity.Children.Add(child);
                }
                unitOfWork.Save(entity);
            }
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void VariuosIdentityChildSubChildDeletionShouldTriggerRootToValidateAutomatically()
        {
            //ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                var entity = new IdentityValidableEntity { Name = "Test" };
                IdentityValidableChildEntity child;
                for (var i = 0; i < 10; i++)
                {
                    child = new IdentityValidableChildEntity { Name = $"Name{i}", IdentityValidableEntity= entity };
                    entity.Children.Add(child);
                }
                unitOfWork.Save(entity);
                //Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);

                entity.Name = "New name";
                unitOfWork.Save(entity);
                unitOfWork.Flush();
                //Assert.AreEqual(2, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the child from the collection and set it as deleted
                // OnPreUpdateCollection will trigger the validation
                child = entity.Children.First();
                entity.Children.Remove(child);
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                //Assert.AreEqual(3, ValidableEntityValidationContextFiller.FilledCount);

                // Remove the releationship with the child on both sides
                child = entity.Children.First();
                entity.Children.Remove(child);
                child.IdentityValidableEntity = null;
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                //Assert.AreEqual(4, ValidableEntityValidationContextFiller.FilledCount);

                unitOfWork.Commit();
            }
            Assert.AreEqual(5, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidRootShouldThrowOnFlushAutomaticValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = new ValidableEntity { Name = "Test2" };
                        ValidableEntityChild child;
                        for (var i = 0; i < 10; i++)
                        {
                            child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                            entity.Children.Add(child);
                            child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                        }
                        unitOfWork.Save(entity);
                        Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
                        entity.Name = null;
                        unitOfWork.Flush();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(2, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidRootShouldThrowOnFlushAutomaticValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = new ValidableEntity { Name = "Test2" };
                        ValidableEntityChild child;
                        for (var i = 0; i < 10; i++)
                        {
                            child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                            entity.Children.Add(child);
                            child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                        }
                        await unitOfWork.SaveAsync(entity);
                        Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
                        entity.Name = null;
                        await unitOfWork.FlushAsync();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(2, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidRootShouldThrowOnAutomaticValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        entity.Name = null;
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidRootShouldThrowOnAutomaticValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        entity.Name = null;
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void AutoValidationShouldNotThrowWhenChildIsSwitchedToANonRegisteredParent()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var entity = unitOfWork.Query<ValidableEntity>()
                        .First(o => o.Name == "Generated1");
                    var child = entity.Children.First();
                    entity.Children.Remove(child);
                    var newEntity = new ValidableEntity
                    {
                        Name = "Test3"
                    };
                    child.ValidableEntity = newEntity;
                    child.CountryCode = "CannotUpdate";
                    newEntity.Children.Add(child);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void PersistedChildShouldBeValidatedWhenAttachedToTransientParent()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var child = new ValidableEntityChild
                        {
                            CountryCode = "Code0"
                        };
                        unitOfWork.Save(child);
                        unitOfWork.Flush();

                        var newEntity = new ValidableEntity
                        {
                            Name = "Test3"
                        };
                        child.ValidableEntity = newEntity;
                        child.CountryCode = "CannotUpdate";
                        newEntity.Children.Add(child);
                        unitOfWork.Save(newEntity);
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void PersistedIdentityChildShouldNotBeValidatedWhenAttachedToTransientParent()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var child = new IdentityValidableChildEntity
                    {
                        Name = "My name"
                    };
                    unitOfWork.Save(child);
                    unitOfWork.Flush();

                    var newEntity = new IdentityValidableEntity
                    {
                        Name = "Test3"
                    };
                    child.IdentityValidableEntity = newEntity;
                    child.Name = "CannotUpdate";
                    newEntity.Children.Add(child);
                    unitOfWork.Save(newEntity);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void RootChildShouldNotThrowAStackOverflowException()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var parent = new ValidableRootChild
                    {
                        Name = "Parent",
                    };
                    var child = new ValidableRootChild
                    {
                        Name = "Child",
                        Parent = parent
                    };
                    var subChild = new ValidableRootChild
                    {
                        Name = "SubChild",
                        Parent = child
                    };

                    unitOfWork.Save(subChild);
                    unitOfWork.Flush();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidRootChildThrowOnAutomaticValidation()
        {
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var parent = new ValidableRootChild
                        {
                            Name = null,
                        };
                        var child = new ValidableRootChild
                        {
                            Name = "Child",
                            Parent = parent
                        };
                        var subChild = new ValidableRootChild
                        {
                            Name = "SubChild",
                            Parent = child
                        };

                        unitOfWork.Save(subChild);
                        unitOfWork.Flush();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        child.CountryCode = "Unknown";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        child.CountryCode = "Unknown";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void ChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnly()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<PowerArhitectureException> (() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        unitOfWork.DefaultSession.SetReadOnly(entity, true);
                        var child = entity.Children.First();
                        child.CountryCode = "Code2";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void ChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnlyAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<PowerArhitectureException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        unitOfWork.DefaultSession.SetReadOnly(entity, true);
                        var child = entity.Children.First();
                        child.CountryCode = "Code2";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidDeletedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        entity.Children.Remove(child);
                        child.ValidableEntity = null;
                        child.CountryCode = "CannotDelete";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidDeletedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        entity.Children.Remove(child);
                        child.ValidableEntity = null;
                        child.CountryCode = "CannotDelete";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidDeletedIdentityChildShouldNotThrowOnAutomaticRootValidation()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var entity = unitOfWork.Query<IdentityValidableEntity>()
                        .First();
                    var child = entity.Children.First();
                    entity.Children.Remove(child);
                    child.IdentityValidableEntity = null;
                    child.Name = "CannotDelete";
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void DeletedIdentityParentShouldNotThrowOnAutomaticValidation()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var entity = unitOfWork.Query<IdentityValidableEntity>()
                        .First();
                    unitOfWork.Delete(entity);
                    while (entity.Children.Any())
                    {
                        var child = entity.Children.First();
                        child.IdentityValidableEntity = null;
                        entity.Children.Remove(child);
                        child.Name = "CannotUpdate";
                    }
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidManuallyDeletedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        entity.Children.Remove(child);
                        unitOfWork.Delete(child);
                        child.CountryCode = "CannotDelete";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidManuallyDeletedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First();
                        entity.Children.Remove(child);
                        await unitOfWork.DeleteAsync(child);
                        child.CountryCode = "CannotDelete";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidInsertedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        entity.Children.Add(new ValidableEntityChild
                        {
                            CountryCode = "CannotInsert",
                            ValidableEntity = entity
                        });
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidInsertedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.ThrowsAsync<ExtendedValidationException>(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        entity.Children.Add(new ValidableEntityChild
                        {
                            CountryCode = "CannotInsert",
                            ValidableEntity = entity
                        });
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    await unitOfWork.CommitAsync();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First().Children.First();
                        child.Name = null;
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void SubChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnly()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<PowerArhitectureException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        unitOfWork.DefaultSession.SetReadOnly(entity, true);
                        var child = entity.Children.First().Children.First();
                        child.Name = "New name";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidDeletedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First().Children.First();
                        entity.Children.First().Children.Remove(child);
                        child.ValidableEntityChild = null;
                        child.Name = "CannotDelete";
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidManuallyDeletedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1");
                        var child = entity.Children.First().Children.First();
                        entity.Children.First().Children.Remove(child);
                        unitOfWork.Delete(child);
                        child.Name = "CannotDelete";

                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        [Test]
        public void InvalidInsertedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityValidationContextFiller.FilledCount = 0;
            Assert.Throws<ExtendedValidationException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var entity = unitOfWork.Query<ValidableEntity>()
                            .First(o => o.Name == "Generated1").Children.First();
                        entity.Children.Add(new ValidableEntitySubChild
                        {
                            Name = "CannotInsert",
                            ValidableEntityChild = entity
                        });
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(0, ValidableEntityValidationContextFiller.FilledCount);
            CheckValidationListenerForLeakage();
        }

        protected override void FillData(ISessionFactory sessionFactory)
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>())
            {
                try
                {
                    for (var i = 0; i < 100; i++)
                    {
                        unitOfWork.Save(new Country { Code = $"Code{i}", Name = $"Code{i}" });
                    }
                    unitOfWork.Flush();

                    for (var j = 0; j < 5; j++)
                    {
                        var entity = new ValidableEntity { Name = $"Generated{j}" };
                        for (var i = 0; i < 10; i++)
                        {
                            var child = new ValidableEntityChild { CountryCode = $"Code{i}", ValidableEntity = entity };
                            entity.Children.Add(child);
                            child.Children.Add(new ValidableEntitySubChild { Name = "My name", ValidableEntityChild = child });
                        }
                        unitOfWork.Save(entity);

                        var idEntity = new IdentityValidableEntity { Name = $"Generated{j}" };
                        for (var i = 0; i < 10; i++)
                        {
                            var child = new IdentityValidableChildEntity { Name = $"Name{i}", IdentityValidableEntity = idEntity };
                            idEntity.Children.Add(child);
                        }
                        unitOfWork.Save(idEntity);

                    }

                    
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
        }

        private void CheckValidationListenerForLeakage()
        {
            var valiadtionListener = Kernel.Get<ValidatePreInsertUpdateDeleteEventHandler>();
            var dict =
                    valiadtionListener.GetMemberValue<ConcurrentDictionary<ISession, ConcurrentSet<IEntity>>>(
                        "_validatedEntities");
            Assert.AreEqual(0, dict.Count);
        }
    }
}
