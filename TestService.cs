using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCore
{
    public class TestService : ITestService
    {
        public string SayHello(string text)
        {
            return "Hello_" + text;
        }
    }
}
