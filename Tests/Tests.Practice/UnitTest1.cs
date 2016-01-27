using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
    }
}
