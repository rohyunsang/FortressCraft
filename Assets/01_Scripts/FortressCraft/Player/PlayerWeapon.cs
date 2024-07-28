using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
<<<<<<< HEAD

    public float damage = 10f;
}
=======
    public BoxCollider2D col;
    public float damage = 10f;
    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if(other.CompareTag("A")) 
        {
            
        }
        else
        {
            col.enabled = false;
        }
        
    }
}
>>>>>>> Seong_0.01
