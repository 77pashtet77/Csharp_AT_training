using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebAddressbookTests
{
    public class ContactModificationTests : TestBase
    {
        [Test]
        public void ContactModificationTest()
        {
            ContactData newData = new ContactData();
            newData.FirstName = "Vasiliy";
            newData.LastName = "Pupkin";
            app.Contacts.Modify(1, newData);
        }

        [Test]
        public void ContactModificationTestEmptyData()
        {
            ContactData newData = new ContactData();
            newData.FirstName = "";
            newData.LastName = "";
            app.Contacts.Modify(1, newData);
        }
    }
}
