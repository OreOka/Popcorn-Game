using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    
    private Slider slider;
    
    private PlayerController playerController;

    [SerializeField] private float playerHealth = 100;
    

    
    
    public bool isPopping { get; private set; }

    


    private float recoveryTimeElapsed;

    private void Awake()
    {
        //CharacterEvents.characterPopped += RecordPop;
        CharacterEvents.characterDamaged += ApplyDamage;
       
        playerController = GetComponent<PlayerController>();
    }
    private void OnDestroy()
    {
      //  CharacterEvents.characterPopped -= RecordPop;
        CharacterEvents.characterDamaged -= ApplyDamage;
    }

    void AddPoints()
    {
          
           

     }

        private void Start()
    {
        
        // Start function WaitAndPrint as a coroutine.
        
    }
    private void Update()
    {
        

    }

    private void ApplyDamage(float damageValue)
    {
        print("Character is losing health" + damageValue);
        playerHealth -= damageValue;
        print(playerHealth);

    }

    



}
