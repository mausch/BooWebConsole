using System;
using BooWebConsole.Views;
using NUnit.Framework;

namespace BooWebConsole.Tests {
    [TestFixture]
    public class ViewsTests {
        [Test]
        public void tt() {            
            var xml = Views.Views.IndexDocument(new Context());
            
            Console.WriteLine(xml.ToString());
        }
    }
}
