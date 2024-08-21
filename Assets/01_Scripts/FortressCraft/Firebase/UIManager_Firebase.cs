using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft {

    public class UIManager_Firebase : MonoBehaviour
    {
        public GameObject _loginScreen;
        public GameObject _mainScreen;

        public Text _uuidText;

        public static UIManager_Firebase _instance;

        public static UIManager_Firebase Instance
        {
            get
            {
                // �ν��Ͻ��� ���ٸ� ���� ����
                if (_instance == null)
                {
                    // �� GameObject�� �����ϰ� UIManager_Firebase ������Ʈ�� �߰�
                    _instance = new GameObject("UIManager_Firebase").AddComponent<UIManager_Firebase>();
                }
                return _instance;
            }
        }


        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject); // �̱��� ��ü �ı� ����
            }
            else
            {
                Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
            }
        }
    }

}

