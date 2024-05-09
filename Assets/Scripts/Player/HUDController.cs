using System;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text roleText;
        private float _timeSpan;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        private void Update()
        {
            _timeSpan += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(_timeSpan);
            timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public void SetRole(String role)
        {
            roleText.text = role;
        }
    }
}