using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class LevelUI : MonoBehaviour
    {
        private Text levelText;
        private Player player;

        private void Awake()
        {
            levelText = GetComponent<Text>();
            player = transform.parent.parent.GetComponent<Player>();
        }

        private void FixedUpdate()
        {
            Debug.Log("Level: " + player.level);
            levelText.text = player.level.ToString();
        }
    }
}


