using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ingame
{
    public abstract class UIComponentBahaviour : SerializedMonoBehaviour
    {
        [HideInInspector]
        public UIComponentBahaviour parentUI;
        [HideInInspector]
        public List<UIComponentBahaviour> childUIs;

        protected T GetChildUI<T>() where T : UIComponentBahaviour
            => (T)childUIs.First(child => child is T);

        protected void Awake()
        {
            InitializeUIComponent();
        }

        private void InitializeUIComponent()
        {
            Transform tr = transform.parent;
            while (tr != null)
            {
                UIComponentBahaviour ui = tr.GetComponent<UIComponentBahaviour>();
                if (ui != null)
                {
                    ui.childUIs.Add(this);
                    parentUI = ui;
                    break;
                }
                tr = tr.parent;
            }
        }
        public abstract void Render();

        public List<T> FindAllChild<T>() where T : UIComponentBahaviour
        {
            List<T> childs = new();
            if (this is T) childs.Add(this as T);

            foreach (var child in childUIs)
                childs.AddRange(child.FindAllChild<T>());
            return childs;
        }
    }
}