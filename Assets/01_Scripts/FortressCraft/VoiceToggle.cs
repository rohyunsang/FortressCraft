using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceToggle : MonoBehaviour
{
    private bool toggle;

    [SerializeField] private Image voiceIcon;

    [SerializeField] private Sprite voiceOnImage;
    [SerializeField] private Sprite voiceOffImage;

    private void Start()
    {
        toggle = false;
        voiceIcon.sprite = voiceOffImage;
    }

    // Voice Icon Changer
    public void VoiceToggleButton()
    {
        if (toggle)
        {
            voiceIcon.sprite = voiceOffImage;
            toggle = false;
        }
        else
        {
            voiceIcon.sprite = voiceOnImage;
            toggle = true;
        }
    }
}
