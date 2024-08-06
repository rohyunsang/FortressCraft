using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        [Networked] public float DestroyCnt {  get; set; }

        public Castle A_Castle;
        public Castle B_Castle;
        public Castle C_Castle;
        public Castle D_Castle;

        private float MaxHP = 1500.0f;

        private ChangeDetector changes;

        public int team_id { get; set; }

        public override void Spawned()
        {
            base.Spawned();

            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (HasStateAuthority)
            {
                A_CurrentHP = MaxHP;
                B_CurrentHP = MaxHP;
                C_CurrentHP = MaxHP;
                D_CurrentHP = MaxHP;
                
                DestroyCnt = 0;

                A_Castle.Init(A_CurrentHP);
                B_Castle.Init(B_CurrentHP);
                C_Castle.Init(C_CurrentHP);
                D_Castle.Init(D_CurrentHP);
            }
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
                    case nameof(DestroyCnt):
                        if (DestroyCnt >= 3)
                            CheckWinner();
                        break;
                }
            }
            // Debug.Log("DestroyCnt" + DestroyCnt);
        }

        public void CheckWinner()
        {
            if (A_CurrentHP > 0)
                team_id = 1;
            if (B_CurrentHP > 0)
                team_id = 2;
            if (C_CurrentHP > 0)
                team_id = 3;
            if (D_CurrentHP > 0)
                team_id = 4;

            if (Runner.TryGetSingleton(out GameManager gameManager))
            {
                gameManager.GetWinnerPlayerRef(team_id.ToString());
            }
        }

        public void UpdateCastleHP(Team team, float damage)
        {
            switch (team)
            {
                case Team.A: A_CurrentHP -= damage;
                    if (A_CurrentHP <= 0)
                    {
                        if (Runner.TryGetSingleton(out GameManager gameManager))
                        {
                            team_id = 1;
                            DestroyCnt++;
                            gameManager.GetDestroyCastlePlayerRef(team_id.ToString());
                        }
                    }
                    break;
                case Team.B: B_CurrentHP -= damage;
                    if (B_CurrentHP <= 0)
                    {
                        if (Runner.TryGetSingleton(out GameManager gameManager))
                        {
                            team_id = 2;
                            DestroyCnt++;

                            gameManager.GetDestroyCastlePlayerRef(team_id.ToString());
                        }
                    }
                    break;
                case Team.C: C_CurrentHP -= damage;
                    if (C_CurrentHP <= 0)
                    {
                        if (Runner.TryGetSingleton(out GameManager gameManager))
                        {
                            team_id = 3;
                            DestroyCnt++;

                            gameManager.GetDestroyCastlePlayerRef(team_id.ToString());
                        }
                    }
                    break;
                case Team.D: D_CurrentHP -= damage;
                    if (D_CurrentHP <= 0)
                    {
                        if (Runner.TryGetSingleton(out GameManager gameManager))
                        {
                            team_id = 4;
                            DestroyCnt++;

                            gameManager.GetDestroyCastlePlayerRef(team_id.ToString());
                        }
                    }
                    break;
            }
        }

        private void SyncHp(float currentHp, Castle castle)
        {
            if (castle != null && castle.HpBarSlider != null)
            {
                castle.HpBarSlider.value = currentHp / MaxHP;
                Debug.Log(currentHp);
            }
        }
    }
}