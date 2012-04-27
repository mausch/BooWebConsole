using System;
using BooWebConsole.Views;
using MiniMVC;
using NUnit.Framework;

namespace BooWebConsole.Tests {
    [TestFixture]
    public class ViewsTests {
        [Test]
        public void Raw() {            
            var xml = Views.Views.Index(new Context());
            Console.WriteLine(xml.ToString());
        }

        [Test]
        public void HTMLCompatible() {
            var xml = Views.Views.Index(new Context()).MakeHTML5Doc();
            Console.WriteLine(xml.ToString());
        }
    }
}
