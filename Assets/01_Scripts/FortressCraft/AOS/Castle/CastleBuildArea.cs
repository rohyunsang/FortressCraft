using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class CastleBuildArea : MonoBehaviour
    {
        public bool hasCastle = false;
        public bool startDelay = false; // 5�ʰ��� �� ��ȯ���� ���ϰ� �ϴ� ����

        private const string PLAYER_TAG = "Player";  // 새로 추가됨
        private const string CASTLE_TAG = "Castle";  // 새로 추가됨

        private float spawnDelayTimer = 5f;   // 새로 추가됨
        private float castleExitDelayTimer = 0f;   // 새로 추가됨

        private readonly Dictionary<Transform, Player> playerCache = new();   // 새로 추가됨

        // **TriggerStay 실행 주기 제한용 타이머**
        private const float TRIGGER_COOLDOWN = 0.5f;   // 0.5 초마다 한 번만 실행
        private float nextTriggerTime = 0f;             // 새로 추가됨


        void Start ()
        {
            Invoke("CanSpawnTime", 5f);
        }
        
        private void CanSpawnTime()
        {
            startDelay = true;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.parent != null && other.transform.parent.CompareTag(PLAYER_TAG) && !hasCastle && startDelay)
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = true;
            }

            if (other.gameObject.name.Contains(CASTLE_TAG))
            {
                hasCastle = true;
                StopAllCoroutines(); 
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.parent != null && other.transform.parent.CompareTag(PLAYER_TAG))
            {
                other.transform.parent.GetComponent<Player>().isBuildCastle = false;
            }
            if (other.gameObject.name.Contains(CASTLE_TAG))
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



