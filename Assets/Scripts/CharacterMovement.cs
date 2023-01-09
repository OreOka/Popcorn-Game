using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.U2D;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float runSpeed = 10f;
    public float power = 5f;

    public PlayerController playerController;

   

    private CapsuleCollider2D m_Capsule;

    private Rigidbody2D m_Rigidbody;
    private LineRenderer m_LineRenderer;
    [SerializeField] AnimationCurve runSpeedCurve;
    private SpriteRenderer sprite;

    Vector2[] trajectory;
    public Vector2 direction;
    private bool receivedPush = false;
    public Vector2 velocity;
    public float timer = 0;
    private Vector2 colliderSize;
    private Vector2 colliderOffset;
    private bool canPop;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_LineRenderer = GetComponent<LineRenderer>();
        sprite = GetComponent<SpriteRenderer>();
       
        m_Capsule = GetComponent<CapsuleCollider2D>();
        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        canPop = false;
    }

    // Update is called once per frame
    void Update()
    {
        direction = playerController.lStickDirection;
        

        if (playerController.HasPopInput)
        {
            playerController.HasPopInput = false;
            Debug.Log("Can Pop");
            canPop = true;
        }
        if (canPop)
        {
            velocity = direction * 2 * power;
            KernelPop(m_LineRenderer, 0.4f, direction * 2 * power);
        }
        if (playerController.HasRunInput && !canPop)
        {
            velocity.Set(direction.x * playerController.runSpeed, m_Rigidbody.velocity.y);
            PopCornRun(velocity);
        }



    }

    public void ChangeRbPhysics(Rigidbody2D rigidbody, float gravityScale, float drag, float power)
    {
        rigidbody.gravityScale = gravityScale;
        rigidbody.drag = drag;
        this.power = power;
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
    void KernelPop(LineRenderer lineRenderer, float popLoadTime, Vector2 _velocity)
    {
        bool canSlomo = true;
        
        timer += Time.deltaTime;
        Vector2[] trajectory = Plot((Vector2)transform.position,
                                       _velocity, 250);


        if (receivedPush && timer > popLoadTime+0.1f)
        {
            //Change state and reset temporary paramaters
            Time.timeScale = 1f;// 
            timer = 0;
            canSlomo = false;
            canPop = false;
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
        if ((timer >popLoadTime || Input.GetButtonUp("Pop") )&& !receivedPush )
        {
            CharacterEvents.characterPopped.Invoke("Regular", gameObject);


            


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
}
