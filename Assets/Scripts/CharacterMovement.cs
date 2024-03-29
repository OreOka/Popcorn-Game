using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Playables;
using UnityEngine.U2D;


public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement Instance { get; private set; }

    public event EventHandler OnPlayerGrounded;
    public event EventHandler<OnPlayerMode> OnPlayerModeChange;

   

    public class OnPlayerMode : EventArgs
    {
        public CharacterMode characterMode;
    }

    

    public enum CharacterMode
    {
        PopCorn,
        Kernel,

    }

    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private AnimationCurve runSpeedCurve;

    // Start is called before the first frame update
    private PlayerController playerController;
    private float popPower = 5f;
    private CapsuleCollider2D m_Capsule;
    private Rigidbody2D m_Rigidbody;
    private LineRenderer m_LineRenderer;
    private SpriteRenderer sprite;
    private Vector2[] trajectory;
    private bool receivedPush = false;
    private float PopTimer = 0;
    private Vector2 colliderSize;
    private Vector2 colliderOffset;
    private bool canPop;
    private Animator m_Animator;
    private bool canDodge;
    private float runTimer;


    public Vector2 direction;
    public float runSpeed = 10f;
    public Vector2 velocity;

    void Awake()
    {
        Instance = this;

        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer = GetComponent<LineRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        CharacterEvents.characterDamaged += CharacterDamaged;
    }

    void Start()
    {
        canPop = false;
        ChangeRbPhysics(m_Rigidbody, 5, 10, 200);
        gameInput.OnDodgePopAction_completed += GameInput_OnDodgePopAction_completed;
        gameInput.OnPopAction_completed += GameInput_OnPopAction_completed;
        gameInput.OnPopAction_started += GameInput_OnPopAction_started;
        gameInput.OnDodgePopAction_started += GameInput_OnDodgePopAction_started;
        gameInput.onKernelHold_performed += GameInput_onKernelHold_performed;
    }

    private void Update()
    {
        direction = gameInput.GetMovementVector();

        SetPlayerRotation(direction);
        //this is to set the default direction of the controller to face the sky so there always a defualt movement for the character
        if (Math.Abs(direction.x) < 0.5f && Math.Abs(direction.y) < 0.5f)

        {
            direction.Set(0, 1);
            
        }

        runSpeed = SetAnimationCurve(runSpeed, runSpeedCurve);

        if (direction.x != 0)
        {
            runTimer += Time.deltaTime;
        }
        else
        {
            runTimer = 0;
        }

        
        
        velocity.Set(direction.x * runSpeed, m_Rigidbody.velocity.y);
        PopCornRun(velocity);
        
        
    }

    private void SetPlayerRotation(Vector2 direction)
    {
        if (direction.x== 1)
        {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if(direction.x < 0 && transform.rotation.y == 0)
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }

        
        

    }

    private void GameInput_onKernelHold_performed(object sender, EventArgs e)
    {
        ChangeRbPhysics(m_Rigidbody, 2, 0, 100);
        Debug.Log("kernel hold");

    }

    private void GameInput_onPopAction_canceled(object sender, EventArgs e)
    {
        print("PopCanceled");
        Time.timeScale = 1f;
        velocity = 2 * popPower * direction;//double power
        if (true) //something to make sure there is no clash between pops

        {
            m_Rigidbody.velocity = velocity;
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
          //  runTimer = 0; //this is so the player resets run cycles after popping

        }
    }

    private void GameInput_OnDodgePopAction_canceled(object sender, EventArgs e)
    {
        print("Pop  Canceled");
        Time.timeScale = 1f;
        velocity = direction * 1 * popPower;
        if (true) //something to make sure there is no clash between pops

        {
            m_Rigidbody.velocity = velocity;
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
            runTimer = 0; //this is so the player resets run cycles after popping


        }
    }

    private void GameInput_OnPopAction_started(object sender, EventArgs e)
    {
        Time.timeScale = 0.6f;
        OnPlayerModeChange?.Invoke(this, new OnPlayerMode
        {
            characterMode = CharacterMode.Kernel
        });
    }

    private void GameInput_OnDodgePopAction_started(object sender, EventArgs e)
    {

        Time.timeScale = 0.6f;
        OnPlayerModeChange?.Invoke(this, new OnPlayerMode
        {
            characterMode = CharacterMode.Kernel
        });
    }




    private void GameInput_OnPopAction_completed(object sender, EventArgs e)
    {
        print("PopCompleted");
        Time.timeScale = 1f;
        velocity = direction * 2 * popPower;
        if (true) //something to make sure there is no clash between pops

        {
            m_Rigidbody.velocity = velocity;
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
            
           
        }


    }

    private void GameInput_OnDodgePopAction_completed(object sender, EventArgs e)
    {
        
        print("DodgeCompleted");
        Time.timeScale = 1f;
        velocity = direction * popPower;
        if (true) //something to make sure there is no clash between pops
        {
            m_Rigidbody.velocity = velocity;
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
        }
    }

   

    // Update is called once per frame
    void FixedUpdate()
    {

      
       
       // m_Animator.SetBool("canPop", canPop);

    }

    private void CharacterDamaged(float _damage)
    {
        m_Rigidbody.AddForce(new Vector2(2, 2), ForceMode2D.Impulse);

    }
    private float SetAnimationCurve(float runSpeed, AnimationCurve runSpeedCurve)
    {
        return runSpeedCurve.Evaluate(runTimer);
    }



/* 
 * ---------------All of these need to be moved to visual -------------------
 * private void KernelPopStart(Vector2 power)
    {
        //   KernelToPop(dodgeLoadTime, direction * 1 * popPower);
        OnPlayerModeChange?.Invoke(this, new OnPlayerMode
        {
            characterMode = CharacterMode.Kernel
        });

        Time.timeScale = 0.6f;// 
        colliderSize.Set(6.940126f, 6.940126f);
        m_Capsule.size = colliderSize;
        colliderOffset.Set(0.1427921f, 0.08117199f);
        m_Capsule.offset = colliderOffset;
        ChangeRbPhysics(m_Rigidbody, 5, 10, 200);
    }-------------------------------------------------------------------*/

    void KernelToPop(float popLoadTime, Vector2 _velocity)
    {
        bool canSlomo = true;

        

        PopTimer += Time.deltaTime;

        if ((PopTimer > popLoadTime || playerController.IsPopButtonReleased) && !receivedPush)
        {
            // if (playerController.isKernelModeLocked) return;

            



            playerController.popSoundEffect.Play();
           // sprite.enabled = true;
            playerController.cornKernel.SetActive(false);

           
            receivedPush = true;
            playerController.HasPopInput = true;
            return;
            //return true;
        }
        if (canSlomo)//if (timer <= loadTimer + popLoadTime)
        {
           
            
            Time.timeScale = 0.6f;// 
            colliderSize.Set(6.940126f, 6.940126f);
            m_Capsule.size = colliderSize;
            colliderOffset.Set(0.1427921f, 0.08117199f);
            m_Capsule.offset = colliderOffset;
            ChangeRbPhysics(m_Rigidbody, 5, 10, 200);
        }

        if (receivedPush && (PopTimer > popLoadTime  || playerController.IsPopButtonReleased))
        {
            //Change state and reset temporary paramaters
           
            PopTimer = 0;
            canSlomo = false;
            if (canPop) canPop = false;
            if (canDodge) canDodge = false;
           // m_Animator.SetBool("canPop", false);
            receivedPush = false;
            colliderSize.Set(12.9884f, 20.48f);
            m_Capsule.size = colliderSize;

            colliderOffset.Set(0.1783689f, 0);
            m_Capsule.offset = colliderOffset;
            return;
        }
        //DrawTrajectory(_velocity, m_LineRenderer);

        //check the timer  variables a video on Time time maybe??
        //loadTime = 0f;
      
       
    }

  /*  private Vector3[] DrawTrajectory(Vector2 _velocity, LineRenderer lineRenderer)
    {
        Vector2[] trajectory = Plot((Vector2)transform.position,
                                       _velocity, 250);

        lineRenderer.positionCount = trajectory.Length;
        Vector3[] positions = new Vector3[trajectory.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = trajectory[i];
        }
        lineRenderer.SetPositions(positions);
        return positions;
    }*/



    public void ChangeRbPhysics(Rigidbody2D rigidbody, float gravityScale, float drag, float power)
    {
        rigidbody.gravityScale = gravityScale;
        rigidbody.drag = drag;
        this.popPower = power;
    }

    void PopCornRun(Vector2 _velocity)
    {
        m_Rigidbody.velocity = velocity;
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

    public bool IsGrounded()
    {
        bool landed = false;
        float extraHeightText = 0.1f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(m_Capsule.bounds.center, m_Capsule.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        if(raycastHit.collider != null)
        {
            if (landed)
                return true;
            OnPlayerGrounded?.Invoke(this, EventArgs.Empty);
            landed = true;
        }
        else {
            
        }
        Debug.DrawRay(m_Capsule.bounds.center + new Vector3(m_Capsule.bounds.extents.x, 0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, 0), Vector2.down * (m_Capsule.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(m_Capsule.bounds.center - new Vector3(m_Capsule.bounds.extents.x, m_Capsule.bounds.extents.y + extraHeightText), Vector2.down * (m_Capsule.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
    }
}
