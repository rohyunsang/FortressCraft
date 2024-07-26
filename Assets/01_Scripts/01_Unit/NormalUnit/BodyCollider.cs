using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class BodyCollider : MonoBehaviour
    {
        public float Damaged { get; set; }

        private void Awake()
        {
            Damaged = 0.0f;
        }
    }
}

