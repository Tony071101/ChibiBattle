using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "New Character Data", menuName = "Sciptable Ojects/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character base stats")]
    public int baseHealth;
    public int baseDamage;
    [Header("Character properties")]
    public string characterName;
    public Sprite characterSprite;
    public GameObject characterModel;
    public string characterDescription;

    [Header("Character Voice Lines")]
    public AudioClip onLobby;
    public AudioClip onMove;
    public AudioClip onHurt;
    public AudioClip onGameStart;
    public AudioClip onVictory;

    [Header("Character SFX")]
    public AudioClip attackSFX;
    public AudioClip impactSFX;
}
