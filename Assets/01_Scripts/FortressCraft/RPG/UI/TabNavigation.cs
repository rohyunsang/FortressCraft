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
                // ���� �Է� �ʵ�� ��Ŀ�� �̵�
                mNextInputField?.Select();
            }
        }
    }
}