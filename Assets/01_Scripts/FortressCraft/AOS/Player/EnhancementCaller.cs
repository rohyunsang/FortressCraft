using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class EnhancementCaller : MonoBehaviour
    {
        private Vector3 orgPos = new Vector3(0.0f, 355.0f, 0.0f);
        private Vector3 castAwayPos = new Vector3(-2000.0f, -2000.0f, 0.0f);

        public void ActiveSelf()
        {
            this.gameObject.SetActive(true);
        }

        public void SetAsOrgPos()
        {
            transform.localPosition = orgPos;
        }

        public void castAway()
        {
            transform.position = castAwayPos;
        }
    }
}


