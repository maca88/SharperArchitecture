﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Tests.Common.Commands;

namespace PowerArhitecture.Tests.Common
{
    [TestFixture]
    public class CommandDispatcherTests : BaseTest
    {
        public CommandDispatcherTests()
        {
            TestAssemblies.Add(typeof(CommandDispatcherTests).Assembly);
        }

        [Test]
        public void CommandDispatcherMustBeTransient()
        {
            var dispatcher = Kernel.Get<ICommandDispatcher>();
            var dispatcher2 = Kernel.Get<ICommandDispatcher>();

            Assert.AreNotEqual(dispatcher, dispatcher2);
        }

        [Test]
        public void CommandHandlerMustBeTransient()
        {
            var handler = Kernel.Get<TestCommandHandler>();
            var handler2 = Kernel.Get<TestCommandHandler>();
            var handler3 = Kernel.Get<ICommandHandler<TestCommand, bool>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(TestCommandHandler), handler3.GetType());
        }

        [Test]
        public void VoidCommandHandlerMustBeTransient()
        {
            var handler = Kernel.Get<VoidCommandHandler>();
            var handler2 = Kernel.Get<VoidCommandHandler>();
            var handler3 = Kernel.Get<ICommandHandler<VoidCommand>>();

            Assert.AreNotEqual(handler, handler2);
            Assert.AreNotEqual(handler2, handler3);
            Assert.AreEqual(typeof(VoidCommandHandler), handler3.GetType());
        }

        [Test]
        public async Task TestCommandHandlerAsyncThenSync()
        {
            var commandDispatcher = Kernel.Get<ICommandDispatcher>();
            var result = await commandDispatcher.DispatchAsync(new TestCommand());

            Assert.AreEqual(true, result);

            result = commandDispatcher.Dispatch(new TestCommand());

            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task TestCommandHandlerSyncThenAsync()
        {
            var commandDispatcher = Kernel.Get<ICommandDispatcher>();
            var result = commandDispatcher.Dispatch(new TestCommand());

            Assert.AreEqual(true, result);

            result = await commandDispatcher.DispatchAsync(new TestCommand());

            Assert.AreEqual(true, result);
        }

        [Test]
        public void VoidCommandHandler()
        {
            var commandDispatcher = Kernel.Get<ICommandDispatcher>();
            var cmd = new VoidCommand();
            commandDispatcher.Dispatch(cmd);

            Assert.AreEqual(true, cmd.Success);
        }

        [Test]
        public async Task VoidCommandHandlerAsync()
        {
            var commandDispatcher = Kernel.Get<ICommandDispatcher>();
            var cmd = new VoidCommand();
            await commandDispatcher.DispatchAsync(cmd);

            Assert.AreEqual(true, cmd.Success);
        }

        [Test]
        public void ModelCommandHandler()
        {
            var commandDispatcher = Kernel.Get<ICommandDispatcher>();
            var model = new Model();
            var cmd = new ModelCommand(model);
            commandDispatcher.Dispatch(cmd);

            Assert.AreEqual("Test", model.Name);
        }
    }
}