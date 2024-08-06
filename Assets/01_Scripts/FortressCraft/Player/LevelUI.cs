using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Agit.FortressCraft
{
    public class LevelUI : MonoBehaviour
    {
        private TextMeshProUGUI levelText;
        [SerializeField] private Player player;

        private void Awake()
        {
            levelText = GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            levelText.text = player.level.ToString();
        }
    }
}


