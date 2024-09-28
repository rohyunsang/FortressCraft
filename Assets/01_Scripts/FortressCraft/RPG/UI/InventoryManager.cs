using UnityEngine;

namespace Agit.FortressCraft
{

    public class InventoryManager : MonoBehaviour
    {
        public Transform equipmentInventoryContent; // �κ��丮 UI�� Content ����
        public Transform consumableInventoryContent;
        public Transform miscInventoryContent;
        public GameObject itemPrefab; // �������� ǥ���� ������
        public static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // ���ӿ�����Ʈ�� �� �ε� �� �ı����� �ʵ��� ����
            }
            else
            {
                Destroy(gameObject); // �̹� �ν��Ͻ��� ������ ��� �ߺ� ������ ��ü�� �ı�
            }
        }
    }
}

