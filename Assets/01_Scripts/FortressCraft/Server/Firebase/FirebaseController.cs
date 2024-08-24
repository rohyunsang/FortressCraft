using Agit.FortressCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Agit.FortressCraft
{
    public class FirebaseController : MonoBehaviour
    {
        public void TestButton()
        {
            Debug.Log("TestButton ´­¸²");
            string uid = FirebaseAuthManager.Instance.UserId;
            Debug.Log(uid);
            FirebaseDBManager.Instance.GetUserPropertiesByUid(uid, userProperties =>
            {
                Debug.Log("Loaded User Data for UID: " + uid);
                Debug.Log("Current Gold: " + userProperties.gold);

                int newGold = userProperties.gold + 100;
                FirebaseDBManager.Instance.UpdateGold(uid, newGold);
            });
        }
    }
}


