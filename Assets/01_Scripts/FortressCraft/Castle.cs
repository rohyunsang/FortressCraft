using Fusion;
using FusionHelpers;
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

        [Networked] public bool isDefeated { get; set; }
        [Networked] public bool isVictory { get; set; }

        public override void Spawned()
        {
            base.Spawned();
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CurrentHP = maxHP;
            isDefeated = false;
            isVictory = false;
        }

        public void Damage(float damage) //* ������ �޴� �Լ�
        {
            if (maxHP == 0 || CurrentHP <= 0) //* �̹� ü�� 0���ϸ� �н�
                return;
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                isDefeated = true;
            }
        }

        private void DestroyCastle()
        {
            if (isDefeated)
            {
                FindObjectOfType<UIManager>().OnDefeatPanel();
            }
        }

        private void Victroy()
        {
            if (isVictory)
            {
                FindObjectOfType<UIManager>().OnVictoryPanel();
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
                    case nameof(isDefeated):
                        DestroyCastle();
                        break;
                    case nameof(isVictory):
                        Victroy();
                        break;
                }
            }
        }

        public void AsycHp() //*HP ����
        {
            Debug.Log(CurrentHP + " " + team.ToString());

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

