using System;
using System.Windows.Automation;

namespace AutomationLibrary
{
    public static class Actions
    {
        public static void Click(this AutomationElement element)
        {
            InvokePattern pattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (pattern == null)
            {
                throw new InvalidCastException("The invoke pattern is not supported by this element");
            }
            pattern.Invoke();
        }

        public static void SetValue(this AutomationElement element, String value)
        {
            ValuePattern pattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (pattern == null)
            {
                throw new InvalidCastException("The value pattern is not supported by this element");
            }
            pattern.SetValue(value);
        }

        public static String Value(this AutomationElement element)
        {
            ValuePattern pattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (pattern == null)
            {
                throw new InvalidCastException("The value pattern is not supported by this element");
            }
            return pattern.Current.Value;
        }
    }
}
