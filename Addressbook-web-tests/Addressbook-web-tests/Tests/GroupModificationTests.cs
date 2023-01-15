﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WebAddressbookTests
{
    [TestFixture]
    public class GroupModificationTests : AuthTestBase
    {
        [Test]
        public void GroupModificationTest()
        {
            GroupData newData = new GroupData("Plumbers");
            newData.Header = null;
            newData.Footer = null;

            app.Groups.CreateGroupIfNoneExists();

            List<GroupData> oldGroups = app.Groups.GetGroupsList();

            app.Groups.Modify(0, newData);

            List<GroupData> newGroups = app.Groups.GetGroupsList();
            oldGroups[0].Name = newData.Name;
            oldGroups.Sort();
            newGroups.Sort();
            Assert.AreEqual(oldGroups, newGroups);
        }

        [Test]
        public void GroupModificationTestEmptyData()
        {
            GroupData newData = new GroupData("");
            newData.Header = "";
            newData.Footer = "";
            app.Groups.CreateGroupIfNoneExists();

            List<GroupData> oldGroups = app.Groups.GetGroupsList();

            app.Groups.Modify(0, newData);

            List<GroupData> newGroups = app.Groups.GetGroupsList();
            oldGroups[0].Name = newData.Name;
            oldGroups.Sort();
            newGroups.Sort();
            Assert.AreEqual(oldGroups, newGroups);
        }
    }
}
