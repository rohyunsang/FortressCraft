using System.Collections.Generic;
using UnityEngine;

namespace Agit.FortressCraft
{
    public class UnitSeparationConnector : MonoBehaviour
    {
        [SerializeField] private UnitSeparationSystem separationSystem;
        [SerializeField] private float updateInterval = 1f;

        private float timer = 0f;

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                timer = 0f;
                RefreshUnitTransforms();
            }
        }

        private void RefreshUnitTransforms()
        {
            List<Transform> unitList = new List<Transform>();
            var units = FindObjectsOfType<NormalUnitRigidBodyMovement>();

            foreach (var unit in units)
            {
                if (unit != null && unit.gameObject.activeInHierarchy)
                    unitList.Add(unit.transform);
            }

            separationSystem.unitTransforms = unitList.ToArray();
        }
    }
}