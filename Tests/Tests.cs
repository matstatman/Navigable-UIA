using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation;
using System.Xml.XPath;

namespace AutomationLibrary.Tests
{
    [TestClass]
    public class Tests : TestEnv
    {
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
            Assert.IsNull(navigator.UnderlyingObject);
            Assert.IsFalse(navigator.MoveToNext());
            Assert.IsFalse(navigator.MoveToPrevious());
            Assert.IsNull(navigator.UnderlyingObject);
            Assert.IsTrue(navigator.MoveToChild(XPathNodeType.Element));
            Assert.IsNotNull(navigator.UnderlyingObject);
            Assert.IsTrue(navigator.MoveToParent());
            Assert.IsNull(navigator.UnderlyingObject);
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
