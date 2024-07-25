using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MenuUI
{
    public class LoadoutSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private RawImage itemIcon;
        
        public void SetItem(string itemName, Texture2D itemIcon)
        {
            this.itemName.text = itemName;
            this.itemIcon.texture = itemIcon;
        }
    }
}