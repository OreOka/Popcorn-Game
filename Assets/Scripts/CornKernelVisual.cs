using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornKernelVisual : MonoBehaviour
{
    private Animator m_Animator;
    private const string ANIMATION_KERNELACTIVATE = "activateKernel";

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        CharacterMovement.Instance.OnPlayerModeChange += CharacterMovement_OnPlayerModeChange;
    }

    private void CharacterMovement_OnPlayerModeChange(object sender, CharacterMovement.OnPlayerMode e)
    {
        if(e.characterMode == CharacterMovement.CharacterMode.Kernel)
        {
            gameObject.SetActive(true);
            m_Animator.SetTrigger(ANIMATION_KERNELACTIVATE);
        }
        else
        {
            gameObject.SetActive(false);   
        }
    }

    
}
