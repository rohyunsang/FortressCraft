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

        private float MaxHP = 1000f;

        private ChangeDetector changes;

        [SerializeField] private CastleHpBar castleHpBar;

        [SerializeField] private CastleBodyCollider bodyCollider;

        public bool isDestroyCastle = false;

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
                        castleHpBar.SetHPBar(CurrentHP);
                        UpdateCastleHP();
                        break;
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCCheckDamaged()
        {
            if (!HasStateAuthority) return;
            CheckDamaged();
        }

        public void CheckDamaged()
        {
            // if (IsDestroyed) return;

            if (bodyCollider.Damaged > 0.0f)
            {
                CurrentHP -= bodyCollider.Damaged;

                bodyCollider.Damaged = 0.0f;
            }
        }

        public void UpdateCastleHP()
        {
            if (this.CurrentHP <= 0 && !isDestroyCastle)
            {
                isDestroyCastle = true;
                if (Runner.TryGetSingleton<GameManager>(out GameManager gameManager))
                {
                    gameManager.GetDestroyCastleOwnerPlayer(gameObject.tag);
                }
                Destroy(gameObject);
            }
        }
    }       
}