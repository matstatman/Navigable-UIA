using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Automation;
using System.Xml.XPath;

namespace AutomationLibrary.ObjectBased
{
    public class ObjectProcessor
    {

        public AutomationElement First(String xpath)
        {
            return First(AutomationElement.RootElement, xpath);
        }

        public AutomationElement First(AutomationElement parent, String xpath)
        {
            XPathNavigator navigator = new AutomationDocument(parent);
            XPathNodeIterator nodes = navigator.Select(xpath);
            if (nodes.MoveNext()) {
                return nodes.Current.UnderlyingObject as AutomationElement;
            }
            return null;
        }

        public void refresh(AutomationElement parent, Model target)
        {
            foreach (FieldInfo prop in target.GetType().GetFields())
            {
                foreach (object attr in prop.GetCustomAttributes(typeof(AutomationAttribute), true))
                {
                    if (attr is XPath)
                    {
                        XPath path = attr as XPath;
                        AutomationElement hit = First(parent, path.Value);
                        prop.SetValue(target, hit);
                    }
                }
            }
        }
    }
}
