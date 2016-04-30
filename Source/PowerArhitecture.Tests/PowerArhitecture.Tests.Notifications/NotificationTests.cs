using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Ninject;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Notifications.Enums;
using PowerArhitecture.Notifications.Events;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Notifications.Entities;
using PowerArhitecture.Tests.Notifications.EventListeners;
using PowerArhitecture.Tests.Notifications.Repositories;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.Notifications
{
    [TestClass]
    public class NotificationTests : BaseTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            EntityAssemblies.Add(typeof(NotificationTests).Assembly);
            TestAssemblies.Add(typeof(NotificationTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
            BaseClassInitialize(testContext,
                CreateDatabaseConfiguration()
                .Conventions(c => c
                    .HiLoId(o => o.Enabled(false)
                    )
                )
            );
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            BaseClassCleanup();
        }

        #region StringRecipient

        [TestMethod]
        public void notification_string_listener_test()
        {
            var unitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            var listener = Kernel.Get<NotificationListener>();
            listener.Reset();
            using (var unitOfWork = (UnitOfWork) unitOfWorkFactory.GetNew())
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
            Assert.IsNotNull(listener.NotificationEvent);
        }

        #endregion


        #region EntityRecipient

        [TestMethod]
        public void notification_entity_listener_test()
        {
            var unitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            var listener = Kernel.Get<NotificationListener>();
            listener.Reset();
            using (var unitOfWork = (UnitOfWork)unitOfWorkFactory.GetNew())
            {
                var user1 = new User {UserName = "User1"};
                var user2 = new User {UserName = "User2"};
                unitOfWork.Save(user1, user2);

                var repo = unitOfWork.GetRepository<NotificationWithEntityRecipient>();
                var notif = new NotificationWithEntityRecipient
                {
                    Type = NotificationType.Success,
                    Message = "Mesage",
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
            Assert.IsNotNull(listener.NotificationEvent);
        }

        #endregion
    }
}
