using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;

namespace Tests.Lexems
{
    [TestClass]
    public class TestLexems
    {
        [TestMethod]
        public void ParseLexemKeywords()
        {
            var input = "class IF Int //";

            var k1 = Lexem.Parse(ref input) as LexemKeyword;
            Assert.IsNotNull(k1);
            Assert.AreEqual("class", k1.Text);

            var k2 = Lexem.Parse(ref input) as LexemKeyword;
            Assert.IsNotNull(k2);
            Assert.AreEqual("if", k2.Text);

            var k3 = Lexem.Parse(ref input) as LexemKeyword;
            Assert.IsNotNull(k3);
            Assert.AreEqual("int", k3.Text);

            var k4 = Lexem.Parse(ref input) as LexemKeyword;
            Assert.IsNotNull(k4);
            Assert.AreEqual("//", k4.Text);

            Assert.IsNull(input);
        }
    }
}
