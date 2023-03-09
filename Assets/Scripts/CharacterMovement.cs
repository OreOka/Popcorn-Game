using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.U2D;


public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement Instance { get; private set; }


    public event EventHandler<OnPopTypeChangedEventArgs> OnPlayerPop;
    public event EventHandler<OnPlayerMode> OnPlayerModeChange;

    public class OnPopTypeChangedEventArgs : EventArgs
    {
        public PopType popType;
    }

    public class OnPlayerMode : EventArgs
    {
        public CharacterMode characterMode;
    }

    public enum PopType
    {
        Dodge,
        Regular,
    }

    public enum CharacterMode
    {
        PopCorn,
        Kernel,

    }
    

    [SerializeField] private float popLoadTime = 0.4f;
    [SerializeField] private float dodgeLoadTime = 0.2f;
    [SerializeField] private  float dodgeTime = 0.06f;

    // Start is called before the first frame update

    public float runSpeed = 10f;
    float popPower = 5f;

    PlayerController playerController;

   

    private CapsuleCollider2D m_Capsule;

    private Rigidbody2D m_Rigidbody;
    private LineRenderer m_LineRenderer;
    [SerializeField] AnimationCurve runSpeedCurve;
    private SpriteRenderer sprite;

    Vector2[] trajectory;
    public Vector2 direction;
    private bool receivedPush = false;
    public Vector2 velocity;
    private float PopTimer = 0;
    private float DodgeTimer = 0;
    [SerializeField] float dodgeSpeed = 5f;
    private Vector2 colliderSize;
    private Vector2 colliderOffset;
    private bool canPop;
    private Animator m_Animator;
    private bool canDodge;

    void Awake()
    {
        Instance = this;

        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer = GetComponent<LineRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        playerController = GetComponent<PlayerController>();
        CharacterEvents.characterDamaged += CharacterDamaged;
    }

    void Start()
    {
        canPop = false;

    }
    private void Update()
    {
        direction = playerController.lStickDirection;

        SetAnimationCurve(dodgeSpeed, runSpeedCurve);

        if (playerController.HasPopInput)
        {
            playerController.HasPopInput = false;
         //   m_Animator.SetBool("canPop", true);
            canPop = true;
        }
        if (playerController.HasDodgeInput)
        {
            playerController.HasDodgeInput = false;
            canDodge = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //this is to set the default direction of the controller to face the sky so there always a defualt movement for the character
        if (Math.Abs( direction.x) < 0.5f && Math.Abs(direction.y) < 0.5f)

        {
            direction.Set(0, 1);
        }

        if (canPop)
        {
            velocity = direction * 2 * popPower;
            KernelPop(popLoadTime, direction * 2 * popPower);
        } else 
        
        if (canDodge)
        {
           //DodgePop(dodgeTime);
            KernelPop(dodgeLoadTime, direction * 1 * popPower);
           
        } else 
        if (playerController.HasRunInput )//&& !canPop && !canDodge)
        {
            velocity.Set(direction.x * playerController.runSpeed, m_Rigidbody.velocity.y);
            PopCornRun(velocity);
        }
       
       // m_Animator.SetBool("canPop", canPop);

    }

    private void CharacterDamaged(float _damage, GameObject character)
    {
        m_Rigidbody.AddForce(new Vector2(2, 2), ForceMode2D.Impulse);

    }
    private float SetAnimationCurve(float runSpeed, AnimationCurve runSpeedCurve)
    {
        return runSpeed =300* runSpeedCurve.Evaluate(DodgeTimer);
    }
    void KernelPop(float popLoadTime, Vector2 _velocity)
    {
        bool canSlomo = true;

        

        PopTimer += Time.deltaTime;

        if ((PopTimer > popLoadTime || playerController.IsPopButtonReleased) && !receivedPush)
        {
            // if (playerController.isKernelModeLocked) return;

            if (canDodge)
            {
                OnPlayerPop?.Invoke(this, new OnPopTypeChangedEventArgs
                {
                    popType = PopType.Dodge
                });
            }
            else
            {
                OnPlayerPop?.Invoke(this, new OnPopTypeChangedEventArgs
                {
                    popType = PopType.Regular
                });
            }



            playerController.popSoundEffect.Play();
           // sprite.enabled = true;
            playerController.cornKernel.SetActive(false);

            m_Rigidbody.velocity = _velocity;
            receivedPush = true;
            playerController.HasPopInput = true;
            return;
            //return true;
        }
        if (canSlomo)//if (timer <= loadTimer + popLoadTime)
        {
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
        }

        if (receivedPush && (PopTimer > popLoadTime + dodgeTime || playerController.IsPopButtonReleased))
        {
            //Change state and reset temporary paramaters
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
            Time.timeScale = 1f;// 
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

    private Vector3[] DrawTrajectory(Vector2 _velocity, LineRenderer lineRenderer)
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
    }

    private void DodgePop(float dodgeTime)
    {
        Vector2 _velocity = direction * dodgeSpeed;
        DodgeTimer += Time.deltaTime;
      //  ChangeRbPhysics(m_Rigidbody, 1, 0, 200);
        
        if(DodgeTimer < dodgeTime)
        {
            Time.timeScale = 0.6f;
           
            m_Rigidbody.MovePosition((Vector2)transform.position + (_velocity * Time.deltaTime));
           
        }
        else
        {
            Time.timeScale = 1f;
            DodgeTimer = 0;
            OnPlayerModeChange?.Invoke(this, new OnPlayerMode
            {
                characterMode = CharacterMode.PopCorn
            });
            canDodge = false;
        }

    }

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

   
}
