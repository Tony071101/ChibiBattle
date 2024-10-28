using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private List<PlayerData> playerDatas = new List<PlayerData>();
    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform btnParent;
    [SerializeField] private Transform modelDisplay;
    [SerializeField] private GameObject btnPlay;
    [SerializeField] private AudioSource audioSource;
    private GameObject currentModel;

    private void Start() {
        GenerateCharacterButtons();
        btnPlay.SetActive(false);
    }

    private void GenerateCharacterButtons() {
        foreach (PlayerData player in playerDatas) {
            GameObject newBtn = Instantiate(btnPrefab, btnParent);
            newBtn.GetComponentInChildren<Image>().sprite = player.characterSprite;
            TextMeshProUGUI btnText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) {
                btnText.text = player.characterName;
            }
            newBtn.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(player));
        }
    }

    private void SelectCharacter(PlayerData player) {
        if(currentModel != null) {
            Destroy(currentModel);
            if(audioSource.isPlaying) {
                audioSource.Stop();
            }
        }
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        currentModel = Instantiate(player.characterModel, modelDisplay.position, rotation, modelDisplay);


        PlayerPrefs.SetString("SelectedCharacter", player.characterName);
        PlayerPrefs.Save();
        btnPlay.SetActive(true);

        if (player.onLobby != null) {
            audioSource.clip = player.onLobby;
            audioSource.Play();
        }
    }
}
