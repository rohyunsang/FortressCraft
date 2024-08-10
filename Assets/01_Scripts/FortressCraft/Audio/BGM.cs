using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class BGM : MonoBehaviour
    {
        private AudioSource bgm = null;

        private void Awake()
        {
            bgm = GetComponent<AudioSource>();
        }

        private void Start()
        {
            bgm.volume = SoundManager.Instance.BGMVolume;
        }

        public void SetVolume(float volume)
        {
            if (bgm == null) return;
            bgm.volume = volume;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}


