using Fusion;
using FusionHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft{

    public class Castle : NetworkBehaviour
    {
        public float CurrentHP { get; private set; }
        public bool IsDestroyed { get; private set; }
        public Slider HpBarSlider;

        public Team team;



        public void Init(float currntHP)
        {
            CurrentHP = currntHP;
            IsDestroyed = false;
        }
        public void SliderInit()
        {
            HpBarSlider.gameObject.SetActive(true);
            HpBarSlider.value = 1;
        }

        
    }
}

