using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    public float runSpeed = 10f;

    public InputActionAsset actions;

    private InputAction moveAction;

    
    public bool HasPopInput { get; set; }


    public Rigidbody2D m_Rigidbody;
    LineRenderer m_LineRenderer;
    [SerializeField] AnimationCurve runSpeedCurve;


     internal CapsuleCollider2D m_Capsule;
    [SerializeField] private LayerMask platformLayerMask;
    
    internal float timer;
    internal bool canTransform = false;

    [SerializeField] internal AudioSource popSoundEffect;
    [SerializeField] internal AudioSource laserSoundEffect;

    public GameObject cornKernel;
  
    public float groundedRayCastDistance = 0.1f;
    
   
   // private State state;
    private float runTimer;
    internal SpriteRenderer sprite;
    private PlayerManager m_PlayerManager;
    public LayerMask deathLayerMask;



    public bool IsCeilinged { get; private set; }
    public Vector2 lStickDirection; //;{ get; private set; }
    public Vector2 Velocity { get; private set; }
    public bool hasStamina { get; private set; }
    public bool isDodging { get; private set; }

    public bool IsKernelModeLocked { get; private set; }
    public bool HasRunInput { get;  private set; }
    public bool isUsingPowerUp { get; private set; }
    public bool HasDodgeInput { get;  set; }
    public bool IsPopButtonReleased { get; private set; }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer= GetComponent<LineRenderer>();
        sprite= GetComponent<SpriteRenderer>();
        cornKernel.SetActive(false);
        m_Capsule = GetComponent<CapsuleCollider2D>();

       // moveAction = actions.FindActionMap("Player").FindAction("Move");
        actions.FindActionMap("Player").FindAction("Pop").performed += OnPop;

    }

    public void OnPop(InputAction.CallbackContext context)
    {
        print("Pop");
        
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        print("Dodge");

    }
    public void OnKernelLock(InputAction.CallbackContext context)
    {
        print("Kernel Lock");
       

    }
    

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        lStickDirection = context.ReadValue<Vector2>();
    }


    private void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        actions.FindActionMap("Player").Disable();
    }






    private void Start()
    {
        m_PlayerManager = gameObject.AddComponent(typeof(PlayerManager)) as PlayerManager;
    }
    // Update is called once per frame
    private void FixedUpdate() => checkDeath();

    void Update()
    {

        if (actions["Kernel Hold"].WasPressedThisFrame())
        {
            IsKernelModeLocked = true;
            CharacterEvents.characterMode.Invoke("Kernel", gameObject);
        }

        //animation curve to set speed applicatiion
        runSpeed = SetAnimationCurve(runSpeed, runSpeedCurve);


        if (!HasPopInput && Input.GetAxis("Horizontal") != 0)
        {
            runTimer += Time.deltaTime; // used in calculating how long the character has been running
            HasRunInput = true;

        }
        else
        {
            runTimer = 0;
            HasRunInput = false;
        }
        if (actions["Pop"].WasPressedThisFrame() && m_PlayerManager.EnergyLevel >= m_PlayerManager.regularPopEnergy)
        {

            cornKernel.SetActive(true);
            sprite.enabled = false;
            laserSoundEffect.Play();

            HasPopInput = true;
        }

        if (actions["Dodge"].WasPressedThisFrame()/*Input.GetButtonDown("Dodge")*/ && m_PlayerManager.EnergyLevel >= m_PlayerManager.dodgePopEnergy)
        {
            // cornKernel.SetActive(true);

            HasDodgeInput = true;
        }

        if (actions["Dodge"].WasReleasedThisFrame() || actions["Pop"].WasReleasedThisFrame())
        {
            IsPopButtonReleased = true;
        }
        else if (actions["Dodge"].WasPressedThisFrame() || actions["Pop"].WasPressedThisFrame())
            IsPopButtonReleased = false;
        

        
    }
  
    private float SetAnimationCurve(float runSpeed, AnimationCurve runSpeedCurve)
    {
        return runSpeed = runSpeedCurve.Evaluate(runTimer);
    }

    public void ChangeRbPhysics(Rigidbody2D rigidbody, float gravityScale, float drag, float power)
    {
        rigidbody.gravityScale = gravityScale;
        rigidbody.drag = drag;
       
    }


    public bool getCanTransform()
    {
        return canTransform;
    }
    public Vector2[] Plot(Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * m_Rigidbody.gravityScale * timestep * timestep;

        float drag = 1f - timestep * m_Rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }
        return results;
    }
    //  EnterState()
   
    public void checkDeath()
    {
        float extraHeightText = 0.1f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(m_Capsule.bounds.center, m_Capsule.bounds.size, 0f, Vector2.down, extraHeightText, deathLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
            CharacterEvents.characterDefeated.Invoke(gameObject);

        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(m_Capsule.bounds.center + new Vector3(m_Capsule.bounds.extents.x, 0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, 0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, m_Capsule.bounds.extents.y + extraHeightText), Vector2.down * (m_Capsule.bounds.extents.x), rayColor);

        
    }

    public bool IsGrounded() 
    {
        
        float extraHeightText = 0.1f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(m_Capsule.bounds.center,m_Capsule.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(m_Capsule.bounds.center + new Vector3(m_Capsule.bounds.extents.x,0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, 0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, m_Capsule.bounds.extents.y + extraHeightText), Vector2.down * (m_Capsule.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
    }

}

