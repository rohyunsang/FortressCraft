using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{

    public class Castle : NetworkBehaviour
    {
        /*[Networked] */public float CurrentHP { get; set; } 
        public bool IsDestroyed { get; private set; }
        public Slider HpBarSlider;

        private float maxHP = 100f; //* �ִ� ü��
        
        private ChangeDetector changes;

        public enum Team
        {
            A, B, C, D
        }
        public Team team;

        public void Init(float currntHP)
        {
            CurrentHP = currntHP;
            IsDestroyed = false;
        }

        public void Damage(float damage)
        {
            if (IsDestroyed || CurrentHP <= 0) return;
            CurrentHP -= damage;

            // Update HP in CastleManager
            var manager = FindObjectOfType<CastleManager>();
            if (manager != null)
            {
                //manager.UpdateCastleHP(team, damage);
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CurrentHP = maxHP;
        }

        public override void Render()
        {

            base.Render();

            foreach (var change in changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(CurrentHP):
                        AsycHp();
                        break;
                }
            }
        }

        public void AsycHp() //*HP ����
        {
            if (HpBarSlider != null)
                HpBarSlider.value = CurrentHP / maxHP;
        }


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(team.ToString()))
            {
                return;
            }
            Debug.Log("Trigger");
            //성의 Team Tag와 플레이어의 Tag가 같을 때 데미지 들어가면 안된다.
            
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon) && !gameObject.CompareTag("A")) 
            // 임시로 Tag A를 넣어보자

            {
                Damage(weapon.damage);
            }
            else
            {
                Debug.Log("This is Not PlayerWeapon");
            }
        }
    }
}

