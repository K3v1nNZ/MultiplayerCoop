using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.MenuUI
{
    public class WaitingForClients : MonoBehaviour
    {
        [SerializeField] private TMP_Text content;
        [SerializeField] private string waitingText;
        
        private void Start()
        {
            StartCoroutine(AnimateText());
        }
        
        private IEnumerator AnimateText()
        {
            while (true)
            {
                content.text = waitingText;
                yield return new WaitForSeconds(0.5f);
                content.text = waitingText + ".";
                yield return new WaitForSeconds(0.5f);
                content.text = waitingText + "..";
                yield return new WaitForSeconds(0.5f);
                content.text = waitingText + "...";
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
