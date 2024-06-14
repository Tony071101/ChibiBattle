using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    private List<GameObject> characters = new List<GameObject>();
    private int selectedIndex = 0;

    private void Start() {
        InitializeCharacters();

        //turn off every characters except the 1st one (if have).
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(i == selectedIndex);
        }
        UpdateCurrentCharacterComponents();
    }

    private void Update() {
        //Check the Num keys from 1 to number of characters.
        for (int i = 0; i < characters.Count; i++)
        {
            if (i != selectedIndex && Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectCharacter(i);
            }
        }
    }

    private void InitializeCharacters() {
        //Initialize a list of characters.
        foreach(Transform child in transform) {
            if(child.GetComponent<Animator>() != null) {
                AddCharacter(child.gameObject);
            }
        }
    }

    private void AddCharacter(GameObject character) {
        if (character.GetComponent<Animator>() != null)
        {
            characters.Add(character);
        }
    }

    private void SelectCharacter(int index) {
        if (index >= 0 && index < characters.Count && index != selectedIndex)
        {
            //disable the current character.
            characters[selectedIndex].SetActive(false);
            //enable next character.
            characters[index].SetActive(true);
            //update the index.
            selectedIndex = index;

            UpdateCurrentCharacterComponents();
        }
    }

    private void UpdateCurrentCharacterComponents() {
        if (characters[selectedIndex] != null) {
            // Update references to the components of the currently selected character
            Animator _anim = characters[selectedIndex].GetComponent<Animator>();
            WeaponManager _weaponManager = characters[selectedIndex].GetComponent<WeaponManager>();

            if (_anim == null || _weaponManager == null)
            {
                Debug.LogError("One or more components are missing on the selected character.");
            }
        }
    }
}
