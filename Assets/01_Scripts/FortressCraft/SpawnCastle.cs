using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Realtime;
using UnityEngine.UI;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class SpawnCastle : NetworkBehaviour
    {
        [SerializeField] private NetworkObject castle;
        
        public void SpawnCastleObject()
        {
            NetworkObject NO = Runner.Spawn(castle, new Vector3(0f,0f,0f), Quaternion.identity);
            string tag = "";

            int idx = 0;
            if (Runner.TryGetSingleton(out GameManager gameManager))
            {
                // gameManager.MakeDictionaryPlayerIdUsingPlayerRef();
                idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);
            }
            
            Debug.Log("AAAAAAAAA" + idx);

            switch (idx)
            {
                case 0 :
                    tag = "A";
                    break;
                case 1:
                    tag = "B";
                    break;
                case 2:
                    tag = "C";
                    break;
                case 3:
                    tag = "D";
                    break;
            }
                

            RPC_SpawnCastleTransformSync(NO,tag);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SpawnCastleTransformSync(NetworkObject NO, string tag)
        {
            NO.gameObject.tag = tag;
            NO.transform.position = transform.parent.position;
            NO.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            NO.gameObject.GetComponent<Castle>().SliderInit();

        }
    }

}

