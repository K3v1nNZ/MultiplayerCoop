using DG.Tweening;
using UnityEngine;

namespace Game.Environment
{
    public class LockPickPin : MonoBehaviour
    {
        public float speed;
        public bool activated;
        public Transform indicator;
        public float height;
        [SerializeField] private float yMax;
        [SerializeField] private float yMin;
        private bool _moving;
        private bool _kill;

        private void Update()
        {
            if (!activated || _moving) return;
            
            _moving = true;
            if (indicator.localPosition.y < yMax)
            {
                indicator.DOLocalMoveY(yMax, speed).SetEase(Ease.Linear).OnComplete(() => _moving = false);
            }
            else if (indicator.localPosition.y > yMin)
            {
                indicator.DOLocalMoveY(yMin, speed).SetEase(Ease.Linear).OnComplete(() => _moving = false);
            }
        }
    }
}