using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private enum PlatformType {Water, Fire, Blade}
    [SerializeField] PlatformType platformType;
    private string charactermode;

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


    private void SetCharacterMode(string mode, GameObject arg1)
    {
        charactermode = mode;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            //check platform mode

            //if Water
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
