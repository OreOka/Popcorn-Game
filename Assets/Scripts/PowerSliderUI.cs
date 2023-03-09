using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerSliderUI : MonoBehaviour
{
    // Start is called before the first frame update
    Slider slider;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = GameManager.Instance.EnergyLevel;
    }
}
