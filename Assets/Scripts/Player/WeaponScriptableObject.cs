using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class WeaponScriptableObject : ScriptableObject
    {
        public string weaponName;
        public WeaponType weaponType;
        public FireMode fireMode;
        public int damage;
        public float fireRate;
        public int clipSize;
        public float reloadTime;
        public GameObject weaponModel; 
        public GameObject bulletTrail;
        public AudioClip fireSound;
        public AudioClip reloadSound;

        public enum WeaponType
        {
            Melee,
            Ranged,
            None
        }
        public enum FireMode
        {
            Single,
            Automatic
        }
    }
}