using AutomationLibrary.ObjectBased;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation;

namespace AutomationLibrary.Tests.Lazy
{
    public class Program : Model
    {
        [WindowNamed("TestWindow")]
        public Window Window;
    }

    public class Window : ReloadableObject<Window>
    {
        [XPath("//*[local-name()='menu item' and @Name='File']")]
        public FileMenu File;

        [XPath("//edit")]
        public AutomationElement Edit;
    }

    public class FileMenu : ReloadableObject<FileMenu>
    {
        [XPath("//*[local-name()='menu item' and @Name='New']")]
        public NewMenu New;
    }

    public class NewMenu : ReloadableObject<NewMenu>
    {
        [XPath("//*[local-name()='menu item' and @Name='Page']")]
        public AutomationElement Page;
    }

    [TestClass]
    public class ReloadableObjectTests : TestEnv
    {
        Program program = new Program();
        ObjectProcessor parser = new ObjectProcessor();

        [TestMethod]
        public void TestClicks()
        {
            parser.parse(AutomationElement.RootElement, program);
            program.Window.Click(p => p.File).Click(p => p.New).Click(p => p.Page);
            Assert.AreEqual("num clicks -1", program.Window.Edit.Value());
        }
    }
}