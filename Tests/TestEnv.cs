using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace AutomationLibrary.Tests
{
    [TestClass]
    public class TestEnv
    {
        internal static String id = Process.GetCurrentProcess().Id.ToString();
        static TestWindow window;
        static Thread thread;

        [AssemblyInitialize()]
        public static void MyTestSetup(TestContext context)
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

        [AssemblyCleanup]
        public static void TestCleanup()
        {
            window.Dispatcher.Invoke((Action)delegate { window.Close(); });
            thread.Join();
        }
    }
}
