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
        [SerializeField] private TMP_Text ammoText;
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
            ammoText.text = PlayerController.Instance.currentAmmo.ToString();
        }

        public void SetRole(string role)
        {
            roleText.text = role;
        }
    }
}