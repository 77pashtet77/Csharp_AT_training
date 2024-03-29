﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace WebAddressbookTests
{
    public class NavigationHelper : HelperBase
    {
        private string baseURL;

        public NavigationHelper(ApplicationManager manager, string baseURL) 
            : base(manager) 
        {
            this.baseURL = baseURL;
        }

        public NavigationHelper OpenHomePage()
        {
            if (driver.Url == baseURL + "addressbook/")
            {
                return this;
            }

            driver.Navigate().GoToUrl(baseURL + "addressbook/");
            return this;
        }

        public NavigationHelper GoToContactsPage()
        {
            if (driver.Url == baseURL + "addressbook/")
            {
                return this;
            }

            driver.FindElement(By.LinkText("home")).Click();
            return this;
        }

        public NavigationHelper GoToGroupsPage()
        {
            if (driver.Url == baseURL + "addressbook/group.php" && IsElementPresent(By.Name("new")))
            {
                return this;
            }
            driver.FindElement(By.LinkText("groups")).Click();
            return this;
        }

        public NavigationHelper InitContactCreation()
        {
            if (driver.Url == baseURL + "addressbook/edit.php")
            {
                return this;
            }

            driver.FindElement(By.LinkText("add new")).Click();
            return this;
        }
    }
}
