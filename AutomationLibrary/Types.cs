using System;
using System.Linq;
using System.Windows.Automation;

namespace AutomationLibrary.ObjectBased
{
    public class AutomationAttribute : Attribute
    {
        internal String Value;
    }

    [AttributeUsageAttribute(AttributeTargets.Field)]
    public class XPath : AutomationAttribute
    {
        public XPath(String value)
        {
            Value = value;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Field)]
    public class AutomationId : XPath
    {
        public AutomationId(String id)
            : base("//*[@AutomationId='" + id.Replace("'", "\\'") + "']")
        {
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Field)]
    public class Named : XPath
    {
        public Named(String name)
            : base("//*[@Name='" + name.Replace("'", "\\'") + "']")
        {
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Field)]
    public class WindowNamed : XPath
    {
        public WindowNamed(String contains)
            : this(contains, 1)
        {

        }

        public WindowNamed(String contains, int level)
            : base(String.Concat(Enumerable.Repeat("/*", level).ToArray()) + "/window[contains(@Name,'" + contains.Replace("'", "\\'") + "')]")
        {

        }
    }

    public abstract class Model
    {

    }

    public abstract class ContainerModel : Model
    {
        internal ObjectProcessor parser = new ObjectProcessor();
        internal AutomationElement parent;
        internal AutomationElement container;
    }
}
