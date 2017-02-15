using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutomationLibrary.ObjectBased;
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;

namespace AutomationLibrary.Tests 
{
    public class Program : Model
    {
        [WindowNamed("TestWindow")]
        public AutomationElement Window;
    }

    public class WindowModel : Model
    {
        [AutomationId("okbutton")]
        public AutomationElement button;

        [XPath("//edit")]
        public AutomationElement edit;

        public void somelogic()
        {
            ValuePattern valuepattern = edit.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            valuepattern.SetValue("num clicks 9");
            InvokePattern invokepattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokepattern.Invoke();
        }
    }
    
    [TestClass]
    public class ObjectTests : TestEnv
    {
        Program program = new Program();
        WindowModel model = new WindowModel();
        ObjectProcessor parser = new ObjectProcessor();
        
        [TestMethod]
        public void TestParseAsObjects()
        {
            parser.refresh(AutomationElement.RootElement, program);
            parser.refresh(program.Window, model);
            model.somelogic();
            parser.refresh(program.Window, model);
            ValuePattern valuepattern = model.edit.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            Assert.AreEqual("num clicks 10", valuepattern.Current.Value);
        }
    }
}
