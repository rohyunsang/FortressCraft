using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class PlayerHPBar : MonoBehaviour
    {
        public float HP { get; set; }
        public Slider HpBarSlider;
        private Player player;

        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }

        private void FixedUpdate()
        {
            HP = player.life;
            HpBarSlider.value = HP / getMaxHPByLevel(player.level, player.Job);
        }

        public float getMaxHPByLevel(int level, JobType jobType)
        {
            return GoogleSheetManager.GetCommanderData(level, jobType).HP;
        }
    }
}


