using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class NormalUnitDataManager : MonoBehaviour
    {
        public static NormalUnitDataManager Instance;

        public float Attack { get; set; }
        public float Defense { get; set; }
        public float spawnTime { get; set; }

        private void Awake()
        {
            Instance = this;

            Attack = 20.0f;
            Defense = 1.0f;
            spawnTime = 5.0f;
        }
    }
}


