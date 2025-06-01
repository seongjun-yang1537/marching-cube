using System;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SUIElement
    {
        protected readonly UnityAction onRender;

        protected SUIElement()
        {

        }
        protected SUIElement(UnityAction onRender)
        {
            this.onRender = onRender;
        }

        public virtual void Render() => onRender?.Invoke();

        public static SUIElement operator |(SUIElement element, UnityAction<SUIElement> render)
        {
            render?.Invoke(element);
            return element;
        }

        public static SUIElement operator +(SUIElement a, SUIElement b)
            => new SUIElement(() =>
            {
                a.Render();
                b.Render();
            });
    }
}