using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Realtime;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class SpawnCastle : NetworkBehaviour
    {
        [SerializeField] private NetworkObject castle;
        
        public void SpawnCastleObject()
        {
            NetworkObject NO = Runner.Spawn(castle, new Vector3(0f,0f,0f), Quaternion.identity); 
            RPC_SpawnCastleTransformSync(NO);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SpawnCastleTransformSync(NetworkObject NO)
        {
            NO.transform.position = transform.parent.position;
            NO.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            NO.gameObject.GetComponent<Castle>().SliderInit();
        }
    }

}

