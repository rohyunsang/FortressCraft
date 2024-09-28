using UnityEngine;

namespace Agit.FortressCraft
{

    public class InventoryManager : MonoBehaviour
    {
        public Transform equipmentInventoryContent; // 인벤토리 UI의 Content 영역
        public Transform consumableInventoryContent;
        public Transform miscInventoryContent;
        public GameObject itemPrefab; // 아이템을 표현할 프리팹
        public static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 게임오브젝트를 씬 로딩 시 파괴하지 않도록 설정
            }
            else
            {
                Destroy(gameObject); // 이미 인스턴스가 생성된 경우 중복 생성된 객체를 파괴
            }
        }
    }
}

