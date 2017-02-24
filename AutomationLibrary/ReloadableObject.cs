using System;
using System.Windows.Automation;

namespace AutomationLibrary.ObjectBased
{
    public abstract class ReloadableObject<T> : ContainerModel where T : ReloadableObject<T>
    {
        private T Self
        {
            get
            {
                return (T)this;
            }
        }

        public AutomationElement Click(Func<T, AutomationElement> field)
        {
            AutomationElement obj = null;
            Poll(e => { obj = field.Invoke(Self); return obj != null; });
            obj.Click();
            return obj;
        }

        public E Click<E>(Func<T, E> field) where E : ReloadableObject<E>
        {
            E obj = null;
            Poll(e => { obj = field.Invoke(Self); return obj != null; });
            obj.container.Click();
            return obj;
        }

        public E WaitFor<E>(Func<T, E> field) where E : ReloadableObject<E>
        {
            E obj = null;
            Poll(e => { obj = field.Invoke(Self); return obj != null; });
            return obj;
        }

        public T Poll(Predicate<T> condition)
        {
            while (condition.Invoke(Self) == false)
            {
                parser.parse(parent, this);
            }
            return Self;
        }
    }
}
