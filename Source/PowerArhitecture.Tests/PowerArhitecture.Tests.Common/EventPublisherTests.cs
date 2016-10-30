using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Tests.Common.Events;

namespace PowerArhitecture.Tests.Common
{
    [TestFixture]
    public class EventPublisherTests : BaseTest
    {
        public EventPublisherTests()
        {
            TestAssemblies.Add(typeof(EventPublisherTests).Assembly);
        }

        [Test]
        public void EventPublisherMustBeSingleton()
        {
            var publisher = Kernel.Get<IEventPublisher>();
            var publisher2 = Kernel.Get<IEventPublisher>();

            Assert.AreEqual(publisher, publisher2);
        }

        [Test]
        public void TestEventHandlerMustBeSingleton()
        {
            var eventHandler = Kernel.Get<TestEventHandler>();
            var eventHandler2 = Kernel.Get<TestEventHandler>();

            Assert.AreEqual(eventHandler, eventHandler2);
        }

        [Test]
        public void TestSingletonEventHandler()
        {
            var eventHandler = Kernel.Get<TestEventHandler>();
            var eventPublisher = Kernel.Get<IEventPublisher>();
            eventPublisher.Publish(new TestEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);
        }

        [Test]
        public async Task TestSingletonEventHandlerAsync()
        {
            var eventHandler = Kernel.Get<TestEventHandler>();
            eventHandler.CallCounter = 0;
            var eventPublisher = Kernel.Get<IEventPublisher>();
            await eventPublisher.PublishAsync(new TestEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);
        }

        [Test]
        public async Task TestSingletonEventHandlerSyncThenAsync()
        {
            var eventHandler = Kernel.Get<TestEventHandler>();
            eventHandler.CallCounter = 0;
            var eventPublisher = Kernel.Get<IEventPublisher>();
            eventPublisher.Publish(new TestEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);

            await eventPublisher.PublishAsync(new TestEvent("test2"));

            Assert.AreEqual(2, eventHandler.CallCounter);
            Assert.AreEqual("test2", eventHandler.ReceivedMesssage);
        }

        [Test]
        public async Task TestSingletonEventHandlerAsyncThenSync()
        {
            var eventHandler = Kernel.Get<TestEventHandler>();
            eventHandler.CallCounter = 0;
            var eventPublisher = Kernel.Get<IEventPublisher>();
            await eventPublisher.PublishAsync(new TestEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);

            eventPublisher.Publish(new TestEvent("test2"));

            Assert.AreEqual(2, eventHandler.CallCounter);
            Assert.AreEqual("test2", eventHandler.ReceivedMesssage);
        }

        [Test]
        public void FaultyEventHandlerThrowing()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            Assert.ThrowsAsync<InvalidOperationException>(() => eventPublisher.PublishAsync(new FaultyEvent("test2")));
        }

        [Test]
        public void TwoEventsPerHandler()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new TwoEventsPerHandlerEvent();
            var evt2 = new TwoEventsPerHandler2Event();
            eventPublisher.Publish(evt);
            eventPublisher.Publish(evt2);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
        }

        [Test]
        public async Task TwoEventsPerHandlerAsync()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new TwoEventsPerHandlerEvent();
            var evt2 = new TwoEventsPerHandler2Event();
            await eventPublisher.PublishAsync(evt);
            await eventPublisher.PublishAsync(evt2);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
        }

        [Test]
        public void ThreeEventsPerHandler()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new ThreeEventsPerHandlerEvent();
            var evt2 = new ThreeEventsPerHandler2Event();
            var evt3 = new ThreeEventsPerHandler3Event();
            eventPublisher.Publish(evt);
            eventPublisher.Publish(evt2);
            eventPublisher.Publish(evt3);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
            Assert.IsTrue(evt3.Success);
        }

        [Test]
        public async Task ThreeEventsPerHandlerAsync()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new ThreeEventsPerHandlerEvent();
            var evt2 = new ThreeEventsPerHandler2Event();
            var evt3 = new ThreeEventsPerHandler3Event();
            await eventPublisher.PublishAsync(evt);
            await eventPublisher.PublishAsync(evt2);
            await eventPublisher.PublishAsync(evt3);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
            Assert.IsTrue(evt3.Success);
        }

        [Test]
        public void FourEventsPerHandler()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new FourEventsPerHandlerEvent();
            var evt2 = new FourEventsPerHandler2Event();
            var evt3 = new FourEventsPerHandler3Event();
            var evt4 = new FourEventsPerHandler4Event();
            eventPublisher.Publish(evt);
            eventPublisher.Publish(evt2);
            eventPublisher.Publish(evt3);
            eventPublisher.Publish(evt4);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
            Assert.IsTrue(evt3.Success);
            Assert.IsTrue(evt4.Success);
        }

        [Test]
        public async Task FourEventsPerHandlerAsync()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var evt = new ThreeEventsPerHandlerEvent();
            var evt2 = new ThreeEventsPerHandler2Event();
            var evt3 = new FourEventsPerHandler3Event();
            var evt4 = new FourEventsPerHandler4Event();
            await eventPublisher.PublishAsync(evt);
            await eventPublisher.PublishAsync(evt2);
            await eventPublisher.PublishAsync(evt3);
            await eventPublisher.PublishAsync(evt4);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
            Assert.IsTrue(evt3.Success);
            Assert.IsTrue(evt4.Success);
        }

        [Test]
        public async Task SequentialAsyncHandlers()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var startedStack = new Stack<string>();
            startedStack.Push("SequentialAsync2EventHandler");
            startedStack.Push("SequentialAsyncEventHandler");

            var completedStack = new Stack<string>();
            completedStack.Push("SequentialAsync2EventHandler");
            completedStack.Push("SequentialAsyncEventHandler");

            await eventPublisher.PublishAsync(new SequentialAsyncEvent
            {
                OnStarted = handler =>
                {
                    Assert.AreEqual(startedStack.Pop(), handler.GetType().Name);
                },
                OnCompleted = handler =>
                {
                    Assert.AreEqual(completedStack.Pop(), handler.GetType().Name);
                    Assert.AreEqual(completedStack.Count, startedStack.Count);
                }
            });
        }

        [Test]
        public async Task SequentialAsyncHandlersCancellation()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var startedStack = new Stack<string>();
            startedStack.Push("SequentialAsync2EventHandler");
            startedStack.Push("SequentialAsyncEventHandler");

            var completedStack = new Stack<string>();
            completedStack.Push("SequentialAsync2EventHandler");
            completedStack.Push("SequentialAsyncEventHandler");

            var tokenSource = new CancellationTokenSource();
            await eventPublisher.PublishAsync(new SequentialAsyncEvent
            {
                OnStarted = handler =>
                {
                    Assert.AreEqual(startedStack.Pop(), handler.GetType().Name);
                },
                OnCompleted = handler =>
                {
                    tokenSource.Cancel();
                    Assert.AreEqual(completedStack.Pop(), handler.GetType().Name);
                    Assert.AreEqual(completedStack.Count, startedStack.Count);
                }
            }, tokenSource.Token);

            Assert.AreEqual(1, startedStack.Count);
            Assert.AreEqual(1, completedStack.Count);
        }

        [Test]
        public async Task SequentialAsyncPriorityHandlers()
        {
            var eventPublisher = Kernel.Get<IEventPublisher>();

            var startedStack = new Stack<string>();
            startedStack.Push("SequentialAsyncPriorityEventHandler");
            startedStack.Push("SequentialAsyncPriority2EventHandler");

            var completedStack = new Stack<string>();
            completedStack.Push("SequentialAsyncPriorityEventHandler");
            completedStack.Push("SequentialAsyncPriority2EventHandler");

            await eventPublisher.PublishAsync(new SequentialAsyncPriorityEvent
            {
                OnStarted = handler =>
                {
                    Assert.AreEqual(startedStack.Pop(), handler.GetType().Name);
                },
                OnCompleted = handler =>
                {
                    Assert.AreEqual(completedStack.Pop(), handler.GetType().Name);
                    Assert.AreEqual(completedStack.Count, startedStack.Count);
                }
            });
        }

    }
}
