using DG.Tweening;
using UnityEngine;

namespace Game.MenuUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuPanel : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        [HideInInspector] public bool isPanelShown;
        
        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void ShowCanvasGroup()
        {
            _canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.Linear).OnComplete(() => CanvasGroupInteractable(true));
        }

        public void HideCanvasGroup()
        {
            _canvasGroup.DOFade(0f, 0.15f).SetEase(Ease.Linear).OnComplete(() => CanvasGroupInteractable(false));
        }
        
        private void CanvasGroupInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
            isPanelShown = interactable;
        }
    }
}
