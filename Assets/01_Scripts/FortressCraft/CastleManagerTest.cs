using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Agit.FortressCraft
{
    public class CastleManagerTest : NetworkBehaviour
    {
        [Networked] public float CurrentHP { get; set; }

        public Castle castle;

        private ChangeDetector changes;
        public int team_id { get; set; }

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (HasStateAuthority)
            {
                CurrentHP = 50f;

                castle.Init(CurrentHP);
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
                        SyncHp(CurrentHP, castle);
                        break;
                }
            }
        }

        public void UpdateCastleHP(Team team, float damage)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                Debug.Log("´ÔÁ×À½");
            }
        }

        private void SyncHp(float currentHp, Castle castle)
        {
            if (castle != null && castle.HpBarSlider != null)
            {
                castle.HpBarSlider.value = currentHp / 50f;
            }
        }
    }
}