using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class BGM : MonoBehaviour
    {
        private AudioSource bgm;

        private void Awake()
        {
            bgm = GetComponent<AudioSource>();
        }

        public void SetVolume(float volume)
        {
            bgm.volume = volume;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}


