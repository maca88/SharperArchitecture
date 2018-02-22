using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Internationalization;
using NUnit.Framework;

namespace SharperArchitecture.Tests.Common
{
    [TestFixture]
    public class Internationalization
    {
        [Test]
        public void TestFormatter()
        {
            var test1 = TranslatorFormatter.Custom("{Test} {0} {0} {1} {{0}} {{escape}}", "str1", "str2", new {Test = "test"});
            Assert.AreEqual("test str1 str1 str2 {0} {escape}", test1);
        }
    }
}
