using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCurrency : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            Player player = FindObjectOfType<Player>();
            player.AddCoins(15);   
            Destroy(gameObject);
        }
    }
}
