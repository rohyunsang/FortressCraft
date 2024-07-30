using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class AttackCollider : MonoBehaviour
    {
        public float Damage { get; set; }
        public string TargetUnit { get; set; }
        public string OwnType { get; set; }
    }
}