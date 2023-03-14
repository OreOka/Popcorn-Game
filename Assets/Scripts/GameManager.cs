using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnPlayerHealthChange;
    public event EventHandler OnPlayerDead;

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

    [SerializeField] private float energyRecoveryRate = 0.9f;

    PlayerController playerController;
    private bool _isGameOver;
    private float playerHealth = 100f;
    private string _startLevel = "Test Level";

    private Vector3 _checkpoint;
    public float EnergyLevel { get; private set; }
    private float timer = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        EnergyLevel = 1;
    }
    private void Start()
    {
        
        CharacterEvents.NotAvailablePlayerAction += NullActionController;
        CharacterEvents.characterDamaged += ChangeCharacterHealth;
    }

    private void ChangeCharacterHealth(float healthpoints)
    {
        playerHealth -= healthpoints;
        OnPlayerHealthChange?.Invoke(this, EventArgs.Empty);
        if (playerHealth <= 0)
        {
            OnPlayerDead?.Invoke(this, EventArgs.Empty);
        }


    }

    private void OnDestroy()
    {
        CharacterEvents.NotAvailablePlayerAction -= NullActionController;
        
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
                timer = Time.time+energyRecoveryRate;
            }
        }
        else timer = Time.time + energyRecoveryRate;

        if (playerController.IsGrounded())
        {
            EnergyLevel = 1;
        }
        else energyRecoveryRate = 1f;
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
