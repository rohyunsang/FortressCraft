using Fusion;
using FusionHelpers;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class SpawnCastle : NetworkBehaviour
    {
        [SerializeField] private NetworkObject castle;

    public NormalUnitSpawner SpawnCastleObject(Player player)
        {
            if (!HasStateAuthority) return null;

            NetworkObject NO = Runner.Spawn(castle, Vector3.zero, Quaternion.identity);

            string tag = "";
            tag = "A"; // Default
            Team team = Team.A;
            if (Runner.TryGetSingleton(out GameManager gameManager))
            {
                if (gameManager.mode != Mode.Team)
                {
                    int idx = 0;
                    idx = gameManager.TryGetPlayerId(Runner.LocalPlayer);

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
                    string playerTeam = gameManager.TryGetPlayerTeam(Runner.LocalPlayer);
                    if (playerTeam == Team.A.ToString())
                    {
                        tag = "A";
                        team = Team.A;
                    }
                    else if (playerTeam == Team.B.ToString())
                    {
                        tag = "B";
                        team = Team.B;
                    }
                }

                RPC_SpawnCastleTransformSync(NO, tag, team, player);
            }
            
            return NO.GetComponent<NormalUnitSpawner>();
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
                NO.transform.position = closestTransform.position + new Vector3(0f, 0.26f, 0f);    // origin 0.59, change 0.85 -> 0.26
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

