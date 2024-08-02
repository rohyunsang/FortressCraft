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
        [SerializeField] private float damage = 100.0f;

        private Vector2[] skill1Vecs;
        private int skill1VecsLength = 20;


        public override void Spawned()
        {
            skill1Vecs = new Vector2[skill1VecsLength];
            for( int i = 0; i < skill1VecsLength; ++i )
            {
                skill1Vecs[i] = new Vector2(-skill1VecsLength + i, skill1VecsLength - i);
            }
            
        }

        // 관통형 화살 1개 발사 
        public void FireArrow()
        {
            //Debug.Log("Fire!");
            NetworkObject no = Runner.Spawn(arrow, transform.position, Quaternion.identity);
            no.transform.SetParent(null);

            ArcherArrow archerArrow = no.GetComponent<ArcherArrow>();
            archerArrow.FireDirection = FireDirection;

            ArcherArrowAttackCollider arrowAttackCollider = no.GetComponent<ArcherArrowAttackCollider>();
            arrowAttackCollider.Damage = damage;
            arrowAttackCollider.OwnType = OwnType;
        }

        // 관통형 화살 10개 연사
        public void FireSkill1()
        {
            // Debug.Log("Skill1");
            for( int i = 1; i < 10; ++i )
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
    }
}