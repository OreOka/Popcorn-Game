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
        private set { }
    }

    public float regularPopEnergy = 0.3f;
    public float dodgePopEnergy = 0.22f;

    [SerializeField] private float recoveryRate = 0.9f;

    PlayerController playerController;
    private bool _isGameOver;
    private string _startLevel = "Test Level";

    private Vector3 _checkpoint;
    public float EnergyLevel { get; private set; }
    private float timer = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        CharacterEvents.characterDefeated += HandleDeath;
        CharacterEvents.NotAvailablePlayerAction += NullActionController;
        EnergyLevel = 1;
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
        _checkpoint.Set(location.x, location.y, 0);
    }

    private void SetEnergyLevel()
    {
       
        if (EnergyLevel< 1)
        {
            if ((Time.time - timer) > 0)
            {
                EnergyLevel += .01f;
                timer = Time.time+recoveryRate;
            }
        }
        else timer = Time.time + recoveryRate;

        if (playerController.IsGrounded())
        {
            EnergyLevel = 1;
        }
        else recoveryRate = 1f;
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
    public void RecordPop(string popType, GameObject gameObject)
    {
        switch (popType)
        {
            case "Dodge": //which is DODGE value
                EnergyLevel -= dodgePopEnergy;


                //TODO: play animation on screen of pop being taken away
                //TODO: 
                break;
            case "Regular": //which is REGULAR POP value
                EnergyLevel -= regularPopEnergy;

                //TODO: play animation on screen of pop being taken away
                break;
            default:
                print("No pop type was calculated");
                break;

        }
    }


    // Update is called once per frame
}
