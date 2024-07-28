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

        private float maxHP = 100f; //* ï¿½Ö´ï¿½ Ã¼ï¿½ï¿½
        
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
            // PlayerWeapon ÄÄÆ÷³ÍÆ®¸¦ ½ÃµµÇÏ¿© °¡Á®¿À°í, ÀÖÀ¸¸é µ¥¹ÌÁö Ã³¸®
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon))
=======

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CurrentHP = maxHP;
        }

        public void Damage(float damage) //* ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Þ´ï¿½ ï¿½Ô¼ï¿½
        {
            if (maxHP == 0 || CurrentHP <= 0) //* ï¿½Ì¹ï¿½ Ã¼ï¿½ï¿½ 0ï¿½ï¿½ï¿½Ï¸ï¿½ ï¿½Ð½ï¿½
                return;
            CurrentHP -= damage;
            AsycHp(); //* Ã¼ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
            if (CurrentHP <= 0)
            {
                Debug.Log("Ã¼ï¿½ï¿½ 0 ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½!");
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

        public void AsycHp() //*HP ï¿½ï¿½ï¿½ï¿½
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
            //ì„±ì˜ Team Tagì™€ í”Œë ˆì´ì–´ì˜ Tagê°€ ê°™ì„ ë•Œ ë°ë¯¸ì§€ ë“¤ì–´ê°€ë©´ ì•ˆëœë‹¤.
            
            if (collision.TryGetComponent<PlayerWeapon>(out PlayerWeapon weapon) && !gameObject.CompareTag("A")) 
            // ìž„ì‹œë¡œ Tag Aë¥¼ ë„£ì–´ë³´ìž
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

