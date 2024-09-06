using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBuildArea : MonoBehaviour
    {
        public bool hasCastle = false;
        public bool startDelay = false; // 5�ʰ��� �� ��ȯ���� ���ϰ� �ϴ� ����

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
                StopAllCoroutines(); // ���� ���� ���� ��� �ڷ�ƾ�� �ߴ�
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
            yield return new WaitForSeconds(1); // 1�� ��ٸ�
            hasCastle = false; // hasCastle�� false�� ����
        }
    }
}



