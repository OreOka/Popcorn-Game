using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    private Animator m_Animator;
    private const string ANIMATION_RUNSPEED = "runSpeed";


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        m_Animator.SetFloat(ANIMATION_RUNSPEED, CharacterMovement.Instance.runSpeed);
        
    }
}
