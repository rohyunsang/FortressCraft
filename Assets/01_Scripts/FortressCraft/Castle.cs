using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft{

    public class Castle : MonoBehaviour
    {
        public float CurrentHP { get; private set; }
        public bool IsDestroyed { get; private set; }
        public Slider HpBarSlider;

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
                manager.UpdateCastleHP(team, damage);
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
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

