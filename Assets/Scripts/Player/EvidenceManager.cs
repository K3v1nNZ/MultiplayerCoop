using System;
using FishNet.Object;
using Game.Networking;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class EvidenceManager : NetworkBehaviour
    {
        public static EvidenceManager Instance;
        [SerializeField] private TMP_Text hatEvidenceText;
        [SerializeField] private TMP_Text upperEvidenceText;
        [SerializeField] private TMP_Text lowerEvidenceText;
        
        public enum EvidenceType
        {
            Hat,
            Upper,
            Lower
        }

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
        
        [ServerRpc(RequireOwnership = false)]
        public void AddEvidence(int evidenceType)
        {
            SetEvidence(evidenceType);
        }
        
        [ObserversRpc(ExcludeOwner = false)]
        private void SetEvidence(int evidenceTypeInt)
        {
            EvidenceType evidenceType = (EvidenceType)evidenceTypeInt;
            
            switch (evidenceType)
            {
                case EvidenceType.Hat:
                    hatEvidenceText.text = NpcSpawner.Instance.targetNpcCont.hatItem.itemDescription;
                    break;
                case EvidenceType.Upper:
                    upperEvidenceText.text = NpcSpawner.Instance.targetNpcCont.upperItem.itemDescription;
                    break;
                case EvidenceType.Lower:
                    lowerEvidenceText.text = NpcSpawner.Instance.targetNpcCont.lowerItem.itemDescription;
                    break;
            }
        }
    }
}