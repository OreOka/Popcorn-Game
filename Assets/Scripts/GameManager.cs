using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Game Manager is Null!!");

            return _instance;
        }
    }


    PlayerController playerController;
    private bool _isGameOver;
    private string _startLevel = "Test Level";
    private Vector3 _checkpoint;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        CharacterEvents.characterDefeated += HandleDeath;
        CharacterEvents.NotAvailablePlayerAction += NullActionController;
    }

    private void NullActionController(string actionName, GameObject character)
    {
        throw new NotImplementedException();
    }

    public void CharacterDead(bool flag)
    {
        _isGameOver = flag;
    }

    public bool IsCharacterDead()
    {
        return _isGameOver;
    }
    public string GetStartLevel()
    {
        return _startLevel;
    }

    public Vector3 GetPlayerCheckpoint()
    {
        return _checkpoint;
    }
    public void SetPlayerCheckPoint(Vector3 location)
    {
        _checkpoint = location;
    }
    private void HandleDeath(GameObject character)
    {
        if (character.CompareTag("Player"))
        {
            character.transform.position = _checkpoint;

        }

        //ShowLoadingScreen
        //load the current level scene additively || let the scene partLoader do it if it is already done
        //moveplayer to location
        //StartCoroutine for Loadin screen

    }
    // Update is called once per frame
}
