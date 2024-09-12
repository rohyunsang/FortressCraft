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
        public float SpawnTime { get; set; }
        public float Speed { get; set; }

        private void Awake()
        {
            Instance = this;

            Attack = 20.0f;
            Defense = 1.0f;
            SpawnTime = 5.0f;
            Speed = 1.5f;
        }
    }
}


