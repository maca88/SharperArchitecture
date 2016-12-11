using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
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
            var publisher = Container.GetInstance<IEventPublisher>();
            var publisher2 = Container.GetInstance<IEventPublisher>();

            Assert.AreEqual(publisher, publisher2);
        }

        [Test]
        public void TestEventHandlerMustBeSingleton()
        {
            var eventHandler = Container.GetInstance<TestEventHandler>();
            var eventHandler2 = Container.GetInstance<TestEventHandler>();

            Assert.AreEqual(eventHandler, eventHandler2);
        }

        [Test]
        public void TestSingletonEventHandler()
        {
            var eventHandler = Container.GetInstance<TestEventHandler>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            eventPublisher.Publish(new TestEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);
        }

        [Test]
        public async Task TestSingletonEventHandlerAsync()
        {
            var eventHandler = Container.GetInstance<TestAsyncEventHandler>();
            eventHandler.CallCounter = 0;
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            await eventPublisher.PublishAsync(new TestAsyncEvent("test"));

            Assert.AreEqual(1, eventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);
        }

        [Test]
        public async Task TestSingletonEventHandlerSyncThenAsync()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            var evt = new SyncAsyncEvent();

            eventPublisher.Publish(evt);
            Assert.AreEqual(1, evt.Counter);

            await eventPublisher.PublishAsync(evt);
            Assert.AreEqual(2, evt.Counter);
        }

        [Test]
        public async Task TestSingletonEventHandlerAsyncThenSync()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            var evt = new SyncAsyncEvent();

            await eventPublisher.PublishAsync(evt);
            Assert.AreEqual(1, evt.Counter);

            eventPublisher.Publish(evt);
            Assert.AreEqual(2, evt.Counter);
        }

        [Test]
        public void FaultyEventHandlerThrowing()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            Assert.Throws<InvalidOperationException>(() => eventPublisher.Publish(new FaultyEvent("test2")));
        }

        [Test]
        public void FaultyEventHandlerThrowingAsync()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            Assert.ThrowsAsync<InvalidOperationException>(() => eventPublisher.PublishAsync(new FaultyAsyncEvent("test2")));
        }

        [Test]
        public void MultipleEventsPerHandler()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();

            var evt = new MultipleEventsPerHandlerEvent();
            var evt2 = new MultipleEventsPerHandler2Event();
            eventPublisher.Publish(evt);
            eventPublisher.Publish(evt2);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
        }

        [Test]
        public async Task MultipleEventsPerHandlerAsync()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();

            var evt = new MultipleEventsPerHandlerAsyncEvent();
            var evt2 = new MultipleEventsPerHandler2AsyncEvent();
            await eventPublisher.PublishAsync(evt);
            await eventPublisher.PublishAsync(evt2);

            Assert.IsTrue(evt.Success);
            Assert.IsTrue(evt2.Success);
        }

        [Test]
        public async Task SequentialAsyncHandlers()
        {
            var eventPublisher = Container.GetInstance<IEventPublisher>();

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
            var eventPublisher = Container.GetInstance<IEventPublisher>();

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
            var eventPublisher = Container.GetInstance<IEventPublisher>();

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
