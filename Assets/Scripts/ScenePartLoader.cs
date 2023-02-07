using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.ComponentModel;
using System;

public enum CheckMethod
{
    Distance,
    Trigger
}

public class ScenePartLoader : MonoBehaviour
    {
        // Use this for initialization
    public Transform player;
   
    [SerializeField] CheckMethod checkMethod;
    [SerializeField] float loadRange;

    private bool isLoaded;
    private bool shouldLoad;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else if (checkMethod == CheckMethod.Trigger)
        {
            TriggerCheck();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }
    private void TriggerCheck()
    {
        if (shouldLoad)
        {
            LoadScene();
           
        }
        else
        {
            UnloadScene();
        }
    }

  

    private void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < loadRange)
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!isLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            isLoaded = true;
        }
    }

    private void UnloadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }
}
