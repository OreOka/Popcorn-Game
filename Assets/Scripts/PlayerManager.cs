using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int regularRecoveryTime = 3;
    [SerializeField] private int dodgeRecoveryTime = 2;
    private IEnumerator recoveryCouroutine;

    public int DODGE{ get; private set; }
    public int REGULARPOP { get; private set; }

    
    public int NumberOfActivePops { get; private set; }

    private void Awake()
    {
        DODGE = 1; //don't change
        REGULARPOP = 2; // don't change
    }
    IEnumerator WaitAndRecover(float time)
    {
        yield return new WaitForSeconds(time);
        //TODO play animation to increase correct Pop
        NumberOfActivePops++;
        print("Wait and Recover. Number of Pops  " + NumberOfActivePops);
    }
    private void Start()
    {
        NumberOfActivePops = 3;
        // Start function WaitAndPrint as a coroutine.

        

    }
    private void Update()
    {
       
    }

    public void RecordPop(int popType)
    {
        switch (popType)
        {
            case 1: //which is DODGE value
                NumberOfActivePops--;
                recoveryCouroutine = WaitAndRecover(dodgeRecoveryTime);
                StartCoroutine(recoveryCouroutine);
                //TODO: play animation on screen of pop being taken away
                //TODO: 
                break;
            case 2: //which is REGULAR POP value
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
