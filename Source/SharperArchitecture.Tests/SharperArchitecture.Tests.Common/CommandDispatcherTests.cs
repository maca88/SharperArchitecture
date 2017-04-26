using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Tests.Common.Commands;

namespace SharperArchitecture.Tests.Common
{
    [TestFixture]
    public class CommandDispatcherTests : BaseTest
    {
        public CommandDispatcherTests()
        {
            TestAssemblies.Add(typeof(CommandDispatcherTests).Assembly);
        }

        [Test]
        public void CommandDispatcherMustBeSingleton()
        {
            var dispatcher = Container.GetInstance<ICommandDispatcher>();
            var dispatcher2 = Container.GetInstance<ICommandDispatcher>();

            Assert.AreEqual(dispatcher, dispatcher2);
        }

        [Test]
        public void CommandHandlerMustBeTransient()
        {
            var handler = Container.GetInstance<TestCommandHandler>();
            var handler2 = Container.GetInstance<ICommandHandler<TestCommand, bool>>();
            var handler3 = Container.GetInstance<ICommandHandler<TestCommand, bool>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(TestCommandHandler), handler3.GetType());
        }

        [Test]
        public void AsyncCommandHandlerMustBeTransient()
        {
            var handler = Container.GetInstance<TestAsyncCommandHandler>();
            var handler2 = Container.GetInstance<IAsyncCommandHandler<TestAsyncCommand, bool>>();
            var handler3 = Container.GetInstance<IAsyncCommandHandler<TestAsyncCommand, bool>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(TestAsyncCommandHandler), handler3.GetType());
        }

        [Test]
        public void VoidCommandHandlerMustBeTransient()
        {
            var handler = Container.GetInstance<VoidCommandHandler>();
            var handler2 = Container.GetInstance<ICommandHandler<VoidCommand>>();
            var handler3 = Container.GetInstance<ICommandHandler<VoidCommand>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(VoidCommandHandler), handler3.GetType());
        }

        [Test]
        public void VoidAsyncCommandHandlerMustBeTransient()
        {
            var handler = Container.GetInstance<VoidAsyncCommandHandler>();
            var handler2 = Container.GetInstance<IAsyncCommandHandler<VoidAsyncCommand>>();
            var handler3 = Container.GetInstance<IAsyncCommandHandler<VoidAsyncCommand>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(VoidAsyncCommandHandler), handler3.GetType());
        }

        [Test]
        public async Task TestCommandHandlerAsyncThenSync()
        {
            var commandDispatcher = Container.GetInstance<ICommandDispatcher>();
            var cmd = new SyncAsyncCommand();
            var result = await commandDispatcher.DispatchAsync(cmd);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1, cmd.Counter);

            result = commandDispatcher.Dispatch(cmd);
            Assert.AreEqual(true, result);
            Assert.AreEqual(2, cmd.Counter);
        }

        [Test]
        public async Task TestCommandHandlerSyncThenAsync()
        {
            var commandDispatcher = Container.GetInstance<ICommandDispatcher>();
            var cmd = new SyncAsyncCommand();
            var result = commandDispatcher.Dispatch(cmd);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1, cmd.Counter);

            result = await commandDispatcher.DispatchAsync(cmd);
            Assert.AreEqual(true, result);
            Assert.AreEqual(2, cmd.Counter);
        }

        [Test]
        public void VoidCommandHandler()
        {
            var commandDispatcher = Container.GetInstance<ICommandDispatcher>();
            var cmd = new VoidCommand();
            commandDispatcher.Dispatch(cmd);

            Assert.AreEqual(true, cmd.Success);
        }

        [Test]
        public async Task VoidCommandHandlerAsync()
        {
            var commandDispatcher = Container.GetInstance<ICommandDispatcher>();
            var cmd = new VoidAsyncCommand();
            await commandDispatcher.DispatchAsync(cmd);

            Assert.AreEqual(true, cmd.Success);
        }
    }
}
