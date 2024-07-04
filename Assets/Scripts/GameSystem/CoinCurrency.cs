using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCurrency : MonoBehaviour
{
    private int coinCurrency = 10;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            Debug.Log("Player collide with Coin, Coin + " + coinCurrency++);
            Destroy(gameObject);
        }
    }
}
