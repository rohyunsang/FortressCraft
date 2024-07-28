using Fusion;
<<<<<<< HEAD
using FusionHelpers;
=======
>>>>>>> Seong_0.01
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft{

<<<<<<< HEAD
    public class Castle : MonoBehaviour
    {
        public float CurrentHP { get; private set; }
        public bool IsDestroyed { get; private set; }
=======
    public enum Team{
        A, B, C, D
    }

    public class Castle : NetworkBehaviour
    {
        [Networked] public float CurrentHP { get; set; } 

        private float maxHP = 100f; //* �ִ� ü��
        
        private ChangeDetector changes;

>>>>>>> Seong_0.01
        public Slider HpBarSlider;

        public Team team;

<<<<<<< HEAD
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
                manager.UpdateCastleHP(team, damage);
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            // PlayerWeapon ������Ʈ�� �õ��Ͽ� ��������, ������ ������ ó��
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon))
=======

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
                Debug.Log("ü�� 0 ���� ����!");
            }
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
>>>>>>> Seong_0.01
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

