using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float regularPopEnergy = 0.3f;
    public float dodgePopEnergy= 0.2f;
    private Slider slider;
    [SerializeField] private float recoveryRate  = 0.1f;
    private IEnumerator recoveryCouroutine;
    private PlayerController playerController;
    

    
    public float EnergyLevel { get; private set; }
    public bool isPopping { get; private set; }

    private float timer = 1f;


    private float recoveryTimeElapsed;

    private void Awake()
    {
        CharacterEvents.characterPopped += RecordPop;
        slider = GameObject.Find("PowerSlider").GetComponent<Slider>();
        playerController = GetComponent<PlayerController>();
    }
    private void OnDestroy()
    {
        CharacterEvents.characterPopped -= RecordPop;
    }

    void AddPoints()
    {
            EnergyLevel += .01f;
            print("We are doing it");

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
            Debug.Log("Timer " + timer);
            Debug.Log("Time.Time " + Time.time);

            if ((Time.time - timer) > 0)
            {
                AddPoints();
                timer= Time.time+recoveryRate;
            }
        }
        else timer = Time.time+recoveryRate;

        if (playerController.IsGrounded())
        {
            recoveryRate = 0.00001f;
        }
        else recoveryRate = 0.1f;


    }


    public void RecordPop(string popType, GameObject gameObject)
    {
        switch (popType)
        {
            case "Dodge": //which is DODGE value
                EnergyLevel -=0.1f;
                

                //TODO: play animation on screen of pop being taken away
                //TODO: 
                break;
            case "Regular": //which is REGULAR POP value
                EnergyLevel-=0.3f;
               
                //TODO: play animation on screen of pop being taken away
                break;
            default:
                print("No pop type was calculated");
                break;

        }
    }



}
