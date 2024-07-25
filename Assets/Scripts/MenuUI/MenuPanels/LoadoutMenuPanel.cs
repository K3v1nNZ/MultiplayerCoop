using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MenuUI.MenuPanels
{
    public class LoadoutSettings
    {
        public string Warning;
        public string AssassinPrimaryWeapon;
        public string AssassinEquipmentOne;
        public string AssassinEquipmentTwo;
        public string AssassinEquipmentThree;
        public string AssassinCosmeticOne;
        public string AssassinCosmeticTwo;
        
        public string InfiltratorPrimaryWeapon;
        public string InfiltratorEquipmentOne;
        public string InfiltratorEquipmentTwo;
        public string InfiltratorEquipmentThree;
        public string InfiltratorCosmeticOne;
        public string InfiltratorCosmeticTwo;
        
        public string HackerPrimaryWeapon;
        public string HackerEquipmentOne;
        public string HackerEquipmentTwo;
        public string HackerEquipmentThree;
        public string HackerCosmeticOne;
        public string HackerCosmeticTwo;
    }
    
    public class LoadoutMenuPanel : MenuPanel
    {
        [SerializeField] private GameObject assassinLoadoutPanel;
        [SerializeField] private GameObject infiltratorLoadoutPanel;
        [SerializeField] private GameObject hackerLoadoutPanel;
        [SerializeField] private GameObject chooseItemPanel;
        [SerializeField] private GameObject chooseItemPanelContainer;
        [SerializeField] private GameObject chooseItemButtonPrefab;
        private LoadoutSettings _loadoutSettings;
        private LoadoutItemScriptableObject.ItemUsableRole _usableRole;
        private LoadoutItemScriptableObject.ItemType _itemType;
        private int _slot;
        
        [SerializeField] private LoadoutSlot assassinPrimaryWeaponSlot;
        [SerializeField] private LoadoutSlot assassinEquipmentOneSlot;
        [SerializeField] private LoadoutSlot assassinEquipmentTwoSlot;
        [SerializeField] private LoadoutSlot assassinEquipmentThreeSlot;
        [SerializeField] private LoadoutSlot assassinCosmeticOneSlot;
        [SerializeField] private LoadoutSlot assassinCosmeticTwoSlot;
        
        [SerializeField] private LoadoutSlot infiltratorPrimaryWeaponSlot;
        [SerializeField] private LoadoutSlot infiltratorEquipmentOneSlot;
        [SerializeField] private LoadoutSlot infiltratorEquipmentTwoSlot;
        [SerializeField] private LoadoutSlot infiltratorEquipmentThreeSlot;
        [SerializeField] private LoadoutSlot infiltratorCosmeticOneSlot;
        [SerializeField] private LoadoutSlot infiltratorCosmeticTwoSlot;
        
        [SerializeField] private LoadoutSlot hackerPrimaryWeaponSlot;
        [SerializeField] private LoadoutSlot hackerEquipmentOneSlot;
        [SerializeField] private LoadoutSlot hackerEquipmentTwoSlot;
        [SerializeField] private LoadoutSlot hackerEquipmentThreeSlot;
        [SerializeField] private LoadoutSlot hackerCosmeticOneSlot;
        [SerializeField] private LoadoutSlot hackerCosmeticTwoSlot;

        public override void PanelStart()
        {
            if (!File.Exists(Application.persistentDataPath + "/loadout.json"))
            {
                GenerateLoadoutConfig();
            }
            _loadoutSettings = JsonUtility.FromJson<LoadoutSettings>(File.ReadAllText(Application.persistentDataPath + "/loadout.json"));
            //TODO: Check if all of the items in the list actually exist in the game
            
            assassinPrimaryWeaponSlot.SetItem(GetItem(_loadoutSettings.AssassinPrimaryWeapon).itemName, GetItem(_loadoutSettings.AssassinPrimaryWeapon).itemIcon);
            assassinEquipmentOneSlot.SetItem(GetItem(_loadoutSettings.AssassinEquipmentOne).itemName, GetItem(_loadoutSettings.AssassinEquipmentOne).itemIcon);
            assassinEquipmentTwoSlot.SetItem(GetItem(_loadoutSettings.AssassinEquipmentTwo).itemName, GetItem(_loadoutSettings.AssassinEquipmentTwo).itemIcon);
            assassinEquipmentThreeSlot.SetItem(GetItem(_loadoutSettings.AssassinEquipmentThree).itemName, GetItem(_loadoutSettings.AssassinEquipmentThree).itemIcon);
            assassinCosmeticOneSlot.SetItem(GetItem(_loadoutSettings.AssassinCosmeticOne).itemName, GetItem(_loadoutSettings.AssassinCosmeticOne).itemIcon);
            assassinCosmeticTwoSlot.SetItem(GetItem(_loadoutSettings.AssassinCosmeticTwo).itemName, GetItem(_loadoutSettings.AssassinCosmeticTwo).itemIcon);
            
            infiltratorPrimaryWeaponSlot.SetItem(GetItem(_loadoutSettings.InfiltratorPrimaryWeapon).itemName, GetItem(_loadoutSettings.InfiltratorPrimaryWeapon).itemIcon);
            infiltratorEquipmentOneSlot.SetItem(GetItem(_loadoutSettings.InfiltratorEquipmentOne).itemName, GetItem(_loadoutSettings.InfiltratorEquipmentOne).itemIcon);
            infiltratorEquipmentTwoSlot.SetItem(GetItem(_loadoutSettings.InfiltratorEquipmentTwo).itemName, GetItem(_loadoutSettings.InfiltratorEquipmentTwo).itemIcon);
            infiltratorEquipmentThreeSlot.SetItem(GetItem(_loadoutSettings.InfiltratorEquipmentThree).itemName, GetItem(_loadoutSettings.InfiltratorEquipmentThree).itemIcon);
            infiltratorCosmeticOneSlot.SetItem(GetItem(_loadoutSettings.InfiltratorCosmeticOne).itemName, GetItem(_loadoutSettings.InfiltratorCosmeticOne).itemIcon);
            infiltratorCosmeticTwoSlot.SetItem(GetItem(_loadoutSettings.InfiltratorCosmeticTwo).itemName, GetItem(_loadoutSettings.InfiltratorCosmeticTwo).itemIcon);
            
            hackerPrimaryWeaponSlot.SetItem(GetItem(_loadoutSettings.HackerPrimaryWeapon).itemName, GetItem(_loadoutSettings.HackerPrimaryWeapon).itemIcon);
            hackerEquipmentOneSlot.SetItem(GetItem(_loadoutSettings.HackerEquipmentOne).itemName, GetItem(_loadoutSettings.HackerEquipmentOne).itemIcon);
            hackerEquipmentTwoSlot.SetItem(GetItem(_loadoutSettings.HackerEquipmentTwo).itemName, GetItem(_loadoutSettings.HackerEquipmentTwo).itemIcon);
            hackerEquipmentThreeSlot.SetItem(GetItem(_loadoutSettings.HackerEquipmentThree).itemName, GetItem(_loadoutSettings.HackerEquipmentThree).itemIcon);
            hackerCosmeticOneSlot.SetItem(GetItem(_loadoutSettings.HackerCosmeticOne).itemName, GetItem(_loadoutSettings.HackerCosmeticOne).itemIcon);
            hackerCosmeticTwoSlot.SetItem(GetItem(_loadoutSettings.HackerCosmeticTwo).itemName, GetItem(_loadoutSettings.HackerCosmeticTwo).itemIcon);
        }

        private static void GenerateLoadoutConfig()
        {
            File.WriteAllText(Application.persistentDataPath + "/loadout.json", JsonUtility.ToJson(new LoadoutSettings
            {
                Warning = "Do not modify this file unless you know what you are doing. If you make an error in the file and the game can't find the correct data, it will reset your loadout to default.",
                AssassinPrimaryWeapon = "1",
                AssassinEquipmentOne = "4",
                AssassinEquipmentTwo = "5",
                AssassinEquipmentThree = "6",
                AssassinCosmeticOne = "13",
                AssassinCosmeticTwo = "14",
                
                InfiltratorPrimaryWeapon = "2",
                InfiltratorEquipmentOne = "7",
                InfiltratorEquipmentTwo = "8",
                InfiltratorEquipmentThree = "9",
                InfiltratorCosmeticOne = "15",
                InfiltratorCosmeticTwo = "16",
                
                HackerPrimaryWeapon = "3",
                HackerEquipmentOne = "10",
                HackerEquipmentTwo = "11",
                HackerEquipmentThree = "12",
                HackerCosmeticOne = "17",
                HackerCosmeticTwo = "18"
            }, true));
        }
        
        private void SaveLoadoutConfig()
        {
            File.WriteAllText(Application.persistentDataPath + "/loadout.json", JsonUtility.ToJson(_loadoutSettings, true));
        }
        
        private static LoadoutItemScriptableObject GetItem(string itemID)
        {
            return Resources.LoadAll<LoadoutItemScriptableObject>("Loadout").ToList().Find(x => x.itemID.ToString() == itemID);
        }

        public void SetSelectMenuRole(int usableRole)
        {
            _usableRole = (LoadoutItemScriptableObject.ItemUsableRole)usableRole;
        }

        public void SetSlot(int slot)
        {
            _slot = slot;
        }
        
        public void CloseSelectMenu()
        {
            chooseItemPanel.SetActive(false);
        }
        
        public void OpenSelectMenu(int itemType)
        {
            _itemType = (LoadoutItemScriptableObject.ItemType)itemType;
            chooseItemPanel.SetActive(true);
            foreach (Transform child in chooseItemPanelContainer.transform)
            {
                Destroy(child.gameObject);
            }
            List<LoadoutItemScriptableObject> loadoutItems = Resources.LoadAll<LoadoutItemScriptableObject>("Loadout")
                .Where(item => item.usableRole == _usableRole || item.usableRole == LoadoutItemScriptableObject.ItemUsableRole.All)
                .Where(item => item.itemType == _itemType)
                .ToList();
            foreach (LoadoutItemScriptableObject item in loadoutItems)
            {
                GameObject button = Instantiate(chooseItemButtonPrefab, chooseItemPanelContainer.transform);
                button.GetComponentInChildren<TMP_Text>().text = item.itemName;
                button.GetComponentInChildren<RawImage>().texture = item.itemIcon;
                button.GetComponent<Button>().onClick.AddListener(() => SelectItem(item.itemID));
            }
        }

        private void SelectItem(int itemID)
        {
            chooseItemPanel.SetActive(false);
            LoadoutItemScriptableObject item = GetItem(itemID.ToString());
            switch (_itemType)
            {
                case LoadoutItemScriptableObject.ItemType.PrimaryWeapon:
                    switch (_usableRole)
                    {
                        case LoadoutItemScriptableObject.ItemUsableRole.Assassin:
                            _loadoutSettings.AssassinPrimaryWeapon = itemID.ToString();
                            assassinPrimaryWeaponSlot.SetItem(item.itemName, item.itemIcon);
                            return;
                        case LoadoutItemScriptableObject.ItemUsableRole.Infiltrator:
                            _loadoutSettings.InfiltratorPrimaryWeapon = itemID.ToString();
                            infiltratorPrimaryWeaponSlot.SetItem(item.itemName, item.itemIcon);
                            return;
                        case LoadoutItemScriptableObject.ItemUsableRole.Hacker:
                            _loadoutSettings.HackerPrimaryWeapon = itemID.ToString();
                            hackerPrimaryWeaponSlot.SetItem(item.itemName, item.itemIcon);
                            return;
                        default:
                            MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item is not compatible with this role.", "Ok", null, null, null);
                            return;
                    }
                    
                case LoadoutItemScriptableObject.ItemType.Equipment:
                    switch (_usableRole)
                    {
                        case LoadoutItemScriptableObject.ItemUsableRole.Assassin:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.AssassinEquipmentOne = itemID.ToString();
                                    assassinEquipmentOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.AssassinEquipmentTwo = itemID.ToString();
                                    assassinEquipmentTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 3:
                                    _loadoutSettings.AssassinEquipmentThree = itemID.ToString();
                                    assassinEquipmentThreeSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        case LoadoutItemScriptableObject.ItemUsableRole.Infiltrator:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.InfiltratorEquipmentOne = itemID.ToString();
                                    infiltratorEquipmentOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.InfiltratorEquipmentTwo = itemID.ToString();
                                    infiltratorEquipmentTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 3:
                                    _loadoutSettings.InfiltratorEquipmentTwo = itemID.ToString();
                                    infiltratorEquipmentThreeSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        case LoadoutItemScriptableObject.ItemUsableRole.Hacker:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.HackerEquipmentOne = itemID.ToString();
                                    hackerEquipmentOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.HackerEquipmentTwo = itemID.ToString();
                                    hackerEquipmentTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 3:
                                    _loadoutSettings.HackerEquipmentThree = itemID.ToString();
                                    hackerEquipmentThreeSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        default:
                            MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item is not compatible with this role.", "Ok", null, null, null);
                            return;
                    }
                    
                case LoadoutItemScriptableObject.ItemType.Cosmetic:
                    switch (_usableRole)
                    {
                        case LoadoutItemScriptableObject.ItemUsableRole.Assassin:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.AssassinCosmeticOne = itemID.ToString();
                                    assassinCosmeticOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.AssassinCosmeticTwo = itemID.ToString();
                                    assassinCosmeticTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        case LoadoutItemScriptableObject.ItemUsableRole.Infiltrator:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.InfiltratorCosmeticOne = itemID.ToString();
                                    infiltratorCosmeticOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.InfiltratorCosmeticTwo = itemID.ToString();
                                    infiltratorCosmeticTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        case LoadoutItemScriptableObject.ItemUsableRole.Hacker:
                            switch (_slot)
                            {
                                case 1:
                                    _loadoutSettings.HackerCosmeticOne = itemID.ToString();
                                    hackerCosmeticOneSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                case 2:
                                    _loadoutSettings.HackerCosmeticTwo = itemID.ToString();
                                    hackerCosmeticTwoSlot.SetItem(item.itemName, item.itemIcon);
                                    return;
                                default:
                                    MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item slot is invalid.", "Ok", null, null, null);
                                    return;
                            }
                        default:
                            MainMenuManager.Instance.ShowModal("Error", "An error occurred while selecting the item. Item is not compatible with this role.", "Ok", null, null, null);
                            return;
                    }
            }
            
        }
        
        public void AssassinButton()
        {
            assassinLoadoutPanel.SetActive(true);
            infiltratorLoadoutPanel.SetActive(false);
            hackerLoadoutPanel.SetActive(false);
        }
        
        public void InfiltratorButton()
        {
            assassinLoadoutPanel.SetActive(false);
            infiltratorLoadoutPanel.SetActive(true);
            hackerLoadoutPanel.SetActive(false);
        }
        
        public void HackerButton()
        {
            assassinLoadoutPanel.SetActive(false);
            infiltratorLoadoutPanel.SetActive(false);
            hackerLoadoutPanel.SetActive(true);
        }
        
        public void BackButton()
        {
            SaveLoadoutConfig();
            MainMenuManager.Instance.loadoutMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.mainMenuPanel.ShowCanvasGroup();
        }
    }
}
