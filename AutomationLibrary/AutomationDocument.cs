using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Automation;
using System.Xml;
using System.Xml.XPath;

namespace AutomationLibrary
{
	public class AutomationDocument : XPathNavigator
	{
		AutomationElement element;
		TreeWalker walker;
        CacheRequest request;

		int attrib;

		AutomationProperty tagAttrib;
		AutomationProperty valueAttrib;

		static readonly AutomationProperty[] defaultAttribs = new AutomationProperty[] { AutomationElement.ClassNameProperty, AutomationElement.AutomationIdProperty, AutomationElement.NameProperty };
		List<KeyValuePair<String, AutomationProperty>> attribs;

		static List<KeyValuePair<String, AutomationProperty>> BuildAttributes(AutomationProperty[] arr) {
			List<KeyValuePair<String, AutomationProperty>> list = new List<KeyValuePair<String, AutomationProperty>>();
			foreach(AutomationProperty prop in arr)
			{
                String name = Automation.PropertyName(prop);                
				list.Add(new KeyValuePair<String, AutomationProperty>(name, prop));
			}
			return list;
		}

		String GetProperty(AutomationProperty prop) {
			if (element != null)  {
                object obj = element.GetCachedPropertyValue(prop, true);
				if (obj != null) { 
					return "" + obj;
				}
			}
			return null;
		}

        CacheRequest cacheProperties()
        {
            CacheRequest cacheRequest = new CacheRequest();
            cacheRequest.Add(tagAttrib);
            cacheRequest.Add(valueAttrib);            
            for (int i = 0; i < attribs.Count; i++)
            {
                AutomationProperty prop = attribs[i].Value;
                if (prop != tagAttrib || prop != valueAttrib) {
                    cacheRequest.Add(attribs[i].Value);
                }
            }
            cacheRequest.Activate();
            return cacheRequest;
        }

        public AutomationDocument() : this(AutomationElement.LocalizedControlTypeProperty, AutomationElement.NameProperty, defaultAttribs)
		{
			
		}

        public AutomationDocument(AutomationProperty tagAttrib, AutomationProperty valueAttrib)
            : this(tagAttrib, valueAttrib, defaultAttribs)
        {
            
        }

        public AutomationDocument(AutomationProperty tagAttrib, AutomationProperty valueAttrib, AutomationProperty[] automationElement)
            : this(tagAttrib, valueAttrib, BuildAttributes(automationElement))
        {
            
        }

        private AutomationDocument(AutomationProperty tagAttrib, AutomationProperty valueAttrib, List<KeyValuePair<String, AutomationProperty>> attribs)
        {
            this.tagAttrib = tagAttrib;
            this.valueAttrib = valueAttrib;
            this.attribs = attribs;
            this.attrib = -1;
            walker = TreeWalker.RawViewWalker;
            request = cacheProperties();
        }

		public override XPathNavigator Clone()
		{
            AutomationDocument other = new AutomationDocument(tagAttrib, valueAttrib, attribs);
			other.MoveTo(this);
			return other;
		}

		public override bool MoveToFirstAttribute()
		{
			int next = NextAttributeIndex(0);
			if (next > -1)
			{
				attrib = next;
				return true;
			}
			return false;
		}

		int NextAttributeIndex(int pos)
		{
			for (; pos < attribs.Count; pos++)
			{
				Object value = GetProperty(attribs[pos].Value);
				if (value != null)
				{
					return pos;
				}
			}
			return -1;
		}

		public override bool MoveToNextAttribute()
		{
			int next = NextAttributeIndex(attrib + 1);
			if (next > -1)
			{
				attrib = next;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToNext()
		{
			if (element == null)
			{
				return false;
			}
            AutomationElement e = walker.GetNextSibling(element, request);
			if (e == null) {
				return false;
			}
			element = e;
			return true;
		}

		public override bool MoveToPrevious()
		{
			if (element == null)
			{
				return false;
			}
            AutomationElement e = walker.GetPreviousSibling(element, request);
			if (e == null)
			{
				return false;
			}
			element = e;
			return true;
		}

		public override bool MoveToFirstChild()
		{
			if (element == null)
			{
				element = AutomationElement.RootElement;
				return true;
			}
            AutomationElement e = walker.GetFirstChild(element, request);
			if (e == null)
			{
				return false;
			}
			element = e;
			attrib = -1;
			return true;
		}

		public override bool MoveToParent()
		{
			if (element == null) { 
				return false;
			}
			AutomationElement e = walker.GetParent(element, request);
			element = e;
			attrib = -1;
			return true;
		}

		public override bool MoveTo(XPathNavigator other)
		{
			AutomationDocument o = other as AutomationDocument;
			if (o == null) { 
				return false;
			}
			element = o.element;
			attrib = o.attrib;
			return true;
		}

		public override bool MoveToId(string id)
		{
			throw new NotImplementedException();
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			AutomationDocument o = other as AutomationDocument;
			if (o == null) { 
				return false;
			}

			return AutomationElement.Equals(element, o.element) && attrib == o.attrib;
		}

		public override XmlNameTable NameTable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				if (element == null)
				{
					return XPathNodeType.Root;
				} 
				else if (attrib > -1)
				{
					return XPathNodeType.Attribute;
				}

				return XPathNodeType.Element;
			}
		}

		public override string LocalName
		{
			get
			{
				if (attrib > -1)
				{
					return attribs[attrib].Key;
				}

				return GetProperty(tagAttrib);
			}
		}

        public override string Value
        {
            get
            {
                if (attrib > -1)
                {
                    return GetProperty(attribs[attrib].Value);
                }

                return GetProperty(valueAttrib);
            }
        }

        public override object UnderlyingObject
        {
            get
            {
                return element;
            }
        }
		public override string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return string.Empty;
			}
		}

		public override string Prefix
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string BaseURI
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}

}
