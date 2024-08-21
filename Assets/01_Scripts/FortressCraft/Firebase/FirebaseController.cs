using Agit.FortressCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
