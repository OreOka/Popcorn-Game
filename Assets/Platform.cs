using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private enum PlatformType {Water, Fire, Blade}
    [SerializeField] PlatformType platformType;
    private string charactermode;
    private float totalTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        CharacterEvents.characterMode += SetCharacterMode;
    }


    private void OnDestroy()
    {
        CharacterEvents.characterMode -= SetCharacterMode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
    }
   
    
    private void SetCharacterMode(string mode, GameObject arg1)
    {
        charactermode = mode;
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        
        GameObject player = col.gameObject;
        if (col.CompareTag("Player"))
        {
            //check platform mode
            
            switch (platformType)
            {
                case PlatformType.Water:

                    if (charactermode.Equals("PopCorn"))
                    {
                        totalTime += Time.deltaTime;
                        if (totalTime > 1)
                        {
                            CharacterEvents.characterDamaged.Invoke(-1, player);
                            totalTime = 0;
                        }
                    }

                    break;
                default:
                    print("no platform type set");
                    break;
            }

            //invoke damage on Coroutine like Ori in water
            //or invoke slowed mode extra boost to player

            //if Fire
            //invoke death
            //or invoke boosted mode

            //if blade
            //kill
        }
    }

}
