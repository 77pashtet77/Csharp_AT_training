﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;

namespace WebAddressbookTests
{
    public class RemovingContactFromGroupTests : AuthTestBase
    {
        [Test]
        public void RemovingContactFromGroupTest()
        {
            int index = 0;
            app.Groups.CreateGroupIfNoneExists();
            app.Contacts.CreateContactIfNoneExists();

            GroupData group = GroupData.GetAll()[index];

            if (group.GetContacts().Count == 0)
            {
                app.Contacts.AddContactToGroupIfGroupHasNone(group, ContactData.GetAll()[index]);
            }

            List<ContactData> oldList = group.GetContacts();
            ContactData toRemove = oldList[index];

            app.Contacts.RemoveContactFromGroup(toRemove, group);

            List<ContactData> newList = group.GetContacts();

            oldList.RemoveAt(index);
            Assert.AreEqual(oldList, newList);

            foreach (ContactData contact in newList)
            {
                Assert.AreNotEqual(contact.Id, toRemove.Id);
            }
        }
    }
}
