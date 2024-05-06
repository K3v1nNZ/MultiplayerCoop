using DG.Tweening;
using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class LockPickPin : MonoBehaviour
    {
        public float speed;
        public bool activated;
        public Transform indicator;
        public float height;
        private float _yMax;
        private float _yMin;
        private RectTransform _indicatorRectTransform;
        private RectTransform _pinRectTransform;
        private bool _moving;
        private bool _kill;

        private void Start()
        {
            _indicatorRectTransform = indicator.GetComponent<RectTransform>();
            _pinRectTransform = GetComponent<RectTransform>();
            if (PlayerController.Instance.playerRole == PlayerController.PlayerRole.Infiltrator)
            {
                height *= 2;
                _indicatorRectTransform.sizeDelta = new Vector2(_indicatorRectTransform.sizeDelta.x, height);
            }
            _yMax = (_pinRectTransform.sizeDelta.y / 2) - (height / 2);
            _yMin = -_yMax;
        }

        private void Update()
        {
            if (!activated || _moving) return;
            
            _moving = true;
            if (indicator.localPosition.y < _yMax)
            {
                indicator.DOLocalMoveY(_yMax, speed).SetEase(Ease.Linear).OnComplete(() => _moving = false);
            }
            else if (indicator.localPosition.y > _yMin)
            {
                indicator.DOLocalMoveY(_yMin, speed).SetEase(Ease.Linear).OnComplete(() => _moving = false);
            }
        }
    }
}