using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft{

    /*public enum Team{
        A, B, C, D
    }*/

    public class TestUniy : NetworkBehaviour
    {
        [Networked] public float CurrentHP { get; set; } 

        private float maxHP = 100f; //* �ִ� ü��
        
        private ChangeDetector changes;

        //public Slider HpBarSlider;

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
            //AsycHp(); //* ü�� ����
            if (CurrentHP <= 0)
            {
                Debug.Log("Die!");
                gameObject.SetActive(false);
            }
        }

       /* public override void Render()
        {

            base.Render();

            foreach (var change in changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(CurrentHP):
                        //AsycHp();
                        break;
                }
            }
        }

        public void AsycHp() //*HP ����
        {
            if (HpBarSlider != null)
                HpBarSlider.value = CurrentHP / maxHP;
        }
        */

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                return;
            }
            Debug.Log("Trigger");

            // PlayerWeapon ������Ʈ�� �õ��Ͽ� ��������, ������ ������ ó��
            // 플레이어와 같은 팀(태그)를 가졌을 경우 데미지가 들어가지 않게 해야함.
            // 네트워크 단에서 어떻게 팀 태그를 지정해주는지가 선행되어야 할듯.
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon))
            {
                Damage(weapon.damage);
            }
            else
            {

            }
        }
    }
}

