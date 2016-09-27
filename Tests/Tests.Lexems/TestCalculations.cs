using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tests.Lexems
{
    [TestClass]
    public class TestCalculations
    {
        public LexemEnvironment _environment;

        [TestInitialize]
        public void Init()
        {
            _environment = new LexemEnvironment();
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
            Assert.AreEqual(4, LexemExpression.Calculate("2 + 2").Value);

            Assert.AreEqual(5, LexemExpression.Calculate("\"2 + 2\".Length").Value);

            Assert.AreEqual(-12, LexemExpression.Calculate("-9 + (-3)").Value);

            Assert.AreEqual(-1, LexemExpression.Calculate("2 + ( -3)").Value);

            Assert.AreEqual(2, LexemExpression.Calculate("2 > 4 ? 8 : 2").Value);

            Assert.AreEqual(6, LexemExpression.Calculate("2 > 4 ? 8 : 2 + 4").Value);

            Assert.AreEqual(4, LexemExpression.Calculate("2 > 4 ? 8 : 2 + (4 - 3)*2").Value);

            Assert.AreEqual(5, LexemExpression.Calculate("2 + (2 > 1 ? 3 : 5)").Value);

            Assert.AreEqual(4, LexemExpression.Calculate("2 > 4 ? (8 + (3 > 1 ? 7 : (-2))) : 2 + (3 > 1?4 - 3:0)*2").Value);

            Assert.AreEqual(2, LexemExpression.Calculate("round(2.333;0)").Value);

            Assert.AreEqual(2016, LexemExpression.Calculate("'23.06.2015'.AddMonths(12).Year").Value);

            Assert.AreEqual(new DateTime(2015, 6, 30), LexemExpression.Calculate("'23.06.2015' + '7.0:0:0'").Value);

            Assert.AreEqual(1, LexemExpression.Calculate("sin(pi()/2)"));

            Assert.AreEqual(1, LexemExpression.Calculate("sin  (pi  (   )/2)"));
        }

        [TestMethod]
        public void CalcWithVars()
        {  
            Assert.AreEqual(11, LexemExpression.Calculate("v2*2 > v3 ? v1 - (vTrue?-10:-20) : 10", _environment).Value);

            Assert.AreEqual(11, LexemExpression.Calculate("a = 10 + v1", _environment).Value);

            Assert.AreEqual(4, LexemExpression.Calculate("b = a > 10 ? v2*2 : 0", _environment).Value);

            Assert.AreEqual(4, _environment["b"].Value);
        }

        [TestMethod]
        public void CalcExceptions()
        {
            LexemEnvironment env = new LexemEnvironment();
            env.Add("a", 10);
            env.Add("b", 7);
            env.Add("c", 2);

            //Func<LexemValue, Lexem, LexemVariable> exec = (v, l) => 
            //{
            //    if (l is LexemFunction && ((LexemFunction)l).Name.Equals("myfunc", StringComparison.InvariantCultureIgnoreCase))
            //        return 100;
            //    if (l is LexemVariable && ((LexemVariable)l).Name.Equals("myprop", StringComparison.InvariantCultureIgnoreCase))
            //        return -100;
            //    return null;
            //};

            //env.OnLexemExecution = new LexemEnvironment.OnLexemExecutionDelegate(exec);

            var inputs = new List<string>();
            inputs.Add("(10-b))*a+(c-3)");
            inputs.Add("(10-b) *a e+(c-3)");
            inputs.Add("(10-b)*a+(c-3),");
            inputs.Add("(10-b)*a+(c.myfunc()-3)");
            inputs.Add("(10-b)*a+(c.myprop-3)");

            foreach (var input in inputs)
            {
                try
                {
                    var exp = new LexemExpression(input);
                    var res = exp.Calculate(env);
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception($"Null Argument Exception should NOT be generated on {input}!");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    continue;
                }

                throw new Exception($"Exception should be generated on {input}!");
            }
        }
    }
}
