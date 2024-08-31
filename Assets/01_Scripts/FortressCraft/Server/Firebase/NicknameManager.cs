using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class NicknameManager : MonoBehaviour
    {
        public InputField _nicknameInputField;
        public Button _submitNicknameButton;
        public string nickname;

        public void Start()
        {
            _submitNicknameButton.onClick.AddListener(TrySetNickname);
        }

        public void TrySetNickname()
        {
            string nickname = _nicknameInputField.text;
            string userId = FirebaseAuthManager.Instance.UserId;
            string uid = FirebaseAuthManager.Instance.UserId;

            if(!PlayerNameValidator.IsValidName(nickname))
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    UIManager.Instance._failNicknameInfo.SetActive(true);
                    UIManager.Instance._outputText.text = "잘못된 닉네임 형식입니다.";
                });
                return;
            }

            FirebaseDBManager.Instance.IsNicknameAvailable(nickname).ContinueWith(task =>
            {
                if (task.Result)
                {
                    // 닉네임 사용 가능
                    FirebaseDBManager.Instance.SetNickname(nickname, userId).ContinueWith(setTask =>
                    {
                        if (setTask.IsCompleted)
                        {
                            Debug.Log("Nickname set successfully");
                            // 성공적으로 설정되었을 때의 로직 구현
                            FirebaseDBManager.Instance.UpdateNickname(uid, nickname);
                            MainThreadDispatcher.Enqueue(() =>
                            {
                                UIManager.Instance._nicknameGroup.SetActive(false);
                                UIManager.Instance._successMakeNicknameGroup.SetActive(true);
                                UIManager.Instance._gameStartButton.SetActive(true);
                                UIManager.Instance._nicknameText.text = nickname;
                            });
                            this.nickname = nickname;
                        }
                    });
                }
                else
                {
                    Debug.Log("Nickname is already taken");
                    // 닉네임이 이미 사용 중일 때의 UI 처리
                    MainThreadDispatcher.Enqueue(() =>
                    {
                        UIManager.Instance._outputText.text = "이미 사용중인 닉네임입니다.";
                    });
                }
            });
        }
    }
}

