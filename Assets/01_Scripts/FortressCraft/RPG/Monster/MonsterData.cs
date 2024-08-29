using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    [CreateAssetMenu( fileName = "MonsterData", menuName = "Scriptable Object/Monster Data" )]
    public class MonsterData : ScriptableObject
    {
        [SerializeField] private float hpMax = 3000;
        public float HPMax { get { return hpMax; } }
        [SerializeField] private float movingWeight = 1.0f;
        public float MovingWeight { get { return movingWeight; } }
        [SerializeField] private float damage = 10.0f;
        public float Damage { get { return damage; } }
    }
}


