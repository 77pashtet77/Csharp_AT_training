﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace addressbook_tests_autoit
{
    public class TestBase
    {
        public ApplicationManager app;
        
        [TestFixtureSetUp]
        public void InitApplication()
        {
            app = ApplicationManager.GetInstance();
        }
    }
}
