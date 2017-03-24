using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation;

namespace AutomationLibrary.Tests
{
    [TestClass]
    public class VirtualTests : TestEnv
    {
        [TestMethod]
        public void TestTree()
        {
            VirtualDocument tree = new VirtualDocument();
            AutomationElement window = AutomationElement.FromHandle(hwnd);
            tree.Childs.Add(window);

            Assert.IsTrue(tree.MoveToFirstChild());
            Assert.IsTrue(tree.MoveToParent());
            Assert.IsTrue(tree.MoveToParent());
            Assert.IsFalse(tree.MoveToParent());
            Assert.IsTrue(tree.MoveToFirstChild());
            Assert.IsTrue(tree.MoveToFirstChild());
            Assert.AreEqual(window, tree.UnderlyingObject);
            Assert.IsTrue(tree.MoveToParent());
            Assert.AreEqual(AutomationElement.RootElement, tree.UnderlyingObject);
        }
    }
}
