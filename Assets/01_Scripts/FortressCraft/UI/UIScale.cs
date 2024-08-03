using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Agit.FortressCraft
{
    public class UIScale : NetworkBehaviour
    {
        private void Update()
        {
            RPCSettingScale();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCSettingScale()
        {
            if (transform.parent.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1.0f,
                                                    transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
                                                    transform.localScale.y, transform.localScale.z);
            }
        }
    }
}


