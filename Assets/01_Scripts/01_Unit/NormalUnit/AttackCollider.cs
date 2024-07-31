using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class AttackCollider : NetworkBehaviour
    {
        public float Damage { get; set; }
        public string TargetUnit { get; set; }
        public string OwnType { get; set; }
    }
}