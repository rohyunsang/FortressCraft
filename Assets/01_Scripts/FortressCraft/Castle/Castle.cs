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

        private bool invincibilityTime = true;

        public Team team;

        void Start()
        {
            Invoke("EndInvincibility", 5f);
        }

        public void Init(float currntHP)
        {
            CurrentHP = currntHP;
            IsDestroyed = false;
        }
        public void SliderInit()
        {
            HpBarSlider.gameObject.SetActive(true);
        }


        public void Damage(float damage)
        {
            if (invincibilityTime) return;

            if (IsDestroyed || CurrentHP <= 0) return;
            CurrentHP -= damage;

            // Update HP in CastleManager
            var manager = GetComponent<CastleManager>(); 

            if (manager != null)
            {
                manager.UpdateCastleHP(damage);
            }
        }

        private void EndInvincibility()
        {
            invincibilityTime = false;
        }
    }
}

