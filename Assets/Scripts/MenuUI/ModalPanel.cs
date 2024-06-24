using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MenuUI
{
    public class ModalPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TMP_Text confirmButtonText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text cancelButtonText;
        [SerializeField] private Button alternateButton;
        [SerializeField] private TMP_Text alternateButtonText;
        private Action _confirmAction;
        private Action _cancelAction;
        private Action _alternateAction;

        public void ShowModal(string header, string body, string confirmText, string cancelText, string alternateMessage, Action confirmAction, Action cancelAction = null, Action alternateAction = null)
        {
            headerText.text = header;
            bodyText.text = body;
            confirmButtonText.text = confirmText;
            _confirmAction = confirmAction;
            bool hasCancel = (cancelText != null);
            bool hasAlternate = (alternateMessage != null);
            cancelButton.gameObject.SetActive(hasCancel);
            alternateButton.gameObject.SetActive(hasAlternate);
            if (hasCancel)
            {
                cancelButtonText.text = cancelText;
                _cancelAction = cancelAction;
            }
            if (hasAlternate)
            {
                alternateButtonText.text = alternateMessage;
                _alternateAction = alternateAction;
            }
        }
        
        public void ConfirmButton()
        {
            _confirmAction?.Invoke();
            Destroy(gameObject);
        }
        
        public void CancelButton()
        {
            _cancelAction?.Invoke();
            Destroy(gameObject);
        }
        
        public void AlternateButton()
        {
            _alternateAction?.Invoke();
            Destroy(gameObject);
        }
    }
}
