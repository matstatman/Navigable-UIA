using System;
using System.Windows;

namespace AutomationLibrary.Tests
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textbox1.Text = "num clicks " + (Int32.Parse(textbox1.Text.Substring("num clicks ".Length)) + 1);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            textbox1.Text = "num clicks -1";
        }
    }
}
