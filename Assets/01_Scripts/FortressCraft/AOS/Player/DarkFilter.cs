using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class DarkFilter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer filter = null;

        public void OnFilter()
        {
            filter.enabled = true;
        }

        public void OffFilter()
        {
            filter.enabled = false;
        }
    }
}


