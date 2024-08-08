using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBuildArea : MonoBehaviour
    {
        public bool hasCastle = false;
        public bool startDelay = false; // 5초간은 성 소환하지 못하게 하는 변수

        void Start ()
        {
            Invoke("CanSpawnTime", 5f);
        }
        
        private void CanSpawnTime()
        {
            startDelay = true;
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            //Debug.Log(other.gameObject.tag);

            if (other.transform.parent != null && other.transform.parent.CompareTag("Player") && !hasCastle && startDelay)
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = true;
            }

            if (other.gameObject.name.Contains("Castle"))
            {
                hasCastle = true;
                StopAllCoroutines(); // 현재 실행 중인 모든 코루틴을 중단
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = false;
            }
            if (other.gameObject.name.Contains("Castle"))
            {
                StartCoroutine(SetHasCastleFalseAfterDelay());
            }
        }
        private IEnumerator SetHasCastleFalseAfterDelay()
        {
            yield return new WaitForSeconds(1); // 1초 기다림
            hasCastle = false; // hasCastle을 false로 설정
        }
    }
}



