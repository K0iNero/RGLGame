using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Weapon") {
            HP--;
            if (HP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
