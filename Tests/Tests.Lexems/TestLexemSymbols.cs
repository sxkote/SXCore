using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;

namespace Tests.Lexems
{
    [TestClass]
    public class TestLexemSymbols
    {
        [TestMethod]
        public void ParseSymbolBracket()
        {
            foreach (var pair in SXLexemBracket.DefaultBrackets)
            {
                string input = pair.Open.ToString() + pair.Close.ToString();

                var bracket1 = SXLexemSymbol.Parse(ref input) as SXLexemBracket;
                var bracket2 = SXLexemSymbol.Parse(ref input) as SXLexemBracket;

                Assert.IsNotNull(bracket1);
                Assert.IsNotNull(bracket2);

                Assert.AreEqual(pair.Open.ToString(), bracket1.Text);
                Assert.AreEqual(pair.Close.ToString(), bracket2.Text);

                Assert.IsNull(bracket1.Simetric);
                Assert.IsNull(bracket2.Simetric);

                bracket1.Simetric = bracket2;

                Assert.AreEqual(bracket2, bracket1.Simetric);
                Assert.AreEqual(bracket1, bracket2.Simetric);

                Assert.IsTrue(String.IsNullOrEmpty(input));
            }
        }

        [TestMethod]
        public void ParseSymbolComma()
        {
            foreach (var com in SXLexemComma.Commas)
            {
                string input = com;

                var symbol = SXLexemSymbol.Parse(ref input) as SXLexemComma;

                Assert.IsNotNull(symbol);
                Assert.IsTrue(symbol is SXLexemComma);
                Assert.AreEqual(com, symbol.Text);

                Assert.IsTrue(String.IsNullOrEmpty(input));
            }
        }

        [TestMethod]
        public void ParseSymbolOperator()
        {
            foreach (var op in SXLexemOperator.Operators)
            {
                string input = op;

                var symbol = SXLexemSymbol.Parse(ref input) as SXLexemOperator;

                Assert.IsNotNull(symbol);
                Assert.AreEqual(op, symbol.Text);

                Assert.IsTrue(String.IsNullOrEmpty(input));
            }
        }

        [TestMethod]
        public void ParseSymbolSwitch()
        {
            string input = "?:";

            var question = SXLexemSymbol.Parse(ref input) as SXLexemSwitch;
            var answer = SXLexemSymbol.Parse(ref input) as SXLexemSwitch;

            Assert.IsNotNull(question);
            Assert.IsNotNull(answer);

            Assert.AreEqual("?", question.Text);
            Assert.AreEqual(":", answer.Text);

            Assert.IsNull(question.Simetric);
            Assert.IsNull(answer.Simetric);

            question.Simetric = answer;

            Assert.AreEqual(answer, question.Simetric);
            Assert.AreEqual(question, answer.Simetric);

            Assert.IsTrue(String.IsNullOrEmpty(input));
        }
    }
}
