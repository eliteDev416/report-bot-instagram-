using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Instagram_Email_Scrape
{
    class CWebBrowser
    {
        public IWebDriver googleChrome()
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("disable-infobars");               //disable test automation message
            option.AddArguments("--disable-notifications");        //disable notifications
            option.AddArguments("--disable-web-security");         //disable save password windows
            option.AddArguments("--window-position=-32000,-32000");
            option.AddUserProfilePreference("credentials_enable_service", false);

            option.AddUserProfilePreference("browser.download.manager.showWhenStarting", false);
            option.AddUserProfilePreference("browser.helperApps.neverAsk.saveToDisk", "text/csv");
            option.AddUserProfilePreference("disable-popup-blocking", "true");
            option.AddUserProfilePreference("safebrowsing.enabled", true);
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(driverService, option);
            return driver;
        }

        public void instagramLogin(IWebDriver driver, string username, string password)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.instagram.com/accounts/login/");
                Thread.Sleep(1500);
                driver.FindElement(By.XPath("(//input[@class='_2hvTZ pexuQ zyHYP'])[1]")).SendKeys(username);
                Thread.Sleep(500);
                driver.FindElement(By.XPath("(//input[@class='_2hvTZ pexuQ zyHYP'])[2]")).SendKeys(password);
                Thread.Sleep(300);
                var collection = driver.FindElements(By.XPath("//button"));
                foreach (var item in collection)
                {
                    if (item.Text.Contains("Log in"))
                    {
                        item.Click();
                        break;
                    }
                }
                Thread.Sleep(500);
            }
            catch (Exception)
            {
            }
        }

        public void newTab(IWebDriver driver, string url)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.open()");
            driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(500);
        }
        public void Tab(IWebDriver driver)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.open()");
            driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
        }
        public void closeBrowser(IWebDriver driver)
        {
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles.First());
        }
        public void pageDown(IWebDriver driver)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(500);
        }
        public void buttonClick(IWebDriver driver, string element, string txt)
        {
            try
            {
                var buttons = driver.FindElements(By.XPath(element));
                foreach (var item in buttons)
                {
                    if (item.Text.Contains(txt))
                    {
                        item.Click();
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            System.Threading.Thread.Sleep(500);
        }

        public void alertAccept(IWebDriver driver)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("window.onbeforeunload = function(e){};");
            }
            catch (Exception)
            {
            }
            Thread.Sleep(500);
        }

        public string getSite(IWebDriver driver, string url)
        {
            string text = "";
            try
            {
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(500);
                try
                {
                    text = driver.FindElement(By.XPath("//body")).GetAttribute("innerHTML");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }
            return text;
        }
    }
}
