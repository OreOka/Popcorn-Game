using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingArrowScript : MonoBehaviour
{
    private Vector2 arrowDirection;


    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        arrowDirection.Set(horizontal, vertical);
        float angle = Vector2.SignedAngle(Vector2.up, arrowDirection);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }
}
