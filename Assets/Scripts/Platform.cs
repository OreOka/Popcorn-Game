using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private enum PlatformType {Water, Fire, Blade}
    [SerializeField] PlatformType platformType;
    private float totalTime = 1;
    private CharacterMovement.OnPlayerMode mode;

    // Start is called before the first frame update
    void Start()
    {
        CharacterMovement.Instance.OnPlayerModeChange += Player_OnPlayerModeChange;
    }

    private void Player_OnPlayerModeChange(object sender, CharacterMovement.OnPlayerMode e)
    {
        mode = e;
    }

  

  

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
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

                    if (mode.characterMode == CharacterMovement.CharacterMode.PopCorn)
                    {
                        totalTime += Time.deltaTime;
                        if (totalTime > 1)
                        {
                            CharacterEvents.characterDamaged.Invoke(1, player);
                            totalTime = 0;
                        }
                    }

                    break;
                case PlatformType.Fire:
                    if (mode.characterMode == CharacterMovement.CharacterMode.Kernel)
                    {
                        CharacterEvents.characterPowerup.Invoke("FiredUp", player);

                    }
                    else if (mode.characterMode == CharacterMovement.CharacterMode.PopCorn)
                    {
                        CharacterEvents.characterDamaged.Invoke(1, player);

                    }
                    break;

                case PlatformType.Blade:
                    if (mode.characterMode == CharacterMovement.CharacterMode.Kernel)
                    {

                    }
                    else if (mode.characterMode == CharacterMovement.CharacterMode.PopCorn)
                    {
                        CharacterEvents.characterDamaged.Invoke(1, player);
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
