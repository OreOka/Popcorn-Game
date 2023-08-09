using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Transform popCornSprite;
    private Animator m_Animator;
    private const string ANIMATION_RUNSPEED = "runSpeed";
    private const string ANIMATION_POPCORN_SHRINK = "popShrink";
    private const string ANIMATION_IS_GROUNDED = "isGrounded";


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }


    private void Start()
    {
        CharacterMovement.Instance.OnPlayerModeChange += CharacterMovement_OnPlayerModeChange;
        CharacterMovement.Instance.OnPlayerGrounded += CharacterMovement_OnPlayerGrounded;
    }

    private void CharacterMovement_OnPlayerGrounded(object sender, EventArgs e)
    {
        m_Animator.SetTrigger(ANIMATION_IS_GROUNDED);
    }

    private void CharacterMovement_OnPlayerModeChange(object sender, CharacterMovement.OnPlayerMode e)
    {
        if (e.characterMode == CharacterMovement.CharacterMode.PopCorn)
        {
            popCornSprite.gameObject.SetActive(true);
        }
        else
        {
            m_Animator.SetTrigger(ANIMATION_POPCORN_SHRINK);
           // popCornSprite.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        m_Animator.SetFloat(ANIMATION_RUNSPEED, CharacterMovement.Instance.runSpeed);
        
    }
}
