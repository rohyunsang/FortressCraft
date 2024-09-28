using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public enum MonsterType
    {
        NORMAL,
        SWORD,
        BOW,
        MAGIC,
        NON
    }

    public enum BuffType
    {
        NON,
        ATTACK,
        DEFENSE
    }

    [CreateAssetMenu( fileName = "MonsterData", menuName = "Scriptable Object/Monster Data" )]
    public class MonsterData : ScriptableObject
    {
        [SerializeField] private float hpMax = 3000;
        public float HPMax { get { return hpMax; } }
        [SerializeField] private float movingWeight = 1.0f;
        public float MovingWeight { get { return movingWeight; } }
        [SerializeField] private float damage = 10.0f;
        public float Damage { get { return damage; } }

        [SerializeField] private float idleDelay= 1.0f;
        public float IdleDelay { get { return idleDelay; } }
        [SerializeField] private float runDelay = 1.0f;
        public float RunDelay { get { return runDelay; } }
        [SerializeField] private float attackDelay = 1.0f;
        public float AttackDelay { get { return attackDelay; } }

        [SerializeField] private int gold = 10;
        public int Gold { get { return gold; } }
        [SerializeField] private float exp = 10.0f;
        public float Exp { get { return exp; } }

        [SerializeField] private MonsterType type = MonsterType.NON;
        public MonsterType Type { get { return type; } }

        [SerializeField] private BuffType buff = BuffType.NON;
        public BuffType Buff { get { return buff; } }
    }
}


