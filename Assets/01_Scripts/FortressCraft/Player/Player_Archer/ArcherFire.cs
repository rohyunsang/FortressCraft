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

        // 관통형 화살 1개 발사 
        public void FireArrow()
        {
            //Debug.Log("Damage: " + damage);
            //Debug.Log("Fire!");
            NetworkObject no = Runner.Spawn(arrow, transform.position, Quaternion.identity);
            no.transform.SetParent(null);

            ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();
            archerArrow.FireDirection = FireDirection;

            ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
            arrowAttackCollider.Damage = damage;
            arrowAttackCollider.OwnType = OwnType;
        }

        // 관통형 화살 n개 연사
        public void FireSkill1()
        {
            //Debug.Log("Damage: " + damage);
            // Debug.Log("Skill1");
            for( int i = 1; i < 11; ++i )
            {
                NetworkObject no = Runner.Spawn(arrow, transform.position, Quaternion.identity);
                no.transform.SetParent(null);
                no.transform.localScale = new Vector3(no.transform.localScale.x * 1.5f,
                                                        no.transform.localScale.y * 1.5f,
                                                        no.transform.localScale.z);

                ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();
                archerArrow.FireDirection = new Vector2( FireDirection.x * i / 2.0f , FireDirection.y * i / 2.0f );

                ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
                arrowAttackCollider.Damage = damage / 2.0f;
                arrowAttackCollider.OwnType = OwnType;
            }
        }

        // 모든 방향으로 36개 화살 난사
        public void FireSkill2()
        {
            //Debug.Log("Damage: " + damage);
            //Debug.Log("Skill2");
            float offset = 360.0f / (float)skill2ArrowCount;
            for( int i = 0; i < skill2ArrowCount; ++i )
            {
                NetworkObject no = Runner.Spawn(arrow, transform.position, Quaternion.identity);
                no.transform.SetParent(null);

                ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();
                archerArrow.FireDirection = new Vector2(Mathf.Cos(i*offset) / 1.5f, Mathf.Sin(i*offset) / 1.5f);

                ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
                arrowAttackCollider.Damage = damage;
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
    }
}