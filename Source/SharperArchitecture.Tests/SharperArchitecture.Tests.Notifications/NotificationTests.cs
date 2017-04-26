using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using SharperArchitecture.Common.Events;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Notifications.Enums;
using SharperArchitecture.Notifications.Events;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.Notifications.Entities;
using SharperArchitecture.Tests.Notifications.EventListeners;
using SharperArchitecture.Tests.Notifications.Repositories;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Tests.Notifications
{
    [TestFixture]
    public class NotificationTests : DatabaseBaseTest
    {
        public NotificationTests()
        {
            EntityAssemblies.Add(typeof(NotificationTests).Assembly);
            TestAssemblies.Add(typeof(NotificationTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(NewNotificationEvent).Assembly);
        }

        protected override IFluentDatabaseConfiguration CreateDatabaseConfiguration(string dbName = "foo", string name = null)
        {
            return base.CreateDatabaseConfiguration(dbName, name)
                .Conventions(c => c
                    .HiLoId(o => o.Enabled(false))
                    .RequiredLastModifiedProperty()
                );
        }

        #region StringRecipient

        [Test]
        public void notification_string_listener_test()
        {
            var unitOfWorkFactory = Container.GetInstance<IUnitOfWorkFactory>();
            var listener = Container.GetInstance<NotificationHandler>();
            listener.Reset();
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                try
                {
                    var repo = unitOfWork.GetRepository<NotificationWithStringRecipient>();
                    var notif = new NotificationWithStringRecipient
                    {
                        Type = NotificationType.Success,
                        Message = "Mesage",
                    };
                    notif.AddRecipient(new NotificationRecipientWithStringRecipient
                    {
                        Recipient = "Recipient1"
                    });
                    notif.AddRecipient(new NotificationRecipientWithStringRecipient
                    {
                        Recipient = "Recipient2"
                    });
                    repo.Save(notif);
                    Assert.IsNull(listener.NotificationEvent);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            Assert.IsNotNull(listener.NotificationEvent);
        }

        #endregion


        #region EntityRecipient

        [Test]
        public void notification_entity_listener_test()
        {
            var unitOfWorkFactory = Container.GetInstance<IUnitOfWorkFactory>();
            var listener = Container.GetInstance<NotificationHandler>();
            listener.Reset();
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                try
                {
                    var user1 = new User { UserName = "User1" };
                    var user2 = new User { UserName = "User2" };
                    unitOfWork.Save(user1, user2);
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("User1"), null);

                    var repo = unitOfWork.GetRepository<NotificationWithEntityRecipient>();
                    var notif = new NotificationWithEntityRecipient
                    {
                        Type = NotificationType.Success,
                        Message = "Mesage"
                    };
                    notif.AddRecipient(new NotificationRecipientWithEntityRecipient
                    {
                        Recipient = user1
                    });
                    notif.AddRecipient(new NotificationRecipientWithEntityRecipient
                    {
                        Recipient = user2
                    });
                    repo.Save(notif);
                    Assert.IsNull(listener.NotificationEvent);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            Assert.IsNotNull(listener.NotificationEvent);
        }

        #endregion
    }
}
