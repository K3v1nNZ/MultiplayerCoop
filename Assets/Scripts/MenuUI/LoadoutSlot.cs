using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MenuUI
{
    public class LoadoutSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private RawImage itemIcon;
        private AspectRatioFitter _aspectRatioFitter;

        private void Start()
        {
            _aspectRatioFitter = itemIcon.gameObject.GetComponent<AspectRatioFitter>();
        }

        public void SetItem(string itemName, Texture2D itemIcon)
        {
            this.itemName.text = itemName;
            this.itemIcon.texture = itemIcon;
            _aspectRatioFitter.aspectRatio = (float) itemIcon.width / itemIcon.height;
        }
    }
}