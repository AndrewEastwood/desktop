using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject_PayDesk_IKC
{
    [TestClass]
    public class UnitTest_LoginForm
    {
        [TestMethod]
        public void TestMethod1()
        {
            mdcore.AppFunc.Authorize("test1", "test2");
            mdcore.AppFunc.Authorize("12324", "1234");
            mdcore.AppFunc.Authorize("", "");
        }
    }
}
