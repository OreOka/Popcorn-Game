using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PlayerController playerController;
    // Start is called before the first frame update
    void Awake()
    {

        CharacterEvents.characterDefeated += ReloadScene;
        CharacterEvents.NotAvailablePlayerAction += NullActionController;
    }

    private void NullActionController(string actionName, GameObject character)
    {
        throw new NotImplementedException();
    }

    private void ReloadScene(GameObject character)
    {
        if (character.tag == "Player")
        {
            SceneManager.LoadScene("Test level");
            Resources.UnloadUnusedAssets();


        }
    }
    // Update is called once per frame
    
}
