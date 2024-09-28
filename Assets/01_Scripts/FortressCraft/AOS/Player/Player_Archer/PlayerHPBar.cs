using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using FusionHelpers;

namespace Agit.FortressCraft
{
    public class PlayerHPBar : NetworkBehaviour
    {
        [Networked] public float HP { get; set; }
        public Slider HpBarSlider;
        private Player player;

        public override void Spawned()
        {
            player = gameObject.GetComponent<Player>();
        }

        public void SetHPBar(float life)
        {
            HP = life;
            HpBarSlider.value = HP / getMaxHPByLevel(player.level, player.Job);
        }

        public float getMaxHPByLevel(int level, JobType jobType)
        {
            return GoogleSheetManager.GetCommanderData(level, jobType).HP;
        }
    }
}


