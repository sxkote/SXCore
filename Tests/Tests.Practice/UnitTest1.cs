using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Tests.Practice
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string pattern = @"(?<sign>[\-\+]?)(?<integer>\d+)(?<fractional>[\.\,]\d*)?(?<exponent>[eE](?<expsign>[\-\+])?(?<expnum>\d+)?)?";
            Match match = Regex.Match("-2", pattern);
            if (match.Success)
            {
                Trace.WriteLine(match.Groups["sign"].Value);

                Debug.WriteLine(match.Groups["integer"].Value);

                Trace.WriteLine(match.Groups["fractional"].Value);
                Trace.WriteLine(match.Groups["fractional"].Success);

                Trace.WriteLine(match.Groups["exponent"].Value);
                Trace.WriteLine(match.Groups["exponentjj"].Success);

                Trace.WriteLine(match.Groups["expsign"].Value);
                Trace.WriteLine(match.Groups["expnum"].Value);
            }

        }


        [Flags]
        public enum ConturType
        {
            None = 0,
            Circle = 1,
            Box = 2,
            Apple = 4
        }

        [TestMethod]
        public void Test_enum_flags_operations()
        {
            ConturType t = ConturType.None;
            var st1 = JsonConvert.SerializeObject(new { MyT = t });
            Debug.WriteLine(((int)t).ToString() + " = " + st1);

            t = ConturType.Apple | ConturType.Circle;
            var st2 = JsonConvert.SerializeObject(new { MyT = t });
            Debug.WriteLine(((int)t).ToString() + " = " + st2);

            if (!Enum.TryParse<ConturType>("8", out t))
                Debug.WriteLine("ERROR");
           
            var isDefined = Enum.IsDefined(typeof(ConturType), t);
            var nam1 = Enum.GetName(typeof(ConturType), 3);
            var isCorrect = Enum.GetValues(typeof(ConturType)).Cast<ConturType>().Where(v => v != 0).Any(v => (t & v) != 0);

            Action<ConturType, ConturType> act = (v, type) =>
            {
                if ((v & type) != 0)
                    Debug.WriteLine("Contains: " + type.ToString());
            };

            act(t, ConturType.None);
            act(t, ConturType.Circle);
            act(t, ConturType.Box);
            act(t, ConturType.Apple);
        }
    }
}
