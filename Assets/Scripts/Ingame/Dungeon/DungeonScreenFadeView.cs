using System.Collections;
using Corelib.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ingame
{
    public class DungeonScreenFadeView : MonoBehaviour
    {
        public float fadeDuration = 1.5f;
        public Image imgProgresss;

        private CanvasGroup canvasGroup;

        protected void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            DungeonManager.Instance.onDungeonGenerationProgress.AddListener(progress => OnDungeonGenerationProgress(progress));
        }

        public IEnumerator FadeIn()
        {
            imgProgresss.fillAmount = 0f;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            yield return canvasGroup.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        public IEnumerator FadeOut()
        {
            canvasGroup.alpha = 1f;
            yield return canvasGroup.DOFade(0f, fadeDuration).WaitForCompletion();
            canvasGroup.interactable = false;
            imgProgresss.fillAmount = 0f;
        }

        private void OnDungeonGenerationProgress(float progress)
        {
            imgProgresss.fillAmount = progress;
        }
    }
}