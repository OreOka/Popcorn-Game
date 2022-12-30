using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int regularRecoveryTime = 3;
    [SerializeField] private int dodgeRecoveryTime = 2;
    [SerializeField] private Slider slider;
    private IEnumerator recoveryCouroutine;

    

    
    public int NumberOfActivePops { get; private set; }
    public bool isPopping { get; private set; }

    private string switchType;

    private void Awake()
    {
        CharacterEvents.characterPopped += RecordPop;
    }
    IEnumerator WaitAndRecover(float time)
    {
        yield return new WaitForSeconds(time);
        //TODO play animation to increase correct Pop
        NumberOfActivePops++;
        print("Wait and Recover. Number of Pops  " + NumberOfActivePops);
    }

    public void setPop(string switchType)
    {
        isPopping = true;
        this.switchType = switchType;
    }
    private void Start()
    {
        NumberOfActivePops = 3;
        // Start function WaitAndPrint as a coroutine.

    }
    private void Update()
    {
     
        slider.value = NumberOfActivePops;
    }

    public void RecordPop(string popType, GameObject gameObject)
    {
        switch (popType)
        {
            case "Dodge": //which is DODGE value
                NumberOfActivePops--;
                recoveryCouroutine = WaitAndRecover(dodgeRecoveryTime);
                StartCoroutine(recoveryCouroutine);
                //TODO: play animation on screen of pop being taken away
                //TODO: 
                break;
            case "Regular": //which is REGULAR POP value
                NumberOfActivePops--;
                recoveryCouroutine = WaitAndRecover(regularRecoveryTime);
                StartCoroutine(recoveryCouroutine);
                //TODO: play animation on screen of pop being taken away
                break;
            default:
                print("No pop type was calculated");
                break;

        }
    }



}
