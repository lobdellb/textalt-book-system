using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;

 */

//using NUnit.Framework;
using Selenium;



namespace NewDownloader
{
    class Program
    {
        static void Main(string[] args)
        {

            string bookHomeUrl = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBWizardView?catalogId=10001&storeId=36052&langId=-1";

            //FirefoxBinary fb = new FirefoxBinary();
            ISelenium selenium = new DefaultSelenium("192.168.1.119", 4444, "*firefox",bookHomeUrl);
            selenium.Start();


            selenium.Open(bookHomeUrl);

            FileStream Fp = new FileStream(@"C:\Users\Bryce Lobdell\Desktop\bn.html",FileMode.Create);

            string Html = selenium.GetHtmlSource();
            // string Html = selenium.GetEval("document.getElementsByTagName('html')[0].innerHTML;");


            // string Body = selenium.GetBodyText();

            byte[] Data = Encoding.UTF8.GetBytes(Html);

            Fp.Write(Data,0,Data.Length);
            Fp.Close();




            Console.Write(Html);


            // driver.Navigate().GoToUrl("/");
            // driver.FindElement(By.XPath("//li[3]/a/strong")).Click();

            // Selenium.DefaultSelenium driver = new Selenium.DefaultSelenium();




            /*
            selenium = new DefaultSelenium("localhost", 4444, "*firefox", "http://www.google.com/");
            selenium.Start();
            verificationErrors = new StringBuilder();
            */
            Console.ReadLine();

        }
    }
}
