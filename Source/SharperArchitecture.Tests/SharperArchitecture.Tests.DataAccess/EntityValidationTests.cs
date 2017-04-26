using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Extensions;
using NUnit.Framework;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.EventListeners;
using SharperArchitecture.DataAccess.NHEventListeners;
using SharperArchitecture.DataAccess.Providers;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.DataAccess.Entities;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Tests.DataAccess
{
    [TestFixture]
    public class EntityValidationTests : DatabaseBaseTest
    {
        public EntityValidationTests()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(LifecycleTests).Assembly);
        }

        [Test]
        public void VariuosChildSubChildDeletionShouldTriggerRootToValidateAutomatically()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
                Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);

                entity.Name = "New name";
                unitOfWork.Save(entity);
                unitOfWork.Flush();
                Assert.AreEqual(2, ValidableEntityBusinessRule.FilledCount);
                
                // Remove the child from the collection and set it as deleted
                // OnPreUpdateCollection will trigger the validation
                child = entity.Children.First();
                entity.Children.Remove(child);
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                Assert.AreEqual(3, ValidableEntityBusinessRule.FilledCount);

                // Remove the releationship with the child on both sides
                child = entity.Children.First();
                entity.Children.Remove(child);
                child.ValidableEntity = null;
                unitOfWork.Delete(child);
                unitOfWork.Flush();
                Assert.AreEqual(4, ValidableEntityBusinessRule.FilledCount);

                // Remove the subchild from the child on both sides
                child = entity.Children.First();
                var subChild = child.Children.First();
                child.Children.Remove(subChild);
                subChild.ValidableEntityChild = null;
                unitOfWork.Flush();
                Assert.AreEqual(5, ValidableEntityBusinessRule.FilledCount);
                unitOfWork.Commit();
            }
            Assert.AreEqual(5, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public async Task VariuosChildSubChildDeletionShouldTriggerRootToValidateAutomaticallyAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
                Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);

                entity.Name = "New name";
                await unitOfWork.SaveAsync(entity);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(2, ValidableEntityBusinessRule.FilledCount);

                // Remove the child from the collection and set it as deleted
                // OnPreUpdateCollection will trigger the validation
                child = entity.Children.First();
                entity.Children.Remove(child);
                await unitOfWork.DeleteAsync(child);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(3, ValidableEntityBusinessRule.FilledCount);

                // Remove the releationship with the child on both sides
                child = entity.Children.First();
                entity.Children.Remove(child);
                child.ValidableEntity = null;
                await unitOfWork.DeleteAsync(child);
                await unitOfWork.FlushAsync();
                Assert.AreEqual(4, ValidableEntityBusinessRule.FilledCount);

                // Remove the subchild from the child on both sides
                child = entity.Children.First();
                var subChild = child.Children.First();
                child.Children.Remove(subChild);
                subChild.ValidableEntityChild = null;
                await unitOfWork.FlushAsync();
                Assert.AreEqual(5, ValidableEntityBusinessRule.FilledCount);
                await unitOfWork.CommitAsync();
            }
            Assert.AreEqual(5, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void ShouldNotLeakWhenTheTransactionIsNotCommited()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
                Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            }
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void ShouldNotLeakForNonManagedSession()
        {
            using (var unitOfWork = Container.GetInstance<ISessionFactory>().OpenSession())
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
            CheckForLeakages();
        }

        [Test]
        public void VariuosIdentityChildSubChildDeletionShouldTriggerRootToValidateAutomatically()
        {
            //ValidableEntityValidationContextFiller.FilledCount = 0;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(5, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidRootShouldThrowOnFlushAutomaticValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
                        Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
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
            Assert.AreEqual(2, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidRootShouldThrowOnFlushAutomaticValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
                        Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
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
            Assert.AreEqual(2, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidRootShouldThrowOnAutomaticValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidRootShouldThrowOnAutomaticValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void AutoValidationShouldNotThrowWhenChildIsSwitchedToANonRegisteredParent()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void PersistedChildShouldBeValidatedWhenAttachedToTransientParent()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var flushMode = unitOfWork.DefaultSession.FlushMode;
                        var child = new ValidableEntityChild
                        {
                            CountryCode = "Code0"
                        };
                        unitOfWork.Save(child);
                        unitOfWork.Flush();
                        Assert.AreEqual(flushMode, unitOfWork.DefaultSession.FlushMode);

                        var newEntity = new ValidableEntity
                        {
                            Name = "Test3"
                        };
                        child.ValidableEntity = newEntity;
                        child.CountryCode = "CannotUpdate";
                        newEntity.Children.Add(child);
                        unitOfWork.Save(newEntity);
                        Assert.AreEqual(flushMode, unitOfWork.DefaultSession.FlushMode);
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    unitOfWork.Commit();
                }
            });
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void PersistedIdentityChildShouldNotBeValidatedWhenAttachedToTransientParent()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            CheckForLeakages();
        }

        [Test]
        public void RootChildShouldNotThrowAStackOverflowException()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            CheckForLeakages();
        }

        [Test]
        public void InvalidRootChildThrowOnAutomaticValidation()
        {
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            CheckForLeakages();
        }

        [Test]
        public void InvalidChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void ChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnly()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<SharperArchitectureException> (() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void ChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnlyAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<SharperArchitectureException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidDeletedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidDeletedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidDeletedIdentityChildShouldNotThrowOnAutomaticRootValidation()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            CheckForLeakages();
        }

        [Test]
        public void DeletedIdentityParentShouldNotThrowOnAutomaticValidation()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            CheckForLeakages();
        }

        [Test]
        public void InvalidManuallyDeletedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidManuallyDeletedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidInsertedChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidInsertedChildShouldThrowOnAutomaticRootValidationAsync()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.ThrowsAsync<EntityValidationException>(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void SubChildShouldThrowOnAutomaticRootValidationWhenRootIsReadOnly()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<SharperArchitectureException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidDeletedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidManuallyDeletedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(1, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }

        [Test]
        public void InvalidInsertedSubChildShouldThrowOnAutomaticRootValidation()
        {
            ValidableEntityBusinessRule.FilledCount = 0;
            Assert.Throws<EntityValidationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
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
            Assert.AreEqual(0, ValidableEntityBusinessRule.FilledCount);
            CheckForLeakages();
        }


        [Test]
        public void SessionShouldNotFlushUponValidation()
        {
            using (var session = Container.GetInstance<ISessionFactory>().OpenSession())
            using(var trans = session.BeginTransaction(IsolationLevel.Serializable))
            {

                trans.Commit();
            }

            var wasCommited = false;
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                try
                {
                    var user = new UserExistance {UserName = "Test"};
                    unitOfWork.Save(user);
                    Assert.AreEqual("Test", user.UserName);
                    Assert.IsTrue(unitOfWork.ContainsDefaultSession());
                    unitOfWork.DefaultSession.Subscribe(o => o.Transaction.AfterCommit((session, b) =>
                    {
                        wasCommited = true;
                    }));
                    unitOfWork.Commit();
                    Assert.IsTrue(wasCommited);
                }
                catch
                {
                    unitOfWork.Rollback();
                }
            }
        }

        protected override void FillData(ISessionFactory sessionFactory)
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
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

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        private void CheckForLeakages()
        {
            var valiadtionListener = Container.GetInstance<ValidatePreInsertUpdateDeleteEventHandler>();
            var dict =
                    valiadtionListener.GetMemberValue<ConcurrentDictionary<ISession, ConcurrentSet<IEntity>>>(
                        "_validatedEntities");
            Assert.AreEqual(0, dict.Count);

            var sessionIds =(ConcurrentSet<Guid>)Container.GetInstance<SessionProvider>().GetMemberValue("RegisteredSessionIds");
            Assert.AreEqual(0, sessionIds.Count);
        }
    }
}
