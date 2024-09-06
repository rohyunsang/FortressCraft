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
        
        public void SpawnCastleObject(Player player)
        {
            if (!HasStateAuthority) return;

            NetworkObject NO = Runner.Spawn(castle, Vector3.zero, Quaternion.identity);
            
            string tag = "";
            tag = "A"; // Default
            Team team = Team.A;
            if (FindObjectOfType<App>().mode == Mode.Survival)
            {
                int idx = 0;
                if (Runner.TryGetSingleton(out GameManager gameManager))
                {
                    idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);
                }
                
                team = Team.A; // Default
                switch (idx)
                {
                    case 0:
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
            }
            else
            {
                if (player.team == Team.A)
                {
                    tag = "A";
                    team = Team.A;
                }
                else if (player.team == Team.B)
                {
                    tag = "B";
                    team = Team.B;
                }
            }
            
            RPC_SpawnCastleTransformSync(NO, tag, team, player);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
        public void RPC_SpawnCastleTransformSync(NetworkObject NO, string tag, Team team, Player player)
        {
            NO.gameObject.tag = tag;

            Transform[] transforms = FindObjectOfType<CastleBuildAreaManager>().castleBuildAreaTransforms;

            Transform closestTransform = null;
            float closestDistance = Mathf.Infinity;
            Vector3 parentPosition = transform.parent.position;

            foreach (Transform t in transforms)
            {
                float distance = Vector3.Distance(parentPosition, t.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = t;
                }
            }

            if (closestTransform != null)
            {
                NO.transform.position = closestTransform.position + new Vector3(0f,0.26f,0f);    // origin 0.59, change 0.85 -> 0.26
            }
            NO.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            NO.gameObject.GetComponent<Castle>().SliderInit();
            NO.gameObject.GetComponent<Castle>().team = team;

            NormalUnitSpawner spawner = NO.GetComponentInChildren<NormalUnitSpawner>();
            spawner.transform.tag = "Unit_" + tag;
            spawner.SpawnerType = tag;

        }
    }
}

