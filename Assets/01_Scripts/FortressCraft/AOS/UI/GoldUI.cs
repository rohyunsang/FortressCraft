using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class GoldUI : MonoBehaviour
    {
        private Text moneyText;

        private void Awake()
        {
            moneyText = GetComponentInChildren<Text>();
        }

        private void FixedUpdate()
        {
            moneyText.text = RewardManager.Instance.Gold.ToString();
        }
    }
}


