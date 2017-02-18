using AutomationLibrary.ObjectBased;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation;

namespace AutomationLibrary.Tests
{
    public class Program : Model
    {
        [WindowNamed("TestWindow")]
        public WindowModel Window;
    }

    public class WindowModel : Model
    {
        [AutomationId("okbutton")]
        public AutomationElement button;

        [XPath("//edit")]
        public AutomationElement edit;

        [AutomationId("mylistbox")]
        public ListBoxModel listbox;
    }

    public class ListBoxModel : Model
    {
        [XPath("/*/*[local-name()='list item']")]
        public AutomationElement[] items;
    }

    [TestClass]
    public class ObjectTests : TestEnv
    {
        Program program = new Program();
        ObjectProcessor parser = new ObjectProcessor();

        [TestMethod]
        public void TestParseAsObjects()
        {
            parser.parse(AutomationElement.RootElement, program);

            Assert.IsNotNull(program);
            Assert.IsNotNull(program.Window);
            Assert.IsNotNull(program.Window.listbox);
            Assert.IsNotNull(program.Window.edit);
            Assert.IsNotNull(program.Window.listbox.items);
            Assert.AreEqual(3, program.Window.listbox.items.Length);
        }
    }
}
