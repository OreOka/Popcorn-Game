using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamea : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin noisePerlin;
    CinemachineVirtualCamera vcam;
    GameObject player;
    [SerializeField] private float shakeTime = 1f;
    [SerializeField] private float frequencyGain = 1f;
    [SerializeField] private float amplitudeGain= 1f;
    private bool isShaking = false;
    private float shakeTimeElapsed = 0f;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noisePerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CharacterEvents.characterPopped += ShakeonPop;
        CharacterEvents.characterDamaged += ShakeonHit;
    }
    private void OnDestroy()
    {
        CharacterEvents.characterPopped -= ShakeonPop;
        
        CharacterEvents.characterDamaged -= ShakeonHit;
    }
  

    private void Update()
    {

        if (isShaking)
        {
        shakeTimeElapsed += Time.deltaTime;

            if (shakeTimeElapsed > shakeTime)
            {
                StopShake();
            }
        }


    }
    public void ShakeonPop(string popType, GameObject characterPopping)
    {
        if (characterPopping.tag == "Player")
        {
            StartShake();
        }
    }

    public void ShakeonHit(float damage, GameObject characterHit)
    {
        if (characterHit.tag == "Player")
        {
            StartShake();

        }
    }
    public void StartShake()
    {
        noisePerlin.m_AmplitudeGain = amplitudeGain;
        noisePerlin.m_FrequencyGain = frequencyGain;
        isShaking = true;
        shakeTimeElapsed = 0;
    }
    public void StopShake()
    {
        isShaking = false;
        noisePerlin.m_AmplitudeGain = 0;
        noisePerlin.m_FrequencyGain = 0;
        shakeTimeElapsed = 0;
    }
}
