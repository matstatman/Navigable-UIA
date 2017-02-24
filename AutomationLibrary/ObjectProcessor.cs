using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Xml.XPath;

namespace AutomationLibrary.ObjectBased
{
    public class ObjectProcessor
    {
        private List<AutomationElement> All(AutomationDocument navigator, String xpath, ulong limit = 0)
        {
            XPathNodeIterator nodes = navigator.Select(xpath);
            List<AutomationElement> result = new List<AutomationElement>();
            while (nodes.MoveNext())
            {
                AutomationElement element = nodes.Current.UnderlyingObject as AutomationElement;
                if (AutomationElement.Equals(element, navigator.Root) == false)
                {
                    result.Add(element);
                    if (--limit == 0)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public void parse(AutomationElement parent, Model target)
        {
            refresh(new AutomationDocument(parent), target);
        }

        public void refresh(AutomationDocument navigator, Model target)
        {
            foreach (FieldInfo field in target.GetType().GetFields())
            {
                foreach (object attr in field.GetCustomAttributes(typeof(AutomationAttribute), true))
                {
                    if (attr is XPath)
                    {
                        ProcessXPath(attr as XPath, navigator, target, field);
                    }
                }
            }
        }

        private void ProcessXPath(XPath path, AutomationDocument navigator, Model target, FieldInfo field)
        {
            if (OfType(field, typeof(AutomationElement)))
            {
                if (field.FieldType.IsArray)
                {
                    List<AutomationElement> hits = All(navigator, path.Value);
                    field.SetValue(target, hits.Count > 0 ? hits.ToArray() : null);
                }
                else
                {
                    List<AutomationElement> hits = All(navigator, path.Value, 1);
                    field.SetValue(target, hits.Count > 0 ? hits[0] : null);
                }
            }
            else if (OfType(field, typeof(Model)))
            {
                if (field.FieldType.IsArray)
                {
                    List<AutomationElement> hits = All(navigator, path.Value);
                    Array arr = null;
                    if (hits.Count > 0)
                    {
                        Type modeltype = field.FieldType.GetElementType();
                        ConstructorInfo modelctor = FindConstructor(modeltype);
                        arr = Array.CreateInstance(modeltype, hits.Count);
                        for (int i = 0; i < hits.Count; i++)
                        {
                            Model model = modelctor.Invoke(new object[] { }) as Model;
                            ContainerModel co = model as ContainerModel;
                            if (field.FieldType.IsSubclassOf(modeltype))
                            {
                                co.parent = navigator.Root;
                                co.container = hits[i];
                            }
                            AutomationDocument clone = navigator.Clone() as AutomationDocument;
                            clone.Root = hits[i];
                            refresh(clone, model);
                            arr.SetValue(model, i);
                        }
                    }
                    field.SetValue(target, arr);
                }
                else
                {
                    List<AutomationElement> hits = All(navigator, path.Value, 1);
                    Model model = null;
                    if (hits.Count > 0)
                    {
                        ConstructorInfo ctor = FindConstructor(field.FieldType);
                        model = ctor.Invoke(new object[] { }) as Model;
                        ContainerModel co = model as ContainerModel;
                        if (field.FieldType.IsSubclassOf(typeof(ContainerModel)))
                        {
                            co.parent = navigator.Root;
                            co.container = hits[0];
                        }
                        AutomationDocument clone = navigator.Clone() as AutomationDocument;
                        clone.Root = hits[0];
                        refresh(clone, model);
                    }
                    field.SetValue(target, model);
                }
            }
        }

        private static ConstructorInfo FindConstructor(Type t)
        {
            ConstructorInfo ctor = t.GetConstructor(new Type[] { });
            if (ctor == null)
            {
                throw new MissingMethodException("Missing no arg constuctor for " + t.FullName);
            }
            return ctor;
        }

        private static bool OfType(FieldInfo field, Type t)
        {
            return t.IsAssignableFrom(field.FieldType) || (field.FieldType.IsArray && t.IsAssignableFrom(field.FieldType.GetElementType()));
        }
    }
}
