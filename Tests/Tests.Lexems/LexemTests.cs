using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Lexems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Lexems
{
    [TestClass]
    public class LexemTests
    {
        [TestMethod]
        public void variable_names_validation_is_successfull()
        {
            Assert.IsTrue(LexemVariable.CheckVariableName("@hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("@_hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("@123_hello_my_dear_friend_123"));

            Assert.IsTrue(LexemVariable.CheckVariableName("hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("_hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("123_hello_my_dear_friend_123"));

            Assert.IsTrue(LexemVariable.CheckVariableName("$hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("$_hello_my_dear_friend_123"));
            Assert.IsTrue(LexemVariable.CheckVariableName("$123_hello_my_dear_friend_123"));

            Assert.IsTrue(LexemVariable.CheckVariableName("___"));
            Assert.IsTrue(LexemVariable.CheckVariableName("@_"));
            Assert.IsTrue(LexemVariable.CheckVariableName("$_"));


            Assert.IsFalse(LexemVariable.CheckVariableName("$"));
            Assert.IsFalse(LexemVariable.CheckVariableName("@"));

            Assert.IsFalse(LexemVariable.CheckVariableName("a@hello"));
            Assert.IsFalse(LexemVariable.CheckVariableName("_@_"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a$bbb"));
            Assert.IsFalse(LexemVariable.CheckVariableName("_$"));

            Assert.IsFalse(LexemVariable.CheckVariableName("!asdsa"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a.b"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a-b"));
            Assert.IsFalse(LexemVariable.CheckVariableName(""));
            Assert.IsFalse(LexemVariable.CheckVariableName("+a"));
            Assert.IsFalse(LexemVariable.CheckVariableName("~a"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a-"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a~"));
            Assert.IsFalse(LexemVariable.CheckVariableName("a+"));
        }
    }
}
