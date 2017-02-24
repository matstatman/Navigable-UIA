using System;
using System.Windows.Automation;

namespace AutomationLibrary
{
    public static class Actions
    {
        public static void Click(this AutomationElement element)
        {
            Object pattern = null;
            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                InvokePattern invoke = pattern as InvokePattern;
                invoke.Invoke();
            }
            else if (element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
            {
                ExpandCollapsePattern expander = pattern as ExpandCollapsePattern;
                if (expander.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
                {
                    expander.Collapse();
                }
                else
                {
                    expander.Expand();
                }
            }
            else
            {
                throw new InvalidCastException("The no matching pattern found on this element");
            }
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
