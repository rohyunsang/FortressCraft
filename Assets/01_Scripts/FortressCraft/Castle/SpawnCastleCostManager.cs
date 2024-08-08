using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agit.FortressCraft
{
    public class SpawnCastleCostManager : MonoBehaviour
    {
        public static SpawnCastleCostManager Instance { get; set; }

        [SerializeField] private Text costText;
        public List<int> costByLevel = new List<int>();
        public int level { get; private set; }

        private void Awake()
        {
            Instance = this;
            level = 0;
            costText.text = costByLevel[0].ToString();
        }

        public int GetCost(int level)
        {
            return costByLevel[level];
        }

        public void LevelUp()
        {
            if (level > 15) return;
            ++level;
            costText.text = costByLevel[level].ToString();
        }
    }
}