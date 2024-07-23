using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft{

    public enum Team{
        A, B, C, D
    }

    public class Castle : NetworkBehaviour
    {
        [Networked] public float CurrentHP { get; set; } 

        private float maxHP = 100f; //* 최대 체력
        
        private ChangeDetector changes;

        public Slider HpBarSlider;

        public Team team;

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CurrentHP = maxHP;
        }

        public void Damage(float damage) //* 데미지 받는 함수
        {
            if (maxHP == 0 || CurrentHP <= 0) //* 이미 체력 0이하면 패스
                return;
            CurrentHP -= damage;
            AsycHp(); //* 체력 갱신
            if (CurrentHP <= 0)
            {
                
                DestroyCastle();
            }
        }

        private void DestroyCastle()
        {
            Debug.Log("Castle Destroyed!");

            //if (Object.HasInputAuthority) // 현재 클라이언트가 입력 권한을 가진 객체인지 확인
            //{

            FindObjectOfType<UIManager>().OnDefeatPanel();
            //}

            // UI Call : Defeat Panel -> OnClilk -> 
            // Delete Input Auth
            // Delete Playter   
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

        public void AsycHp() //*HP 갱신
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

            // PlayerWeapon 컴포넌트를 시도하여 가져오고, 있으면 데미지 처리
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon))
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

