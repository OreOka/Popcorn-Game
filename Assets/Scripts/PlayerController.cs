using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class PlayerController : MonoBehaviour
{
    public float runSpeed = 10f;
    public float power = 5f;
    
    public Rigidbody2D m_Rigidbody;
    LineRenderer m_LineRenderer;
    [SerializeField] AnimationCurve runSpeedCurve;


    CapsuleCollider2D m_Capsule;
    [SerializeField] private LayerMask platformLayerMask;
    
    internal float timer;
    internal bool canTransform = false;
    private Vector2 direction;

    [SerializeField] internal AudioSource popSoundEffect;
    [SerializeField] internal AudioSource laserSoundEffect;

    Vector2 m_CurrentPosition;
    Vector2 m_PreviousPosition;
    Vector2 m_NextMovement;
    public float groundedRayCastDistance = 0.1f;
    Vector2[] m_RaycastPositions = new Vector2[3];
    ContactFilter2D m_ContactFilter;
    RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
    RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
    Collider2D[] m_GroundColliders = new Collider2D[3];
    private State state;
    private float runTimer;
    private PlayerManager m_PlayerManager;

    public bool IsCeilinged { get; private set; }
    public Vector2 Velocity { get; private set; }
    public bool hasStamina { get; private set; }
    public bool isDodging { get; private set; }
    public bool isUsingPowerUp { get; private set; }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer= GetComponent<LineRenderer>();
        m_PlayerManager= GetComponent<PlayerManager>();
        m_Capsule= GetComponent<CapsuleCollider2D>();
        state = new PopCornState();
        state.SetPlayer(this);

        m_CurrentPosition = m_Rigidbody.position;
        m_PreviousPosition = m_Rigidbody.position;

     

    }
    // Update is called once per frame
    void Update()
    {
        float leftHorizontal = Input.GetAxis("Horizontal");
        float rightHorizontal = Input.GetAxis("Mouse X");
        float leftVertical = Input.GetAxis("Vertical");
        float rightVertical = Input.GetAxis("Mouse Y");

       if (Input.GetButtonDown("Pop") && !canTransform && m_PlayerManager.NumberOfActivePops > 0)
       // if (Input.GetButtonDown("Jump") && !canTransform)
        {
            laserSoundEffect.Play();
            ChangeRbPhysics(m_Rigidbody, 5, 10, 200);
          //  m_PlayerManager.RecordPop("Regular");
            canTransform = true;
        }
      

      
        isUsingPowerUp = Input.GetButton("Power up");

        //animation curve to set speed applicatiion
        runSpeed = SetAnimationCurve(runSpeed, runSpeedCurve);


        if (canTransform)
        {
            timer = Time.time; //this to keep track of how long transformation has occured
            canTransform = Transform(rightHorizontal, rightVertical, m_LineRenderer);
            
        }else if (Input.GetAxis("Horizontal") != 0)
        {
            runTimer += Time.deltaTime; // used in calculating how long the character has been running
            Run(leftHorizontal, leftVertical, m_LineRenderer);   
        }
        else
        {
            runTimer = 0;
        }

        if (Input.GetButtonDown("Jump") && m_PlayerManager.NumberOfActivePops > 0)
        {
            popSoundEffect.Play();
            ChangeRbPhysics(m_Rigidbody, 10, 10, 200);
            Vector2 _velocity = (new Vector2(m_Rigidbody.velocity.x, power));
            m_Rigidbody.velocity = _velocity;
            CharacterEvents.characterPopped("Dodge", gameObject);

       //     m_PlayerManager.RecordPop("Dodge");

        }

        //pop 

            //add force to the ball on hit click
            //set hit to true 

            //-----states---------
            //-popcorn mode

            //-kernel mode
            //-powered up state


            //while hit is true
            //  if not let go stay in popcorn state 


            //use state design pattern to determine what happens for damages and actually everything

    }

    private float SetAnimationCurve(float runSpeed, AnimationCurve runSpeedCurve)
    {
        return runSpeed = runSpeedCurve.Evaluate(runTimer);
    }

    public void ChangeRbPhysics(Rigidbody2D rigidbody, float gravityScale, float drag, float power)
    {
        rigidbody.gravityScale = gravityScale;
        rigidbody.drag = drag;
        this.power = power;
    }

       

    private void Run(float horizontal, float vertical, LineRenderer lineRenderer)
    {
        direction = new Vector2(horizontal, 0); // for only horizontal running
        
        state.PlayerMovement(direction, lineRenderer);
    }

    private bool Transform(float horizontal, float vertical, LineRenderer line)
    {
        direction = new Vector2(horizontal, vertical) * 2;
       return state.PlayerMovement(direction, line);
       // Debug.DrawRay(transform.position, new Vector3(direction.x, direction.y, 0) * 10f, Color.red);
       

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
    public void TransitionTo(State state)
    {
        this.state = state;
        Debug.Log("Transitioning to "+ state);

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

public abstract class State
{
    public PlayerController playerController;

    public CharacterEvents characterEvents;
 

    public void SetPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public abstract void IsHit();
    //  PlayerMovement()
    public abstract bool PlayerMovement(Vector2 direction, LineRenderer line);
    public abstract void PoppingOut();
    public abstract void Attack();


}
public class PopCornState : State
{
    Vector2 _velocity;


    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void IsHit()
    {
        throw new System.NotImplementedException();
    }

    public override bool PlayerMovement(Vector2 direction, LineRenderer line)
    {
        if (playerController.getCanTransform()) //player is beginning a kernel pop
        {
            KernelState kernelState = new KernelState(Time.time);
            kernelState.SetPlayer(playerController);// it is important to do for everytransition so fix constructor
            playerController.TransitionTo(kernelState);
            return true;
        }
        //      handle regular movement
       // if(player.transform.position == new Vector3(3.30552506f, -7.82100248f, 0))
       
        
            _velocity = new Vector2(direction.x * playerController.runSpeed, playerController.m_Rigidbody.velocity.y);
            playerController.m_Rigidbody.velocity = _velocity;
        
     



        return false;
    }

    public override void PoppingOut()
    {
        throw new System.NotImplementedException();
    }
}

public class KernelState : State
{
    private float loadTimer;
    private bool receivedPush = false;
    private float midPoint =0f; //element 0 for Midpoint, element 1 for transform

    public float loadTime { get; private set; }

    KernelState()
    {

    }
    public KernelState(float loadTimer)
    {
        this.loadTimer = loadTimer;
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void IsHit()
    {
        throw new System.NotImplementedException();
    }


    public override bool PlayerMovement(Vector2 direction, LineRenderer line)
    {

        loadTime = 0.4f;

        Vector2 _velocity; 
        _velocity= direction * playerController.power;


         Vector2[] trajectory = playerController.Plot((Vector2)playerController.transform.position, _velocity, 250);
        bool isGoingForward = true;
        if (midPoint == 0f || midPoint != trajectory[3].x)
            midPoint = trajectory[3].x;

        if (trajectory[0].x < midPoint)
            isGoingForward = true;
        else
            isGoingForward = false;

        ///----------- This is when the character has received a push and is already in the air already.
        if (receivedPush && popCornIsAtPeak(midPoint, isGoingForward))
        {
            //Change state and reset temporary paramaters
        
            PopCornState popCornState = new PopCornState();
            popCornState.SetPlayer(playerController);
            this.playerController.TransitionTo(popCornState);
            midPoint = 0f;
            playerController.canTransform = false;
            receivedPush = false;
            return false;
        }
       


        line.positionCount = trajectory.Length;
        Vector3[] positions = new Vector3[trajectory.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = trajectory[i];
        }

        line.SetPositions(positions);



        //check the timer  variables a video on Time time maybe??
        //loadTime = 0f;
        if (playerController.timer > loadTimer + loadTime && receivedPush != true)//-- you may want to add this again>>>
        {
            CharacterEvents.characterPopped.Invoke("Regular", playerController.gameObject);

            playerController.popSoundEffect.Play();
            Time.timeScale = 1f;// 
            

            

            playerController.m_Rigidbody.velocity = _velocity;
            receivedPush = true;

            return true;
        }
        else if (playerController.timer <= loadTimer + loadTime)
        {

            Time.timeScale = 0.6f;// 
            return true;
        }
        else
            return true;
      
        
        }

    
        private bool popCornIsAtPeak(float midPoint, bool isGoingForward)
    {

        //check if player.transform is at that location.

        if (isGoingForward)
        {
            if (playerController.transform.position.x >= midPoint)
            {
                Debug.Log("Transform: " + playerController.transform.position.x + "\nMidpoint: " +
                    midPoint);
            }
            return playerController.transform.position.x >= midPoint;
        }
        else
        {
            if (playerController.transform.position.x <= midPoint)
            {
                Debug.Log("Transform: " + playerController.transform.position.x + "\nMidpoint: " +
                    midPoint);
            }
            return playerController.transform.position.x <= midPoint;
        }
    }

    public override void PoppingOut()
    {
        
    }
    public void SetLoadTimer(float timer)
    {
        loadTimer = timer;
  

    }
}