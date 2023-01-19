using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterEvents : ScriptableObject
{
    // Start is called before the first frame update
    public static UnityAction<string, GameObject> characterPopped;
    public static UnityAction<string, GameObject> characterMode; // Strings could be "PopCorn" or "Kernel"

    public static UnityAction<float, GameObject> characterDamaged;

    public static UnityAction<GameObject> characterDefeated;
    public static UnityAction<string, GameObject> NotAvailablePlayerAction;

}
