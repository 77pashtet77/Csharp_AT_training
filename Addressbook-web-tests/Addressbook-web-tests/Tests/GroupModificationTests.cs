using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebAddressbookTests
{
    [TestFixture]
    public class GroupModificationTests : TestBase
    {
        [Test]
        public void GroupModificationTest()
        {
            GroupData newData = new GroupData("Plumbers");
            newData.Header = null;
            newData.Footer = null;
            app.Groups.Modify(1, newData);
        }

        [Test]
        public void GroupModificationTestEmptyData()
        {
            GroupData newData = new GroupData("");
            newData.Header = "";
            newData.Footer = "";
            app.Groups.Modify(1, newData);
        }
    }
}
