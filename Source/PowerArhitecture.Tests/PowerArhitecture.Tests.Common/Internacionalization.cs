using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Internationalization;
using NUnit.Framework;

namespace PowerArhitecture.Tests.Common
{
    [TestFixture]
    public class Internacionalization
    {
        [Test]
        public void Test()
        {
            var test1 = TranslatorFormatter.Custom("{Drek} {0} {0} {1} {{0}} {{escape}}", "str1", "str2", new {Drek = "dela"});
            Assert.AreEqual("dela str1 str1 str2 {0} {escape}", test1);
        }
    }
}
