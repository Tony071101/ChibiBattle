using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "New Player Data", menuName = "Sciptable Ojects/Player Data")]
public class PlayerData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    public GameObject characterModel;
}
