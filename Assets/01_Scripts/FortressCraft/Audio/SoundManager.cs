using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; set; }

        public float BGMVolume { get; set; }
        public float SFXVolume { get; set; }

        private void Awake()
        {
            Instance = this;

            BGMVolume = 1.0f;
            SFXVolume = 1.0f;
        }

        public void UpdateBGMVolume()
        {
            BGM bgm = GameObject.Find("BGM").GetComponent<BGM>();
            bgm.SetVolume(BGMVolume);
        }
    }
}


