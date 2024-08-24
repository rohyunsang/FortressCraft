using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace Agit.FortressCraft
{
    public class LoadingMsg : NetworkBehaviour
    {
        [SerializeField] private GameObject info;

        public void OnInfo()
        { 
            RPCSetInfoActive();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetInfoActive()
        {
            info.SetActive(true);
        }
    }
}


