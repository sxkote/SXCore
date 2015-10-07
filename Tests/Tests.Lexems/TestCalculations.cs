using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;

namespace Tests.Lexems
{
    [TestClass]
    public class TestCalculations
    {
        public SXEnvironment _environment;

        [TestInitialize]
        public void Init()
        {
            _environment = new SXEnvironment();
            _environment.Add("v1", 1);
            _environment.Add("v2", 2);
            _environment.Add("v3", 3);
            _environment.Add("vBegin", new DateTime(2015, 06, 23));
            _environment.Add("vEnd", new DateTime(2015, 06, 30));
            _environment.Add("vTrue", true);
            _environment.Add("vFalse", true);
        }

        [TestMethod]
        public void CalcSimple()
        {
            Assert.AreEqual(4, SXExpression.Calculate("2 + 2").Value);

            Assert.AreEqual(5, SXExpression.Calculate("\"2 + 2\".Length").Value);

            Assert.AreEqual(-12, SXExpression.Calculate("-9 + (-3)").Value);

            Assert.AreEqual(-1, SXExpression.Calculate("2 + ( -3)").Value);

            Assert.AreEqual(2, SXExpression.Calculate("2 > 4 ? 8 : 2").Value);

            Assert.AreEqual(6, SXExpression.Calculate("2 > 4 ? 8 : 2 + 4").Value);

            Assert.AreEqual(4, SXExpression.Calculate("2 > 4 ? 8 : 2 + (4 - 3)*2").Value);

            Assert.AreEqual(5, SXExpression.Calculate("2 + (2 > 1 ? 3 : 5)").Value);

            Assert.AreEqual(4, SXExpression.Calculate("2 > 4 ? (8 + (3 > 1 ? 7 : (-2))) : 2 + (3 > 1?4 - 3:0)*2").Value);

            Assert.AreEqual(2, SXExpression.Calculate("round(2.333;0)").Value);

            Assert.AreEqual(2016, SXExpression.Calculate("'23.06.2015'.AddMonths(12).Year").Value);

            Assert.AreEqual(new DateTime(2015, 6, 30), SXExpression.Calculate("'23.06.2015' + '7.0:0:0'").Value);

            Assert.AreEqual(1, SXExpression.Calculate("sin(pi()/2)"));
        }

        [TestMethod]
        public void CalcWithVars()
        {  
            Assert.AreEqual(11, SXExpression.Calculate("v2*2 > v3 ? v1 - (vTrue?-10:-20) : 10", _environment).Value);

            Assert.AreEqual(11, SXExpression.Calculate("a = 10 + v1", _environment).Value);

            Assert.AreEqual(4, SXExpression.Calculate("b = a > 10 ? v2*2 : 0", _environment).Value);
        }
    }
}
