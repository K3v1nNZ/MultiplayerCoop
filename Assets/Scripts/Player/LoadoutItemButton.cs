using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    public class LoadoutItemButton : MonoBehaviour
    {
        [SerializeField] private LoadoutItemScriptableObject loadoutItem;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private RawImage itemImage;
        [SerializeField] private Button itemButton;

        public void SetupButton(string itenName, Texture2D itemIcon, Action onClickEvent)
        {
            itemNameText.text = itenName;
            itemImage.texture = itemIcon;
            itemButton.onClick.AddListener(() => onClickEvent());
        }
    }
}
