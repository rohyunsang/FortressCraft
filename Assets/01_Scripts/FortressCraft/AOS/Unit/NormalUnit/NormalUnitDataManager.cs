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
        public float AttackDelay { get; set; }
        public float Scale { get; set; }

        private void Awake()
        {
            Instance = this;

            Attack = 20.0f;
            Defense = 1.0f;
            SpawnTime = 1.0f;
            Speed = 1.5f;
            AttackDelay = 2.0f;
            Scale = 0.35f;
        }
    }
}


