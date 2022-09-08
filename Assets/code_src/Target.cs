using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroyOnMeleeAttack(collision);
    }

    private void DestroyOnMeleeAttack(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MeleeWeapon>() == null)
        {
            return;
        }

        Destroy(gameObject);
    }
}
