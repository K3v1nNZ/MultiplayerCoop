using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Environment
{
    public class InformantCameraButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void SetupButton(string cameraName, Action onClickAction)
        {
            buttonText.text = cameraName;
            _button.onClick.AddListener(() => onClickAction());
        }
    }
}
