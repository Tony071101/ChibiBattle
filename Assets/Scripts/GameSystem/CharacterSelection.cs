using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private List<CharacterData> characterDatas = new List<CharacterData>();
    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform btnParent;
    [SerializeField] private Transform modelDisplay;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    [SerializeField] private GameObject btnPlay;
    [SerializeField] private AudioSource audioSource;
    private GameObject currentModel;

    private void Start() {
        LoadCharacterDatas();
        GenerateCharacterButtons();
        btnPlay.SetActive(false);
    }
    
    private void LoadCharacterDatas()
    {
        CharacterData[] loadedCharacterDatas = Resources.LoadAll<CharacterData>("CharacterDatas");
        characterDatas.AddRange(loadedCharacterDatas);
    }

    private void GenerateCharacterButtons() {
        foreach (CharacterData character in characterDatas) {
            GameObject newBtn = Instantiate(btnPrefab, btnParent);
            newBtn.GetComponentInChildren<Image>().sprite = character.characterSprite;
            newBtn.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(character));
        }
    }

    private void SelectCharacter(CharacterData character) {
        if(currentModel != null) {
            Destroy(currentModel);
            if(audioSource.isPlaying) {
                audioSource.Stop();
            }
        }
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        currentModel = Instantiate(character.characterModel, modelDisplay.position, rotation, modelDisplay);

        characterNameText.text = character.characterName;
        characterDescriptionText.text = character.characterDescription;

        PlayerPrefs.SetString("SelectedCharacter", character.characterName);
        PlayerPrefs.Save();
        btnPlay.SetActive(true);

        if (character.onLobby != null) {
            audioSource.clip = character.onLobby;
            audioSource.Play();
        }
    }
}
