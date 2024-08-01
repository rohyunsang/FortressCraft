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
            Team team;
            team = Team.A; // Default
            switch (idx)
            {
                case 0 :
                    tag = "A";
                    team = Team.A;
                    break;
                case 1:
                    tag = "B";
                    team = Team.B;
                    break;
                case 2:
                    tag = "C";
                    team = Team.C;
                    break;
                case 3:
                    tag = "D";
                    team = Team.D;
                    break;
            }
                

            RPC_SpawnCastleTransformSync(NO, tag, team);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SpawnCastleTransformSync(NetworkObject NO, string tag, Team team)
        {
            NO.gameObject.tag = tag;
            NO.transform.position = transform.parent.position;
            NO.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            NO.gameObject.GetComponent<Castle>().SliderInit();
            NO.gameObject.GetComponent<Castle>().team = team;
        }
    }
}

