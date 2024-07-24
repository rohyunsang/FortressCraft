using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Agit.FortressCraft.CastleManager;
using static Fusion.NetworkBehaviour;


namespace Agit.FortressCraft
{
    public enum Team
    {
        A, B, C, D
    }
    public class CastleManager : NetworkBehaviour
    {
        [Networked] public float A_CurrentHP { get; set; }
        [Networked] public float B_CurrentHP { get; set; }
        [Networked] public float C_CurrentHP { get; set; }
        [Networked] public float D_CurrentHP { get; set; }

        public Castle A_Castle;
        public Castle B_Castle;
        public Castle C_Castle;
        public Castle D_Castle;

        private ChangeDetector changes;
        public override void Spawned()
        {
            base.Spawned();
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            base.Render();

            foreach (var change in changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(A_CurrentHP):
                        SyncHp(A_CurrentHP, A_Castle);
                        break;
                    case nameof(B_CurrentHP):
                        SyncHp(B_CurrentHP, B_Castle);
                        break;
                    case nameof(C_CurrentHP):
                        SyncHp(C_CurrentHP, C_Castle);
                        break;
                    case nameof(D_CurrentHP):
                        SyncHp(D_CurrentHP, D_Castle);
                        break;
                }
            }
        }
        public void UpdateCastleHP(Team team, float hp)
        {
            switch (team)
            {
                case Team.A: A_CurrentHP = hp; break;
                case Team.B: B_CurrentHP = hp; break;
                case Team.C: C_CurrentHP = hp; break;
                case Team.D: D_CurrentHP = hp; break;
            }
        }

        private void SyncHp(float currentHp, Castle castle)
        {
            if (castle != null && castle.HpBarSlider != null)
            {
                castle.HpBarSlider.value = currentHp / castle.maxHP;
            }
        }
    }
}