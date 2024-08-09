using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;


namespace Agit.FortressCraft
{
    public enum Team
    {
        A, B, C, D
    }
    public class CastleManager : NetworkBehaviour
    {
        [Networked] public float CurrentHP { get; set; }

        public Castle castle;

        private float MaxHP = 100f;

        private ChangeDetector changes;

        public int team_id { get; set; }

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (HasStateAuthority)
            {
                CurrentHP = MaxHP;

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

        public void UpdateCastleHP(float damage)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
                {
                    gameManager.GetDestroyCastleOwnerPlayer(gameObject.tag);
                }
                Destroy(gameObject);
            }
        }

        private void SyncHp(float currentHp, Castle castle)
        {
            if (castle != null && castle.HpBarSlider != null)
            {
                castle.HpBarSlider.value = currentHp / MaxHP;
            }
        }
    }
}