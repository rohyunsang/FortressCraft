using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class ArrowVector : NetworkBehaviour
    {
        public Vector2 TargetDirection { get; set; }
        public
        Vector3 pos;

        private void Awake()
        {
            TargetDirection = Vector2.left;
        }

        public override void Spawned()
        {
            if (transform.parent.gameObject.GetComponent<Player>().PlayerIndex !=
                Runner.LocalPlayer.PlayerId - 1)
            {
                gameObject.SetActive(false);
            }
        }

        public override void FixedUpdateNetwork()
        {
            float angle = Mathf.Atan2(TargetDirection.y, TargetDirection.x) * Mathf.Rad2Deg;
            Quaternion FireRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = FireRotation;

            float range = 0.3f;
            pos = new Vector3(TargetDirection.x * range, TargetDirection.y * range + 0.1f, 0.0f);

            transform.position = transform.parent.position + pos;
        }

        private void FixedUpdate()
        {
            
        }
    }

}

