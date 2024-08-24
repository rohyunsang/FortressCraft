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
                // 인스턴스가 없다면 새로 생성
                if (_instance == null)
                {
                    // 새 GameObject를 생성하고 UIManager_Firebase 컴포넌트를 추가
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
                DontDestroyOnLoad(gameObject); // 싱글턴 객체 파괴 방지
            }
            else
            {
                Destroy(gameObject); // 중복 인스턴스 제거
            }
        }
    }

}

