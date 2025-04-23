using Agit.FortressCraft;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class CastleHpBar : NetworkBehaviour
    {
        [Networked] public float HP { get; set; }
        public Slider HpBarSlider;
        private Castle castle;

        public override void Spawned()
        {
            castle = GetComponent<Castle>();
        }

        public void SetHPBar(float currentHP)
        {
            HP = currentHP;
            HpBarSlider.value = HP / 1000f;
        }

        public float getMaxHPByLevel(int level, JobType jobType)
        {
            return GoogleSheetManager.GetCommanderData(level, jobType).HP;
        }
    }

}
