using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

namespace Agit.FortressCraft
{
    public class LevelUI : MonoBehaviour
    {
        private TextMeshProUGUI levelText;
        [SerializeField] private Player player;
        [SerializeField] private RPG_Player player_RPG;

        private void Awake()
        {
            levelText = GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            if (FindObjectOfType<App>().rpgMode) return;
            levelText.text = player.level.ToString();
        }
    }
}


