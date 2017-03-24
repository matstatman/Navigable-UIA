
using System.Collections.Generic;
using System.Windows.Automation;
using System.Xml.XPath;
namespace AutomationLibrary
{
    public class VirtualDocument : AutomationDocument
    {
        public VirtualDocument()
            : base()
        {
            Childs = new List<AutomationElement>();
            element = Root;
        }

        public override bool MoveTo(XPathNavigator other)
        {
            VirtualDocument o = other as VirtualDocument;
            if (o == null)
            {
                return false;
            }
            Childs = new List<AutomationElement>(o.Childs);
            return base.MoveTo(other);
        }

        public List<AutomationElement> Childs;

        public override bool MoveToParent()
        {
            int index = Childs.IndexOf(element);
            if (index != -1)
            {
                element = rootElement;
                attrib = -1;
                return true;
            }
            return base.MoveToParent();
        }

        public override bool MoveToFirstChild()
        {
            if (element == null)
            {
                element = rootElement;
                return true;
            }
            else if (element.Equals(rootElement))
            {
                if (Childs.Count == 0)
                {
                    return false;
                }
                element = Childs[0];
                attrib = -1;
                return true;
            }
            return base.MoveToFirstChild();
        }

        public override bool MoveToPrevious()
        {
            int index = Childs.IndexOf(element);
            if (index != -1)
            {
                index--;
                if (index >= 0 && index < Childs.Count)
                {
                    element = Childs[index];
                    attrib = -1;
                    return true;
                }
                return false;
            }
            return base.MoveToPrevious();
        }

        public override bool MoveToNext()
        {
            int index = Childs.IndexOf(element);
            if (index != -1)
            {
                index++;
                if (index < Childs.Count)
                {
                    element = Childs[index];
                    attrib = -1;
                    return true;
                }
                return false;
            }
            return base.MoveToNext();
        }
    }
}
