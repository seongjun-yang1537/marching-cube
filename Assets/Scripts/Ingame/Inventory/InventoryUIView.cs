using UnityEngine;
using DG.Tweening;

namespace Ingame
{
    [RequireComponent(typeof(InventoryUIModel))]
    public class InventoryUIView : MonoBehaviour
    {
        private InventoryUIModel _model;
        public InventoryUIModel Model { get => _model ??= GetComponent<InventoryUIModel>(); }

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            SubscribeEventListeners();
        }

        private void SubscribeEventListeners()
        {
            Model.onVisible.AddListener(visible =>
            {
                (float from, float to) = (0f, 1f);
                if (!visible) (from, to) = (to, from);

                canvasGroup.alpha = from;
                canvasGroup.DOFade(to, 0.5f);
            });
        }
    }
}