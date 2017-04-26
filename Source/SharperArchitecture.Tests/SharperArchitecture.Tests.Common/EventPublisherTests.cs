using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Tests.Common.Events;

namespace SharperArchitecture.Tests.Common
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
        public void EventSubscriberMustBeSingleton()
        {
            var subscriber = Container.GetInstance<IEventSubscriber>();
            var subscriber2 = Container.GetInstance<IEventSubscriber>();

            Assert.AreEqual(subscriber, subscriber2);
        }

        [Test]
        public void EventSubscriberAndEventPublisherMustBeEqual()
        {
            var subscriber = Container.GetInstance<IEventSubscriber>();
            var publisher = Container.GetInstance<IEventPublisher>();

            Assert.AreEqual(subscriber, publisher);
        }


        [Test]
        public void TestEventHandlerMustBeSingleton()
        {
            var eventHandler = Container.GetInstance<TestEventHandler>();
            var eventHandler2 = Container.GetInstance<TestEventHandler>();

            Assert.AreEqual(eventHandler, eventHandler2);
        }

        [Test]
        public void TransientEventHandlerMustBeTransient()
        {
            var eventHandler = Container.GetInstance<TransientEventHandler>();
            var eventHandler2 = Container.GetInstance<TransientEventHandler>();

            Assert.AreNotEqual(eventHandler, eventHandler2);
        }

        [Test]
        public void TestSingletonEventHandler()
        {
            TestEventHandler.CallCounter = 0;

            var eventHandler = Container.GetInstance<TestEventHandler>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            eventPublisher.Publish(new TestEvent("test"));

            Assert.AreEqual(1, TestEventHandler.CallCounter);
            Assert.AreEqual("test", eventHandler.ReceivedMesssage);
        }

        [Test]
        public void TestTransientEventHandler()
        {
            TransientEventHandler.CallCounter = 0;
            TransientEventHandler.CreatedTimes = 0;

            var eventPublisher = Container.GetInstance<IEventPublisher>();
            eventPublisher.Publish(new TransientEvent("test"));

            Assert.AreEqual(1, TransientEventHandler.CallCounter);
            Assert.AreEqual(1, TransientEventHandler.CreatedTimes);

            eventPublisher.Publish(new TransientEvent("test"));

            Assert.AreEqual(2, TransientEventHandler.CallCounter);
            Assert.AreEqual(2, TransientEventHandler.CreatedTimes);
        }

        [Test]
        public void TestDelegateEventHandler()
        {
            TestEventHandler.CallCounter = 0;

            var eventSubscriber = Container.GetInstance<IEventSubscriber>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            var called = false;

            SharperArchitecture.Common.Events.EventHandler<TestEvent> handler = e => { called = true; };
            eventSubscriber.Subscribe(handler);

            eventPublisher.Publish(new TestEvent("test"));
            Assert.AreEqual(1, TestEventHandler.CallCounter);
            Assert.IsTrue(called);

            eventSubscriber.Unsubscribe(handler);

            CheckForLeakage(eventSubscriber);
        }

        [Test]
        public async Task TestAsyncDelegateEventHandler()
        {
            TestAsyncEventHandler.CallCounter = 0;

            var eventSubscriber = Container.GetInstance<IEventSubscriber>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            var called = false;

            AsyncEventHandler<TestAsyncEvent> handler = async (e, t) =>
            {
                await Task.Yield();
                called = true;
            };
            eventSubscriber.Subscribe(handler);

            await eventPublisher.PublishAsync(new TestAsyncEvent("test"));
            Assert.AreEqual(1, TestAsyncEventHandler.CallCounter);
            Assert.IsTrue(called);

            eventSubscriber.Unsubscribe(handler);

            CheckForLeakage(eventSubscriber);
        }

        [Test]
        public void TestMethodEventHandler()
        {
            TestEventHandler.CallCounter = 0;
            _isTestEventMethodCalled = false;

            var eventSubscriber = Container.GetInstance<IEventSubscriber>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            
            eventSubscriber.Subscribe<TestEvent>(TestEventMethod);

            eventPublisher.Publish(new TestEvent("test"));
            Assert.AreEqual(1, TestEventHandler.CallCounter);
            Assert.IsTrue(_isTestEventMethodCalled);

            eventSubscriber.Unsubscribe<TestEvent>(TestEventMethod);

            CheckForLeakage(eventSubscriber);
        }

        private bool _isTestEventMethodCalled;

        private void TestEventMethod(TestEvent e)
        {
            _isTestEventMethodCalled = true;
        }

        [Test]
        public async Task TestAsyncMethodEventHandler()
        {
            TestAsyncEventHandler.CallCounter = 0;
            _isAsyncTestEventMethodCalled = false;

            var eventSubscriber = Container.GetInstance<IEventSubscriber>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();

            eventSubscriber.Subscribe<TestAsyncEvent>(AsyncTestEventMethod);

            await eventPublisher.PublishAsync(new TestAsyncEvent("test"));
            Assert.AreEqual(1, TestAsyncEventHandler.CallCounter);
            Assert.IsTrue(_isAsyncTestEventMethodCalled);

            eventSubscriber.Unsubscribe<TestAsyncEvent>(AsyncTestEventMethod);

            CheckForLeakage(eventSubscriber);
        }

        private bool _isAsyncTestEventMethodCalled;

        private async Task AsyncTestEventMethod(TestAsyncEvent e, CancellationToken token)
        {
            await Task.Yield();
            _isAsyncTestEventMethodCalled = true;
        }

        private void CheckForLeakage(IEventSubscriber subscriber)
        {
            var coll = (ICollection) subscriber.GetMemberValue("_delegateEventHandlers");
            Assert.AreEqual(0, coll.Count);
        }

        [Test]
        public async Task TestSingletonEventHandlerAsync()
        {
            TestAsyncEventHandler.CallCounter = 0;

            var eventHandler = Container.GetInstance<TestAsyncEventHandler>();
            var eventPublisher = Container.GetInstance<IEventPublisher>();
            await eventPublisher.PublishAsync(new TestAsyncEvent("test"));

            Assert.AreEqual(1, TestAsyncEventHandler.CallCounter);
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
