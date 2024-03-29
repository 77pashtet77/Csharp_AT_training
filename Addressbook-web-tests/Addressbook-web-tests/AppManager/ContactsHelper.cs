﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace WebAddressbookTests
{
    public class ContactsHelper : HelperBase
    {
        public ContactsHelper(ApplicationManager manager) : base(manager) { }

        public ContactsHelper Create(ContactData contact)
        {
            manager.Navigator.GoToContactsPage();
            manager.Navigator.InitContactCreation();
            FillContactForm(contact);
            SubmitNewContact();
            return this;
        }

        public ContactsHelper Delete(int v, bool alertAccept)
        {
            manager.Navigator.GoToContactsPage();
            SelectContact(v);
            DeleteContact();
            AlertAccept(alertAccept);
            return this;
        }

        public ContactsHelper Delete(ContactData contact, bool alertAccept)
        {
            manager.Navigator.GoToContactsPage();
            SelectContact(contact.Id);
            DeleteContact();
            AlertAccept(alertAccept);
            return this;
        }

        public ContactsHelper DeleteContactThroughEdit(int v)
        {
            manager.Navigator.GoToContactsPage();
            InitContactEdit(v);
            driver.FindElement(By.XPath("//input[@value='Delete']")).Click();
            contactListCache = null;
            contactInfoFromEditFormCache = null;
            return this;
        }

        public ContactsHelper DeleteContactThroughEdit(ContactData contact)
        {
            manager.Navigator.GoToContactsPage();
            InitContactEdit(contact.Id);
            driver.FindElement(By.XPath("//input[@value='Delete']")).Click();
            contactListCache = null;
            contactInfoFromEditFormCache = null;
            return this;
        }

        public ContactsHelper Modify(int v, ContactData contact)
        {
            manager.Navigator.GoToContactsPage();
            InitContactEdit(v);
            FillContactForm(contact);
            UpdateContact();
            return this;
        }

        public ContactsHelper Modify(ContactData contactOld, ContactData contactNew)
        {
            manager.Navigator.GoToContactsPage();
            InitContactEdit(contactOld.Id);
            FillContactForm(contactNew);
            UpdateContact();
            return this;
        }

        public ContactsHelper AddContactToGroup(ContactData contact, GroupData group)
        {
            manager.Navigator.GoToContactsPage();
            ClearGroupFilter();
            SelectContact(contact.Id);
            SelectGroupToAddTo(group.Name);
            CommitAddingContactToGroup();
            new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                .Until(d => d.FindElements(By.CssSelector("div.msgbox")).Count > 0);
            return this;
        }

        public ContactsHelper RemoveContactFromGroup(ContactData contact, GroupData group)
        {
            manager.Navigator.GoToContactsPage();
            SetGroupFilter(group.Id);
            AddContactToGroupIfGroupHasNone(group, contact);
            SelectContact(contact.Id);
            InitRemoveContactFromGroup();
            new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                .Until(d => d.FindElements(By.CssSelector("div.msgbox")).Count > 0);
            return this;
        }

        //Check if group has contacts, then choosing which contact to add
        public ContactsHelper AddContactToGroupIfGroupHasNone(GroupData group, ContactData contact)
        {
            manager.Navigator.GoToContactsPage();
            SetGroupFilter(group.Id);
            if (!IsElementPresent(By.Name("entry")))
            {
                AddContactToGroup(contact, group);
                return this;
            }
            return this;
        }

        public ContactsHelper InitRemoveContactFromGroup()
        {
            driver.FindElement(By.Name("remove")).Click();
            return this;
        }

        public ContactsHelper SetGroupFilter(string id)
        {
            new SelectElement(driver.FindElement(By.Name("group"))).SelectByValue(id);
            return this;
        }

        public ContactsHelper SelectContact(int index)
        {
            driver.FindElement(By.XPath("//table//tr[" + (index + 2) + "]/td[1]/input")).Click();
            return this;
        }

        public ContactsHelper SelectContact(string id)
        {
            driver.FindElement(By.Id(id)).Click();
            return this;
        }

        public ContactsHelper InitContactEdit(int index)
        {
            driver.FindElement(By.XPath("//table//tr[" + (index + 2) + "]/td[8]/a/img")).Click();
            return this;
        }

        public ContactsHelper InitContactEdit(string id)
        {
            driver.FindElement(By.XPath("//input[@name='selected[]' and @value='"+id+"']/../../td[8]/a/img")).Click();
            return this;
        }

        public ContactsHelper OpenContactDetails(int index)
        {
            driver.FindElement(By.XPath("//table//tr[" + (index + 2) + "]/td[7]/a/img")).Click();
            return this;
        }

        public ContactsHelper OpenContactDetails(string id)
        {
            driver.FindElement(By.XPath("//input[@name='selected[]' and @value='" + id + "']/../../td[7]/a/img")).Click();
            return this;
        }

        public string GetContactDetails(int index)
        {
            OpenContactDetails(index);
            return driver.FindElement(By.Id("content")).Text;
        }


        public string GetContactDetails(ContactData contact)
        {
            OpenContactDetails(contact.Id);
            return driver.FindElement(By.Id("content")).Text;
        }

        public ContactsHelper UpdateContact()
        {
            driver.FindElement(By.Name("update")).Click();
            contactListCache = null;
            contactInfoFromEditFormCache = null;
            return this;
        }

        public ContactsHelper DeleteContact()
        {
            driver.FindElement(By.XPath("//input[@value='Delete']")).Click();
            contactListCache = null;
            contactInfoFromEditFormCache = null;
            return this;
        }

        public ContactsHelper SubmitNewContact()
        {
            driver.FindElement(By.Name("submit")).Click();
            contactListCache = null;
            contactInfoFromEditFormCache = null;
            return this;
        }

        public ContactsHelper FillContactForm(ContactData contact)
        {
            Type(By.Name("firstname"), contact.FirstName);
            Type(By.Name("lastname"), contact.LastName);
            //Add additional fields for full contact data filling
            return this;
        }

        public ContactsHelper CommitAddingContactToGroup()
        {
            driver.FindElement(By.Name("add")).Click();
            return this;
        }

        public ContactsHelper SelectGroupToAddTo(string name)
        {
            new SelectElement(driver.FindElement(By.Name("to_group"))).SelectByText(name);
            return this;
        }

        public ContactsHelper ClearGroupFilter()
        {
            new SelectElement(driver.FindElement(By.Name("group"))).SelectByText("[all]");
            return this;
        }

        public bool IsContactPresent()
        {
            return IsElementPresent(By.Name("entry"));
        }

        public ContactsHelper CreateContactIfNoneExists()
        {
            manager.Navigator.GoToContactsPage();

            if (!IsContactPresent())
            {
                ContactData contact = new ContactData("Good", "Doggie");
                Create(contact);
                return this;
            }
            return this;
        }

        public ContactsHelper AlertAccept(bool alertAccept)
        {
            if (alertAccept == true)
            {
                driver.SwitchTo().Alert().Accept();
            }
            else
            {
                driver.SwitchTo().Alert().Dismiss();

            }
            return this;
        }

        private List<ContactData> contactListCache = null;

        public List<ContactData> GetContactsList()
        {
            if (contactListCache == null)
            {
                contactListCache = new List<ContactData>();

                manager.Navigator.GoToContactsPage();

                ICollection<IWebElement> elements = driver.FindElements(By.Name("entry"));
                foreach (IWebElement element in elements)
                {
                    contactListCache.Add(new ContactData(element.FindElement(By.XPath(".//td[2]")).Text, element.FindElement(By.XPath(".//td[3]")).Text)
                    {
                        //adding Id property for each found element
                        Id = element.FindElement(By.TagName("input")).GetAttribute("id")
                    });
                }
            }

            return new List<ContactData>(contactListCache);
        }

        //Hashing
        //Remove code below for stable tests
        public int GetContactsCount()
        {
            manager.Navigator.GoToContactsPage();
            return driver.FindElements(By.Name("entry")).Count;
        }
        //Remove

        public ContactData GetContactInformationFromTable(int index)
        {
            manager.Navigator.GoToContactsPage();

            IList<IWebElement> cells = driver.FindElements(By.Name("entry"))[index]
                .FindElements(By.TagName("td"));

            string firstName = cells[2].Text;
            string lastName = cells[1].Text;
            string address = cells[3].Text;
            string allPhones = cells[5].Text;
            string allEmails = cells[4].Text;

            //returning data in a format used on the main page
            return new ContactData(lastName, firstName)
            {
                Address = address,
                AllPhones = allPhones,
                AllEmails = allEmails
            };
        }

        public ContactData GetContactInformationFromTable(ContactData contact)
        {
            manager.Navigator.GoToContactsPage();

            IList<IWebElement> cells = driver.FindElement(By.XPath("//input[@name='selected[]' and @value='" + Convert.ToInt32(contact.Id) + "']/../.."))
                .FindElements(By.TagName("td"));

            string firstName = cells[2].Text;
            string lastName = cells[1].Text;
            string address = cells[3].Text;
            string allPhones = cells[5].Text;
            string allEmails = cells[4].Text;

            //returning data in a format used on the main page
            return new ContactData(lastName, firstName)
            {
                Address = address,
                AllPhones = allPhones,
                AllEmails = allEmails
            };
        }

        private ContactData contactInfoFromEditFormCache = null;

        public ContactData GetContactInformationFromForm(int index)
        {
            if (contactInfoFromEditFormCache == null)
            {
                manager.Navigator.GoToContactsPage();
                InitContactEdit(index);
                string firstName = driver.FindElement(By.Name("firstname")).GetAttribute("value");
                string lastName = driver.FindElement(By.Name("lastname")).GetAttribute("value");
                string middleName = driver.FindElement(By.Name("middlename")).GetAttribute("value");
                string nickname = driver.FindElement(By.Name("nickname")).GetAttribute("value");

                string address = driver.FindElement(By.Name("address")).GetAttribute("value");
                string company = driver.FindElement(By.Name("company")).GetAttribute("value");
                string title = driver.FindElement(By.Name("title")).GetAttribute("value");

                string homePhone = driver.FindElement(By.Name("home")).GetAttribute("value");
                string mobilePhone = driver.FindElement(By.Name("mobile")).GetAttribute("value");
                string workPhone = driver.FindElement(By.Name("work")).GetAttribute("value");
                string faxPhone = driver.FindElement(By.Name("fax")).GetAttribute("value");

                string email1 = driver.FindElement(By.Name("email")).GetAttribute("value");
                string email2 = driver.FindElement(By.Name("email2")).GetAttribute("value");
                string email3 = driver.FindElement(By.Name("email3")).GetAttribute("value");

                //making the no-choise dropdown selection an empty string to match other fields empty state
                string birthdayDay = driver.FindElement(By.XPath("//select[@name='bday']/option[1]")).Text;
                string birthdayMonth = driver.FindElement(By.XPath("//select[@name='bmonth']/option[1]")).Text;
                string birthdayYear = driver.FindElement(By.Name("byear")).GetAttribute("value");
                if (birthdayDay == "-")
                {
                    birthdayDay = "";
                }
                if (birthdayMonth == "-")
                {
                    birthdayMonth = "";
                }

                string anniversaryDay = driver.FindElement(By.XPath("//select[@name='aday']/option[1]")).Text;
                string anniversaryMonth = driver.FindElement(By.XPath("//select[@name='amonth']/option[1]")).Text;
                string anniversaryYear = driver.FindElement(By.Name("ayear")).GetAttribute("value");
                if (anniversaryDay == "-")
                {
                    anniversaryDay = "";
                }
                if (anniversaryMonth == "-")
                {
                    anniversaryMonth = "";
                }

                string secondAddress = driver.FindElement(By.Name("address2")).GetAttribute("value");
                string secondHomePhone = driver.FindElement(By.Name("phone2")).GetAttribute("value");
                string notes = driver.FindElement(By.Name("notes")).GetAttribute("value");

                contactInfoFromEditFormCache = new ContactData(lastName, firstName)
                {
                    MiddleName = middleName,
                    Nickname = nickname,
                    Address = address,
                    Company = company,
                    Title = title,
                    HomePhone = homePhone,
                    MobilePhone = mobilePhone,
                    WorkPhone = workPhone,
                    FaxPhone = faxPhone,
                    Email1 = email1,
                    Email2 = email2,
                    Email3 = email3,
                    BirthdayDay = birthdayDay,
                    BirthdayMonth = birthdayMonth,
                    BirthdayYear = birthdayYear,
                    AnniversaryDay = anniversaryDay,
                    AnniversaryMonth = anniversaryMonth,
                    AnniversaryYear = anniversaryYear,
                    SecondAddress = secondAddress,
                    SecondHomePhone = secondHomePhone,
                    Notes = notes
                };
            }
            return contactInfoFromEditFormCache;
        }

        public ContactData GetContactInformationFromDB(int index)
        {
            contactInfoFromEditFormCache = ContactData.GetAll().ElementAt(index);
            if (contactInfoFromEditFormCache.BirthdayDay == "0")
            {
                contactInfoFromEditFormCache.BirthdayDay = "";
            }
            if (contactInfoFromEditFormCache.BirthdayMonth == "-")
            {
                contactInfoFromEditFormCache.BirthdayMonth = "";
            }
            if (contactInfoFromEditFormCache.AnniversaryDay == "0")
            {
                contactInfoFromEditFormCache.AnniversaryDay = "";
            }
            if (contactInfoFromEditFormCache.AnniversaryMonth == "-")
            {
                contactInfoFromEditFormCache.AnniversaryMonth = "";
            }
            return contactInfoFromEditFormCache;
        }

        public string GetFormattedDetailsFromEditForm(int index)
        {
            ContactData infoFromForm = GetContactInformationFromDB(index);
            string currentAge = (GetCurrentAgeFromForm(index)).ToString();
            string anniversaryAge = (GetYearsTillAnniversaryFromForm(index)).ToString();

            //Divide all the data in several blocks, the same way it is done in details page
            //if-checks can be refactored into custom method, accepting 3 string arguments which concat a string as a result
            /*string FormatContactField(string 1, string 2, string 3)
            if (2 != "")
            {
                2 = 1 + 2 + 3;
            }
            */
            //where 2 stands for info from form field

            //first block
            if (infoFromForm.FirstName != "")
            {
                infoFromForm.FirstName = infoFromForm.FirstName + " ";
            }
            if (infoFromForm.MiddleName != "")
            {
                infoFromForm.MiddleName = infoFromForm.MiddleName + " ";
            }
            if (infoFromForm.LastName != "")
            {
                infoFromForm.LastName = infoFromForm.LastName + "\r\n";
            }
            //if last name isnt filled - trim excessive whitespaces and add string break
            string nameLine = infoFromForm.FirstName + infoFromForm.MiddleName + infoFromForm.LastName;
            if (nameLine != "" && infoFromForm.LastName == "")
            {
                nameLine = nameLine.Trim() + "\r\n";
            }
            if (infoFromForm.Nickname != "")
            {
                infoFromForm.Nickname = infoFromForm.Nickname + "\r\n";
            }
            if (infoFromForm.Title != "")
            {
                infoFromForm.Title = infoFromForm.Title + "\r\n";
            }
            if (infoFromForm.Company != "")
            {
                infoFromForm.Company = infoFromForm.Company + "\r\n";
            }
            if (infoFromForm.Address != "")
            {
                infoFromForm.Address = infoFromForm.Address + "\r\n";
            }
            string firstBlock = nameLine + infoFromForm.Nickname + infoFromForm.Title + infoFromForm.Company + infoFromForm.Address;
            //checking if a block exists
            if (firstBlock != "")
            {
                firstBlock += "\r\n";
            }

            //second block
            if (infoFromForm.HomePhone != "")
            {
                infoFromForm.HomePhone = "H: " + infoFromForm.HomePhone + "\r\n";
            }
            if (infoFromForm.MobilePhone != "")
            {
                infoFromForm.MobilePhone = "M: " + infoFromForm.MobilePhone + "\r\n";
            }
            if (infoFromForm.WorkPhone != "")
            {
                infoFromForm.WorkPhone = "W: " + infoFromForm.WorkPhone + "\r\n";
            }
            if (infoFromForm.FaxPhone != "")
            {
                infoFromForm.FaxPhone = "F: " + infoFromForm.FaxPhone + "\r\n";
            }
            string secondBlock = infoFromForm.HomePhone + infoFromForm.MobilePhone + infoFromForm.WorkPhone + infoFromForm.FaxPhone;
            if (secondBlock != "")
            {
                secondBlock += "\r\n";
            }

            //third block
            if (infoFromForm.Email1 != "")
            {
                infoFromForm.Email1 = infoFromForm.Email1 + "\r\n";
            }
            if (infoFromForm.Email2 != "")
            {
                infoFromForm.Email2 = infoFromForm.Email2 + "\r\n";
            }
            if (infoFromForm.Email3 != "")
            {
                infoFromForm.Email3 = infoFromForm.Email3 + "\r\n";
            }
            string thirdBlock = infoFromForm.Email1 + infoFromForm.Email2 + infoFromForm.Email3;
            if (thirdBlock != "")
            {
                thirdBlock += "\r\n";
            }


            //fourth block
            if (infoFromForm.BirthdayDay != "")
            {
                infoFromForm.BirthdayDay = "Birthday " + infoFromForm.BirthdayDay + ". ";
            }
            if (infoFromForm.BirthdayMonth != "")
            {
                infoFromForm.BirthdayMonth = infoFromForm.BirthdayMonth + " ";
            }
            if (infoFromForm.BirthdayYear != "")
            {
                infoFromForm.BirthdayYear = infoFromForm.BirthdayYear;
            }
            if (currentAge != "0")
            {
                currentAge = " (" + currentAge + ")\r\n";
            }
            else
            {
                currentAge = "\r\n";
            }
            string birthdayLine = infoFromForm.BirthdayDay + infoFromForm.BirthdayMonth + infoFromForm.BirthdayYear + currentAge;
            //removing string break if no line exists
            if (birthdayLine == "\r\n")
            {
                birthdayLine = "";
            }
            if (infoFromForm.AnniversaryDay != "")
            {
                infoFromForm.AnniversaryDay = "Anniversary " + infoFromForm.AnniversaryDay + ". ";
            }
            if (infoFromForm.AnniversaryMonth != "")
            {
                infoFromForm.AnniversaryMonth = infoFromForm.AnniversaryMonth + " ";
            }
            if (infoFromForm.AnniversaryYear != "")
            {
                infoFromForm.AnniversaryYear = infoFromForm.AnniversaryYear;
            }
            if (anniversaryAge != "0")
            {
                anniversaryAge = " (" + anniversaryAge + ")\r\n";
            }
            else
            {
                anniversaryAge = "\r\n";
            }
            string anniversaryLine = infoFromForm.AnniversaryDay + infoFromForm.AnniversaryMonth + infoFromForm.AnniversaryYear + anniversaryAge;
            if (anniversaryLine == "\r\n")
            {
                anniversaryLine = "";
            }

            string fourthBlock = birthdayLine + anniversaryLine + "\r\n";

            //rest
            if (infoFromForm.SecondAddress != "")
            {
                infoFromForm.SecondAddress = infoFromForm.SecondAddress + "\r\n\r\n";
            }
            //supporting custom logic: when second address is absent and second phone exists - adding one more string break between previous block and phone
            else if (infoFromForm.SecondHomePhone != "" && infoFromForm.SecondAddress == "")
            {
                infoFromForm.SecondAddress = "\r\n";
            }
            if (infoFromForm.SecondHomePhone != "")
            {
                infoFromForm.SecondHomePhone = "P: " + infoFromForm.SecondHomePhone + "\r\n\r\n";
            }

            //trimming a block if no further text exists
            string[] blocks = new string[] {firstBlock, secondBlock,thirdBlock, fourthBlock, infoFromForm.SecondAddress, infoFromForm.SecondHomePhone, infoFromForm.Notes };
            string textFromForm = firstBlock + secondBlock + thirdBlock + fourthBlock + infoFromForm.SecondAddress + infoFromForm.SecondHomePhone + infoFromForm.Notes;
            string buffer = "";
            for (int i = 0; i < blocks.Length; i++)
            {
                buffer = buffer + blocks[i];
                if (buffer.Length == textFromForm.Length)
                {
                    textFromForm = textFromForm.Trim();
                }
            }

            //cache should be cleared since we tweak data a lot
            contactInfoFromEditFormCache = null;

            return textFromForm;
        }

        public int GetCurrentAgeFromForm(int index)
        {
            ContactData infoFromForm = GetContactInformationFromDB(index);
            if (infoFromForm.BirthdayYear != "")
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime birthdayDate = DateTime.Parse($"{MonthConverter(infoFromForm.BirthdayMonth)}/{infoFromForm.BirthdayDay}/{infoFromForm.BirthdayYear}", provider).Date;
                //may be slightly incorrect, since used approx days per year
                int currentAge = Convert.ToInt32(Math.Truncate(((DateTime.Now.Date - birthdayDate.Date).TotalDays) / 365.2425));
                return currentAge;
            }
            return 0;
        }

        public int GetYearsTillAnniversaryFromForm(int index)
        {
            ContactData infoFromForm = GetContactInformationFromDB(index);
            if (infoFromForm.AnniversaryYear != "")
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime yearsTillAnniversary = DateTime.Parse($"{MonthConverter(infoFromForm.AnniversaryMonth)}/{infoFromForm.AnniversaryDay}/{infoFromForm.AnniversaryYear}", provider).Date;
                int anniversaryAge = Convert.ToInt32(Math.Truncate(((DateTime.Now.Date - yearsTillAnniversary.Date).TotalDays) / 365.2425));
                return anniversaryAge;
            }
            return 0;
        }

        public int GetNumberOfSearchResults()
        {
            manager.Navigator.GoToContactsPage();

            return Int32.Parse(driver.FindElement(By.Id("search_count")).Text);
        }
    }
}
