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

        private float maxHP = 100f; //* �ִ� ü��
        
        private ChangeDetector changes;

        public Slider HpBarSlider;

        public Team team;

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CurrentHP = maxHP;
        }

        public void Damage(float damage) //* ������ �޴� �Լ�
        {
            if (maxHP == 0 || CurrentHP <= 0) //* �̹� ü�� 0���ϸ� �н�
                return;
            CurrentHP -= damage;
            AsycHp(); //* ü�� ����
            if (CurrentHP <= 0)
            {
                
                DestroyCastle();
            }
        }

        private void DestroyCastle()
        {
            Debug.Log("Castle Destroyed!");

            //if (Object.HasInputAuthority) // ���� Ŭ���̾�Ʈ�� �Է� ������ ���� ��ü���� Ȯ��
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

            // PlayerWeapon ������Ʈ�� �õ��Ͽ� ��������, ������ ������ ó��
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

