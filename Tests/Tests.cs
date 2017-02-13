using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using AutomationLibrary;
using System.Xml.XPath;
using System.Windows.Automation;

namespace AutomationLibrary.Tests
{
    [TestClass]
    public class Tests
    {
        static String id = Process.GetCurrentProcess().Id.ToString();

        #region Setup test env
        static TestWindow window;
        static Thread thread;

        [ClassInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            AutoResetEvent reset = new AutoResetEvent(false);
            thread = new Thread(new ParameterizedThreadStart((data) =>
            {
                window = new TestWindow();
                window.Title += id;
                window.Loaded += (sender, e) => reset.Set();
                new System.Windows.Application().Run(window);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            reset.WaitOne();
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            window.Dispatcher.Invoke((Action) delegate { window.Close(); });
            thread.Join();
        }
        #endregion

        [TestMethod]
        public void DesktopHasParent()
        {
            XPathNavigator navigator = new AutomationDocument();
            XPathNodeIterator nodes = navigator.Select("/pane");
            Assert.IsTrue(nodes.MoveNext());
            Assert.IsTrue(nodes.Current.MoveToParent());
            Assert.IsFalse(nodes.Current.MoveToParent());
            Assert.IsNull(nodes.Current.UnderlyingObject);
        }

        [TestMethod]
        public void CanFindTestWindow()
        {
            XPathNavigator navigator = new AutomationDocument();
            XPathNodeIterator nodes = navigator.Select("/pane/window[@Name='TestWindow" + id + "']");
            Assert.IsTrue(nodes.MoveNext());
            Assert.IsFalse(nodes.MoveNext());
        }

        [TestMethod]
        public void TestWindowAsParent()
        {
            XPathNavigator navigator = new AutomationDocument();
            XPathNodeIterator nodes = navigator.Select("/pane/window[@Name='TestWindow" + id + "']");
            Assert.IsTrue(nodes.MoveNext());
            AutomationElement element = nodes.Current.UnderlyingObject as AutomationElement;
            navigator = new AutomationDocument(element);
            Assert.IsFalse(navigator.MoveToParent());
            Assert.IsFalse(navigator.MoveToNext());
            Assert.IsTrue(navigator.MoveToChild(XPathNodeType.Element));
            Assert.IsFalse(navigator.MoveToNext());
            nodes = navigator.Select("button[@AutomationId='okbutton']");
            Assert.IsTrue(nodes.MoveNext());
        }

        [TestMethod]
        public void NamesShouldBeTagNames()
        {
            XPathNavigator navigator = new AutomationDocument(AutomationElement.RootElement, AutomationElement.NameProperty, AutomationElement.LocalizedControlTypeProperty);
            XPathNodeIterator nodes = navigator.Select("/*/TestWindow" + id + "/*[.='button']");
            Assert.IsTrue(nodes.MoveNext());
            Assert.AreEqual("Click me", nodes.Current.LocalName);
            Assert.AreEqual("button", nodes.Current.Value);
            Assert.IsFalse(nodes.MoveNext());
        }

        [TestMethod]
        public void ClickButton()
        {
            XPathNavigator navigator = new AutomationDocument();
            XPathNodeIterator nodes = navigator.Select("/pane/window[@Name='TestWindow" + id + "']/button[@AutomationId='okbutton']");
            Assert.IsTrue(nodes.MoveNext());

            AutomationElement element = nodes.Current.UnderlyingObject as AutomationElement;
            InvokePattern invokepattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokepattern.Invoke();
            
            nodes = navigator.Select("/pane/window[@Name='TestWindow" + id + "']/edit");
            Assert.IsTrue(nodes.MoveNext());

            element = nodes.Current.UnderlyingObject as AutomationElement;
            ValuePattern valuepattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            Assert.AreEqual("num clicks 1", valuepattern.Current.Value);
        }
    }
}
