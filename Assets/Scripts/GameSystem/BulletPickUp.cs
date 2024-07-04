using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPickUp : MonoBehaviour
{
    private int ammoAmount = 15;

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            if(playerAttack != null) {
                playerAttack.OnAmmoAdd(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}
