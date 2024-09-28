using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceManager : MonoBehaviour
{
    public Button voiceOffButton;
    public Button voiceOnButton;
    public void OnClickVoiceOff()
    {
        GameObject.Find("[Recorder]").GetComponent<ConnectAndJoin>().Init();
    }

    public void OncClickVoiceOn()
    {
        GameObject.Find("[Recorder]").GetComponent<UnityVoiceClient>().DisConnectVoice();
    }

}
