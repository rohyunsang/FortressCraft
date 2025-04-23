using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class MonsterHPBar : NetworkBehaviour
    {
        [Networked] public float HP { get; set; }
        [SerializeField] private Slider HpBarSlider;
        [SerializeField] private MonsterController controller;

        public void SetHPBar(float hp)
        {
            HP = hp;
            HpBarSlider.value = HP / controller.hpMax;
        }
    }
}


