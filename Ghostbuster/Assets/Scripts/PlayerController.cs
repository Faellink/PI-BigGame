using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;

    [SerializeField]private const float NORMAL_FOV = 60f;
    [SerializeField]private const float HOOKSHOT_FOV = 120f;

    public ParticleSystem speedLinesParticleSystem;
    public CharacterController controller;
    public GameObject playerCamera;
    public CameraFov cameraFov;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float hookshotThrowSpeed = 150f;

    public float groundDistance = 0.4f;
    bool isGrounded;

    public float hookshotRange = 100f;
    public bool hookshotEnabled;

    float moveY;

    private float hookshotSize;

    public float jumpSpeed = 40f;
    public float jumpForce = 3f;
    public float speed = 10f;
    public float gravity = -9.81f;

    GameObject cidadao;
    public Transform carregandoCidadaoOmbro;

    Vector3 reversedMove;

    Vector3 move;
    Vector3 velocity;
    Vector3 characterVelocityMomentum;
    Vector3 hookshotPosition;

    private State state;

    private enum State
    {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void Awake()
    {
        cameraFov = playerCamera.GetComponent<CameraFov>();
        speedLinesParticleSystem = GetComponentInChildren<ParticleSystem>();
        state = State.Normal;
        hookshotTransform.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance , groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        */
        switch (state)
        {
            default:
            case State.Normal:
                Movement();
                HookshotStart();
                break;
            case State.HookshotThrown:
                HandleHookshotThrow();
                Movement();
                break;
            case State.HookshotFlyingPlayer:
                HookshotMovement();
                break;

        }
        

    }

    private void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        /*if (isGrounded && move.y < 0)
        {
            move.y = -2f;
        }*/

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        //Vector3 move = transform.right * x * speed + transform.forward * z * speed;
        //controller.Move(move * speed * Time.deltaTime);

        move = transform.right * x * speed + transform.forward * z * speed;

        if (isGrounded)
        {
            moveY = 0f;
            //velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);//
            if (TestInputJump())
            {
                moveY = jumpForce;
            }
        }

        moveY += gravity * Time.deltaTime;

        move.y = moveY;

        move += characterVelocityMomentum;

        controller.Move(move * Time.deltaTime);

        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }

        //Debug.Log(characterVelocityMomentum);
        //Debug.Log(characterVelocityMomentum.magnitude);

        //Debug.Log(move.y);
        //Debug.Log(moveY);
    }

    private void HookshotStart()
    {
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, hookshotRange))
        {
            hookshotEnabled = true;
        }
        else
        {
            hookshotEnabled = false;
        }

        if (TestInputHookshot())
        {
            if(Physics.Raycast(playerCamera.transform.position , playerCamera.transform.forward, out RaycastHit raycastHit))
            {
                debugHitPointTransform.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                hookshotSize = 0f;
                hookshotTransform.gameObject.SetActive(true);
                hookshotTransform.localScale = Vector3.zero;
                state = State.HookshotThrown;
            }
        }
    }

    private void HandleHookshotThrow()
    {
        hookshotTransform.LookAt(hookshotPosition);

        //float hookshotThrowSpeed = 150f;
        hookshotSize += hookshotThrowSpeed * Time.deltaTime;
        hookshotTransform.localScale = new Vector3(1,1, hookshotSize);

        if (hookshotSize >= Vector3.Distance(transform.position, hookshotPosition))
        {
            state = State.HookshotFlyingPlayer;
            cameraFov.SetCameraFov(HOOKSHOT_FOV);
            speedLinesParticleSystem.Play();
        }
    }

    private void HookshotMovement()
    {
        hookshotTransform.LookAt(hookshotPosition);

        Vector3 hookshotDirection = (hookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        controller.Move(hookshotDirection * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotDistance = 1f;

        if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotDistance)
        {
            StopHookshot();
        }

        if (TestInputHookshot())
        {
            StopHookshot();
        }

        if (TestInputJump())
        {
            float momentumExtraSpeed = 1f;
            characterVelocityMomentum = hookshotDirection * hookshotSpeed * momentumExtraSpeed;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            StopHookshot();
        }

    }

    private void StopHookshot()
    {
        state = State.Normal;
        ResetGravity();
        hookshotTransform.gameObject.SetActive(false);
        cameraFov.SetCameraFov(NORMAL_FOV);
        speedLinesParticleSystem.Stop();
    }

    private bool TestInputHookshot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private void ResetGravity()
    {
        moveY = 0f;
    }

    void SavingCidadao()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cidadao.gameObject.transform.position = carregandoCidadaoOmbro.position;
            Debug.Log("pegou");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isGrounded && hit.normal.y < 0.1f)
        {
            if (TestInputJump())
            {
                Vector3 testVector = Vector3.Reflect(move, hit.normal);

                if (move.y <= 0)
                {
                    //falling
                    reversedMove = Vector3.Reflect(hit.moveDirection, move);
                    Debug.DrawRay(transform.position, reversedMove, Color.yellow, 10f);
                }
                else
                {
                    //up
                    reversedMove = Vector3.Reflect(move, hit.moveDirection);
                    Debug.DrawRay(transform.position, reversedMove, Color.red, 10f);
                }

                Debug.DrawRay(transform.position, testVector, Color.white, 10f);

                Debug.DrawRay(transform.position, hit.moveDirection, Color.green, 10f);
                Debug.DrawRay(transform.position, hit.normal, Color.black, 10f);
                //Debug.Break();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cidadao"))
        {
            cidadao = other.gameObject;

            Debug.Log("encostou cidadao");
        }
    }
}
