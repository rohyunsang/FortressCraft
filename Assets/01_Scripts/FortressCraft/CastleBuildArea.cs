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
                Debug.Log("성 지을수 있어요");
                other.transform.parent.GetComponent<Player>().isBuildCastle = true;
                Debug.Log(other.transform.parent.GetComponent<Player>().isBuildCastle);
            }
        }
        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("성 지을수 없어요");
                other.transform.parent.GetComponent<Player>().isBuildCastle = false;
                Debug.Log(other.transform.parent.GetComponent<Player>().isBuildCastle);
            }
        }
    }
}



