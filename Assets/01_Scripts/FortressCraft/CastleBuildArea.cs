using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBuildArea : MonoBehaviour
    {
        public void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = true;
            }
        }
        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = false;
            }
        }
    }
}



