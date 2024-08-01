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
    }
}