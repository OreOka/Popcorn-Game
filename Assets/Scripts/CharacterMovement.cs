using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.U2D;

public class CharacterMovement : MonoBehaviour
{
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
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer = GetComponent<LineRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        playerController = GetComponent<PlayerController>();
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
            m_Animator.SetBool("canPop", true);
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
        

        if (canPop)
        {
            velocity = direction * 2 * popPower;
            KernelPop(m_LineRenderer, 0.4f, direction * 2 * popPower);
        }
        
        if (canDodge)
        {
            DodgePop(dodgeTime);
        }
        if (playerController.HasRunInput && !canPop)
        {
            velocity.Set(direction.x * playerController.runSpeed, m_Rigidbody.velocity.y);
            PopCornRun(velocity);
        }
       


       // m_Animator.SetBool("canPop", canPop);


    }
    private float SetAnimationCurve(float runSpeed, AnimationCurve runSpeedCurve)
    {
        return runSpeed =300* runSpeedCurve.Evaluate(DodgeTimer);
    }
    void KernelPop(LineRenderer lineRenderer, float popLoadTime, Vector2 _velocity)
    {
        bool canSlomo = true;

        PopTimer += Time.deltaTime;
        Vector2[] trajectory = Plot((Vector2)transform.position,
                                       _velocity, 250);


        if (receivedPush && PopTimer > popLoadTime + dodgeTime)
        {
            //Change state and reset temporary paramaters
            Time.timeScale = 1f;// 
            PopTimer = 0;
            canSlomo = false;
            canPop = false;
            m_Animator.SetBool("canPop", false);
            receivedPush = false;
            colliderSize.Set(12.9884f, 20.48f);
            m_Capsule.size = colliderSize;

            colliderOffset.Set(0.1783689f, 0);
            m_Capsule.offset = colliderOffset;
            return;
        }
        lineRenderer.positionCount = trajectory.Length;
        Vector3[] positions = new Vector3[trajectory.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = trajectory[i];
        }

        lineRenderer.SetPositions(positions);



        //check the timer  variables a video on Time time maybe??
        //loadTime = 0f;
        if (/*(*/PopTimer > popLoadTime /*|| Input.GetButtonUp("Pop")) */&& !receivedPush)
        {
            CharacterEvents.characterPopped.Invoke("Regular", gameObject);
            CharacterEvents.characterMode.Invoke("PopCorn", gameObject);



            playerController.popSoundEffect.Play();
            sprite.enabled = true;
            playerController.cornKernel.SetActive(false);



            m_Rigidbody.velocity = _velocity;
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
    }

    private void DodgePop(float dodgeTime)
    {
        Vector2 _velocity = direction * dodgeSpeed;
        DodgeTimer += Time.deltaTime;
      //  ChangeRbPhysics(m_Rigidbody, 1, 0, 200);
        
        if(DodgeTimer < dodgeTime)
        {
            m_Rigidbody.MovePosition((Vector2)transform.position + (_velocity * Time.deltaTime));
           
        }
        else
        {
            DodgeTimer = 0;
            
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
        if (playerController.IsGrounded())
        {
            m_Rigidbody.velocity = velocity;

        }
        else
            m_Rigidbody.velocity.Set(velocity.x / 2, velocity.y);

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
