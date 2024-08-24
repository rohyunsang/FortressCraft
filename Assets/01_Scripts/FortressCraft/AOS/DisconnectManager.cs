using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class DisconnectManager : MonoBehaviour
    {
        [SerializeField] private GameObject _disconnectPrompt;

        public GameObject DisconnectPrompt => _disconnectPrompt;

        private void Start()
        {
            _disconnectPrompt.SetActive(false);
        }

        public void AtteptDisconnect()
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if(gm == null)
            {
                return;
            }
            gm.DisconnectByPrompt = true;
        }
    }
}


