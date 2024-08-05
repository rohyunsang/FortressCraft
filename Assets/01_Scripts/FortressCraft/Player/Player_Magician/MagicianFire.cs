using Fusion;
using FusionHelpers;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using TMPro;

namespace Agit.FortressCraft
{
	public class MagicianFire : NetworkBehaviour
    {
        [SerializeField] private NetworkObject spell;
        public Vector2 FireDirection { get; set; }
        public string OwnType { get; set; }
        [SerializeField] private float damage = 0.0f;

        public override void Spawned()
        {
            OwnType = null;
        }

        // 추적탄 1개 발사 
        public void FireSpell()
        {
            //Debug.Log("Damage: " + damage);
            if( OwnType == null ) return;
            
            Debug.Log("Fire!");
            NetworkObject no = Runner.Spawn(spell, transform.position, Quaternion.identity);
            no.transform.SetParent(null);

            MagicianSpell magicianSpell = no.GetComponent<MagicianSpell>();
            magicianSpell.SpellSpeed = 3.0f;

            MagicianSpellAttackCollider magicAttackCollider = no.GetComponent<MagicianSpellAttackCollider>();
            magicAttackCollider.Damage = damage;
            magicAttackCollider.OwnType = OwnType;
        }

        public void FireSkill1()
        {
            if (OwnType == null) return;
        }

        public void FireSkill2()
        {
            if (OwnType == null) return;
        }

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