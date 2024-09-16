using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCurrency : MonoBehaviour
{
    private int coinCurrency;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            Player player = FindObjectOfType<Player>();
            player.coinCurrency += 10;   
            Destroy(gameObject);
        }
    }
}
