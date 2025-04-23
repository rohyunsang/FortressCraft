using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private GameObject BGM_Main;

        public static SoundManager Instance { get; set; }

        public float BGMVolume { get; set; }
        public float SFXVolume { get; set; }

        private void Awake()
        {
            Instance = this;

            BGMVolume = 0.0f;
            SFXVolume = 0.0f;
        }

        public void UpdateBGMVolume()
        {
            BGM bgm = GameObject.Find("BGM").GetComponent<BGM>();
            bgm.SetVolume(BGMVolume);
        }

        public void SpawnMainBGM()
        {
            Instantiate(BGM_Main).gameObject.name ="BGM";
        }
    }
}


