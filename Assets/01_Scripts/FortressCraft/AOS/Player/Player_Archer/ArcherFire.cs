using Fusion;
using FusionHelpers;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using TMPro;

namespace Agit.FortressCraft
{
	public class ArcherFire : NetworkBehaviour
    {
        [SerializeField] private NetworkObject arrow;
        public Vector2 FireDirection { get; set; }
        public string OwnType { get; set; }
        [SerializeField] private float damage = 0.0f;

        public int skill2ArrowCount = 36;

        private Vector3 offsetVector = new Vector3(0.0f, 0.1f, 0.0f);
        private NetworkPrefabId id;
        private NetworkObjectPoolManager poolManager;

        public override void Spawned()
        {
            poolManager = NetworkObjectPoolManager.Instance;
            NetworkObject temp = Runner.Spawn(arrow, (Vector2)transform.position, Quaternion.identity);
            id = temp.NetworkTypeId.AsPrefabId;
            Destroy(temp.gameObject);
            poolManager.AddPoolTable(id);
            /*
            int prePoolingCount = 30;

            NetworkObject[] arrows = new NetworkObject[50];

            for( int i = 0; i < prePoolingCount; ++ i )
            {
                NetworkObject no = null;
                NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
                var result = poolManager.AcquirePrefabInstance(Runner, context, out no);
                no.GetComponent<ArcherArrow>().ID = id;
                arrows[i] = no;
            }
            for (int i = 0; i < prePoolingCount; ++i)
            {
                arrows[i].GetComponent<ArcherArrow>().Release();
            }
            */
        }

        // 관통형 화살 1개 발사 
        public void FireArrow()
        {
            if (OwnType == null) return;
            NetworkObject no = null;
            NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
            var result = poolManager.AcquirePrefabInstance(Runner, context, out no);

            if( result == NetworkObjectAcquireResult.Success )
            {
                no.transform.SetParent(null);
                ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();
                
                archerArrow.FireDirection = FireDirection;
                archerArrow.ID = id;
                RPCSetActive(archerArrow, transform.position + offsetVector);
                archerArrow.ReserveRelease();

                ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
                arrowAttackCollider.Damage = damage;
                arrowAttackCollider.OwnType = OwnType;
            }
        }

        // 관통형 화살 n개 연사
        public void FireSkill1()
        {
            //Debug.Log("Damage: " + damage);
            // Debug.Log("Skill1");
            if (OwnType == null) return;
            for ( int i = 1; i < 11; ++i )
            {
                NetworkObject no = null;
                NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
                var result = poolManager.AcquirePrefabInstance(Runner, context, out no);
                //NetworkObject no = Runner.Spawn(arrow, transform.position + offsetVector, Quaternion.identity);
                no.transform.SetParent(null);

                if (result == NetworkObjectAcquireResult.Success)
                {
                    no.transform.SetParent(null);
                    ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();

                    archerArrow.FireDirection = new Vector2(FireDirection.x * i / 2.0f, FireDirection.y * i / 2.0f);
                    archerArrow.ID = id;
                    RPCSetActive(archerArrow, transform.position + offsetVector);
                    archerArrow.ReserveRelease();

                    ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
                    arrowAttackCollider.Damage = damage / 1.4f;
                    arrowAttackCollider.OwnType = OwnType;
                }
            }
        }

        // 모든 방향으로 36개 화살 난사
        public void FireSkill2()
        {
            if (OwnType == null) return;
            float offset = 360.0f / (float)skill2ArrowCount;
            for( int i = 0; i < skill2ArrowCount; ++i )
            {
                NetworkObject no = null;
                NetworkPrefabAcquireContext context = new NetworkPrefabAcquireContext(id);
                var result = poolManager.AcquirePrefabInstance(Runner, context, out no);
                //NetworkObject no = Runner.Spawn(arrow, transform.position + offsetVector, Quaternion.identity);
                no.transform.SetParent(null);
                ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();

                archerArrow.FireDirection = new Vector2(Mathf.Cos(i * offset) / 1.5f, Mathf.Sin(i * offset) / 1.5f);
                archerArrow.ID = id;
                RPCSetActive(archerArrow, transform.position + offsetVector);
                archerArrow.ReserveRelease();

                ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
                arrowAttackCollider.Damage = damage / 4.0f;
                arrowAttackCollider.OwnType = OwnType;
            }
        }

        public int PlayerLevel { get; set; }

        public void SetDamageByLevel(int level, JobType jobType)
        {
            int offset = 0;

            switch (jobType)
            {
                case JobType.Warrior:
                    offset = -1;
                    break;
                case JobType.Magician:
                    offset = 14;
                    break;
                case JobType.Archer:
                    offset = 29;
                    break;
            }

            damage = GoogleSheetManager.commanderDatas[level + offset].Attack;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSetActive(ArcherArrow arrow, Vector3 pos)
        {
            arrow.gameObject.SetActive(true);
            arrow.gameObject.GetComponent<Rigidbody2D>().transform.position = pos;
        }
    }
}