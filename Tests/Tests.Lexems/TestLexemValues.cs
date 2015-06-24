using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;

namespace Tests.Lexems
{
    [TestClass]
    public class TestLexemValues
    {
        double delta = 0.000000001;

        [TestMethod]
        public void ParseValueBool()
        {
            string input = "true false unknown";

            var btrue = SXLexemValue.Parse(ref input) as SXLexemBool;
            Assert.IsNotNull(btrue);
            Assert.AreEqual(SXLexemBool.BoolType.True, btrue.Value);

            var bfalse = SXLexemValue.Parse(ref input) as SXLexemBool;
            Assert.IsNotNull(bfalse);
            Assert.AreEqual(SXLexemBool.BoolType.False, bfalse.Value);

            var bunk = SXLexemValue.Parse(ref input) as SXLexemBool;
            Assert.IsNotNull(bunk);
            Assert.AreEqual(SXLexemBool.BoolType.Unknown, bunk.Value);

            Assert.AreEqual(btrue, bfalse.Negative);

            Assert.AreEqual(true, bfalse || btrue);
            Assert.AreEqual(false, bfalse && btrue);
        }

        [TestMethod]
        public void ParseValueComplex()
        {
            string input = "[3;4] [-3.3;-1.01] [infinity]";

            var c1 = SXLexemValue.Parse(ref input) as SXLexemComplex;
            Assert.IsNotNull(c1);
            Assert.AreEqual(3, c1.Re);
            Assert.AreEqual(4, c1.Im);
            Assert.AreEqual(5, c1.Norm);
            Assert.IsFalse(c1.IsInfinity);

            var c2 = SXLexemValue.Parse(ref input) as SXLexemComplex;
            Assert.IsNotNull(c2);
            Assert.IsFalse(c2.IsInfinity);

            var cres = c1 + c2;
            Assert.AreEqual(-0.3, cres.Re, delta);
            Assert.AreEqual(2.99, cres.Im, delta);
            Assert.IsFalse(cres.IsInfinity);

            cres = c1 - c2;
            Assert.AreEqual(6.3, cres.Re, delta);
            Assert.AreEqual(5.01, cres.Im, delta);
            Assert.IsFalse(cres.IsInfinity);

            Assert.IsTrue(c1 > c2);

            var c3 = SXLexemValue.Parse(ref input) as SXLexemComplex;
            Assert.IsNotNull(c3);
            Assert.IsTrue(c3.IsInfinity);
            Assert.IsFalse(cres.IsInfinity);

            Assert.IsTrue((c1 + c3).IsInfinity);
            Assert.IsTrue((c1 - c3).IsInfinity);
            Assert.IsTrue(c1 <= c3);

            Assert.AreEqual(c1 + c3, c1 - c3);
            Assert.AreEqual(c1 + c2, c1 + c2);
            Assert.AreNotEqual(c1 + c2, c1 - c2);

            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueDate()
        {
            string input = "'17.11.1985' '23.06.2015 11:59' 'future' 'now' 'past'";

            var d1 = SXLexemValue.Parse(ref input) as SXLexemDate;
            Assert.IsNotNull(d1);
            Assert.AreEqual(SXLexemDate.TenseType.Present, d1.Tense);
            Assert.AreEqual(new DateTime(1985, 11, 17), d1.Value);

            var d2 = SXLexemValue.Parse(ref input) as SXLexemDate;
            Assert.IsNotNull(d2);
            Assert.AreEqual(SXLexemDate.TenseType.Present, d2.Tense);
            Assert.AreEqual(new DateTime(2015, 06, 23, 11, 59, 0), d2.Value);

            Assert.IsTrue(d2 > d1);
            Assert.AreNotEqual(d1, d2);

            var dfuture = SXLexemValue.Parse(ref input) as SXLexemDate;
            Assert.IsNotNull(dfuture);
            Assert.AreEqual(SXLexemDate.TenseType.Future, dfuture.Tense);

            var dnow = SXLexemValue.Parse(ref input) as SXLexemDate;
            Assert.IsNotNull(dnow);
            Assert.AreEqual(SXLexemDate.TenseType.Present, dnow.Tense);
            
            var dpast = SXLexemValue.Parse(ref input) as SXLexemDate;
            Assert.IsNotNull(dpast);
            Assert.AreEqual(SXLexemDate.TenseType.Past, dpast.Tense);

            Assert.IsTrue(dnow > d1);
            Assert.IsTrue(dnow >= d2);
            Assert.IsTrue(dnow < dfuture);
            Assert.IsTrue(dnow > dpast);
            Assert.IsTrue(new SXLexemDate(DateTime.Now) >= dnow);

            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueNumber()
        {
            string input = "310 3.141592 infinity  99.2e-1";

            var n1 = SXLexemValue.Parse(ref input) as SXLexemNumber;
            Assert.IsNotNull(n1);
            Assert.AreEqual(310d, n1.Value, delta);
            Assert.IsFalse(n1.IsInfinity);

            var n2 = SXLexemValue.Parse(ref input) as SXLexemNumber;
            Assert.IsNotNull(n2);
            Assert.IsFalse(n2.IsInfinity);

            var cres = n1 + n2;
            Assert.AreEqual(313.141592d, cres.Value, delta);
            Assert.IsFalse(cres.IsInfinity);

            cres = n1 - n2;
            Assert.AreEqual(306.858408d, cres.Value, delta);
            Assert.IsFalse(cres.IsInfinity);

            Assert.IsTrue(n1 > n2);

            var n3 = SXLexemValue.Parse(ref input) as SXLexemNumber;
            Assert.IsNotNull(n3);
            Assert.IsTrue(n3.IsInfinity);

            Assert.IsTrue((n1 + n3).IsInfinity);
            Assert.IsTrue((n1 - n3).IsInfinity);
            Assert.IsTrue(n1 <= n3);

            Assert.AreEqual(n1 + n2, n1 + n2);
            Assert.AreNotEqual(n1 + n2, n1 - n2);

            var n4 = SXLexemValue.Parse(ref input) as SXLexemNumber;
            Assert.IsNotNull(n4);
            Assert.IsFalse(n4.IsInfinity);
            Assert.IsTrue(n4 < 10);


            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueNumbrList()
        {
            string input = "0,22293e+10 99.2e-10 99.2e-1 33.22 292,2 202, .9090 0 9 ";

            while (!String.IsNullOrEmpty(input))
            {
                var number = SXLexemValue.Parse(ref input) as SXLexemNumber;
                Assert.IsNotNull(number);
                Assert.IsFalse(number.IsInfinity);
                Assert.IsTrue(number >= 0);
            }
        }

        [TestMethod]
        public void ParseValueSpan()
        {
            string input = "'1;23;59;59' '0.1:0:0'";

            //var t = new TimeSpan(1, 23, 59, 59).ToString("hh");

            var s1 = SXLexemValue.Parse(ref input) as SXLexemSpan;
            Assert.IsNotNull(s1);
            Assert.IsTrue(s1.Value == new TimeSpan(1, 23, 59, 59));

            var s2 = SXLexemValue.Parse(ref input) as SXLexemSpan;
            Assert.IsNotNull(s2);
            Assert.AreEqual(new TimeSpan(1, 0, 0), s2.Value);

            Assert.IsTrue(s1 > s2);

            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueStruct()
        {
            string input = "{'a':22.3,'m':{'c':'bla-bla[]{}'}}";

            var s1 = SXLexemValue.Parse(ref input) as SXLexemStruct;
            Assert.IsNotNull(s1);
            Assert.IsTrue(s1.Members.Count == 2);

            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueText()
        {
            string input = "\"asda asdkjsa &amp;&quote; ( P { } -23\" \"a\" \"b\"";

            var s1 = SXLexemValue.Parse(ref input) as SXLexemText;
            Assert.IsNotNull(s1);
            Assert.AreEqual("asda asdkjsa &\" ( P { } -23", s1.Value);

            var sa = SXLexemValue.Parse(ref input) as SXLexemText;
            Assert.IsNotNull(sa);
            Assert.AreEqual("a", sa.Value);

            var sb = SXLexemValue.Parse(ref input) as SXLexemText;
            Assert.IsNotNull(sb);
            Assert.AreEqual("b", sb.Value);

            Assert.AreEqual("ab", (sa + sb).Value);
            Assert.AreNotEqual(sa, sb);

            Assert.IsNull(input);
        }

        [TestMethod]
        public void ParseValueVoid()
        {
            string input = "null";

            var v1 = SXLexemValue.Parse(ref input) as SXLexemVoid;
            Assert.IsNotNull(v1);
            Assert.AreEqual(null,v1.Value);

            Assert.IsNull(input);
        }
    }
}
