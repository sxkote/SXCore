using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;

namespace Tests.Lexems
{
    [TestClass]
    public class TestFunctionality
    {
        [TestMethod]
        public void Lexem_Split_simple()
        {
            var values = Lexem.Split("aaa ; bbb , ccc", new char[]{',',';'});
            Assert.AreEqual("aaa ", values[0]);
            Assert.AreEqual(" bbb ", values[1]);
            Assert.AreEqual(" ccc", values[2]);
        }

        [TestMethod]
        public void Lexem_Split_with_brackets()
        {
            var values = Lexem.Split("aaa ; {bbb , ccc} , ddd", new char[] { ',', ';' }, new SymbolPair[] { new SymbolPair('{', '}') });
            Assert.AreEqual("aaa ", values[0]);
            Assert.AreEqual(" {bbb , ccc} ", values[1]);
            Assert.AreEqual(" ddd", values[2]);
        }

        [TestMethod]
        public void Lexem_Split_with_quotes()
        {
            var values = Lexem.Split("aaa ; \"bbb ,' 'ccc\" , ddd", new char[] { ',', ';' }, new SymbolPair[] { new SymbolPair('{', '}') });
            Assert.AreEqual("aaa ", values[0]);
            Assert.AreEqual(" \"bbb ,' 'ccc\" ", values[1]);
            Assert.AreEqual(" ddd", values[2]);
        }

        [TestMethod]
        public void Lexem_Split_with_quotes2()
        {
            var values = Lexem.Split("aaa ; \"bbb ,' 'ccc\" ', ddd'", new char[] { ',', ';' }, new SymbolPair[] { new SymbolPair('{', '}') });
            Assert.AreEqual("aaa ", values[0]);
            Assert.AreEqual(" \"bbb ,' 'ccc\" ', ddd'", values[1]);
        }
    }
}
