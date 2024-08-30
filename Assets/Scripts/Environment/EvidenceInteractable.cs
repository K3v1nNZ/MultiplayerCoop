using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class EvidenceInteractable : MonoBehaviour, IInteractable
    {
        public EvidenceManager.EvidenceType evidenceType;
        
        public void Interact(PlayerController interactor)
        {
            EvidenceManager.Instance.AddEvidence((int)evidenceType);
            Destroy(gameObject);
        }
    }
}
