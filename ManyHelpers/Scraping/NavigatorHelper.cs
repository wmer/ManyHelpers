using ManyHelpers.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ManyHelpers.Scraping {
    public class NavigatorHelper {
        private readonly string _link;
        private readonly string _driverLocation;
        private static int count;
        private int countWindow;

        public IWebDriver WebDriver { get; private set; }
        public IntPtr WindowHandler { get; private set; }
        public string WindowName { get; private set; }

        public NavigatorHelper(string webDriverLocation, string link) {
            _link = link;
            _driverLocation = webDriverLocation;
            count++;
            countWindow = count;
        }


        public void InitChrome() {
            var options = new ChromeOptions();
            WebDriver = new ChromeDriver(_driverLocation, options);
            if (Navigate(_link)) {
                var name = $"{WebDriver.Title}{countWindow}";
                ChangeWindowName(name);
            }
        }

        public void Close() => WebDriver.Close();

        public bool Navigate(string url) {
            try {
                WebDriver.Navigate().GoToUrl(url);
                while (!PageIsReady()) ;
                return true;
            } catch {
                return false;
            }
        }

        public bool Reload() {
            try {
                WebDriver.Navigate().Refresh();
                while (!PageIsReady()) ;
                return true;
            } catch {
                return false;
            }
        }

        public void DimissAlert() {
            try {
                var alert = WebDriver.SwitchTo().Alert();
                alert.Accept();
            } catch { }
        }

        public bool PageIsReady() {
            var ready = false;

            try {
                IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
                ready = js.ExecuteScript($"return document.readyState").Equals("complete");
            } catch { }

            return ready;
        }

        public Bitmap GetElementScreenShort(IWebElement element) {
            Bitmap bitmap = null;

            if (element != null) {
                Screenshot sc = ((ITakesScreenshot)WebDriver).GetScreenshot();
                var img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap;
                bitmap = img.Clone(new Rectangle(element.Location, element.Size), img.PixelFormat);
            }

            return bitmap;
        }

        public Bitmap GetElementScreenShort(By by) => 
                                GetElementScreenShort(WebDriver.FindElement(by));

        public void ChangeWindowName(string name) {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            WindowName = (string)js.ExecuteScript($"document.title = '{name}'");

            WindowHandler = IntPtr.Zero;

            while (WindowHandler == IntPtr.Zero) {
                WindowHandler = WindowHelper.GetWindowHandle(name);
            }
        }

    }
}
