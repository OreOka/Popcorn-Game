using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float regularPopEnergy = 0.3f;
    public float dodgePopEnergy= 0.22f;
    
    private Slider slider;
    [SerializeField] private float recoveryRate  = 0.9f;
    private PlayerController playerController;

    [SerializeField] private float playerHealth = 100;
    

    
    public float EnergyLevel { get; private set; }
    public bool isPopping { get; private set; }

    private float timer = 1f;


    private float recoveryTimeElapsed;

    private void Awake()
    {
        CharacterEvents.characterPopped += RecordPop;
        CharacterEvents.characterDamaged += ApplyDamage;
        slider = GameObject.Find("PowerSlider").GetComponent<Slider>();
        playerController = GetComponent<PlayerController>();
    }
    private void OnDestroy()
    {
        CharacterEvents.characterPopped -= RecordPop;
        CharacterEvents.characterDamaged -= ApplyDamage;
    }

    void AddPoints()
    {
            EnergyLevel += .01f;
           

     }

        private void Start()
    {
        EnergyLevel = 1;
        // Start function WaitAndPrint as a coroutine.

    }
    private void Update()
    {
        slider.value = EnergyLevel;
        if (EnergyLevel < 1)
        {
            if ((Time.time - timer) > 0)
            {
                AddPoints();
                timer= Time.time+recoveryRate;
            }
        }
        else timer = Time.time+recoveryRate;

        if (playerController.IsGrounded())
        {
            EnergyLevel = 1;
        }
        else recoveryRate = 1f;

    }

    private void ApplyDamage(float damageValue, GameObject gameObject)
    {
        print("Character is losing health" + damageValue);
        playerHealth += damageValue;
        print(playerHealth);

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
                EnergyLevel-=regularPopEnergy;
               
                //TODO: play animation on screen of pop being taken away
                break;
            default:
                print("No pop type was calculated");
                break;

        }
    }



}
