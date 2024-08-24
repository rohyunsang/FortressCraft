using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabNavigation : MonoBehaviour
{
    public static bool IsTabEnabled = true;

    private InputField mCurrentInputField;
    [SerializeField] private InputField mPrevInputField = null;
    [SerializeField] private InputField mNextInputField = null;

    private void Awake()
    {
        mCurrentInputField = mPrevInputField;
    }

    void Update()
    {
        if (mCurrentInputField.isFocused && IsTabEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // 다음 입력 필드로 포커싱 이동
                mNextInputField?.Select();
            }
        }
    }
}