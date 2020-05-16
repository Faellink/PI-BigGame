using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;

    [SerializeField] private const float NORMAL_FOV = 60f;
    [SerializeField] private const float HOOKSHOT_FOV = 120f;

    public ParticleSystem speedLinesParticleSystem;
    public CharacterController controller;
    public GameObject playerCamera;
    public CameraFov cameraFov;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float hookshotThrowSpeed = 150f;

    //

    public bool pulouParede;
    public bool isParede;
    public Transform paredeCheck;
    public float paredeDistance = 0.8f;

    public float groundDistance = 0.4f;
    bool isGrounded;

    public float hookshotRange = 100f;
    public bool hookshotEnabled;

    float moveY;
    public float tempo;
    public float moveZ;

    private float hookshotSize;

    public float jumpSpeed = 40f;
    public float jumpForce = 3f;
    public float speed = 10f;
    public float gravity = -85f;
    //gravidade no inspector

    public bool resetGravityExecuted;

    GameObject cidadao;
    public Transform carregandoCidadaoOmbro;

    //

    public float wallJumpForce = 0f;
    public float wallJumpDrag = -10f;

    // 

    public GameObject cidadaoPrefab;
    public float forcaJogar = 0f;
    public bool carregarCidadao;

    [SerializeField] SavingCidadao cidadaoScrpt;

    Vector3 reversedMove;

    public Transform impactPoint;
    public bool bateuPredio;

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
        pulouParede = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("HookRafael");
        }

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Break();
        }

        JogandoCidadao();

    }

    private void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isParede = Physics.CheckCapsule(paredeCheck.position, new Vector3(paredeCheck.position.x, paredeCheck.position.y + 0.5f, paredeCheck.position.z), paredeDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x * speed + transform.forward * z * speed;


        if (isGrounded)
        {
            moveY = 0f;
            if (TestInputJump())
            {
                moveY = jumpForce;
            }
        }

        if (isParede)
        {
            moveY = 0f;
            gravity = 0f;
            if (TestInputJump())
            {
                moveY = jumpForce;
                moveZ = wallJumpForce;
               // moveZ += wallJumpDrag * Time.deltaTime;
                pulouParede = true;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                moveY = jumpForce;
                moveZ = wallJumpForce;
                moveZ += wallJumpDrag * Time.deltaTime;
                pulouParede = true;
            }
        }
        else
        {
            gravity = -85f;
        }        

        if (moveZ <= 0 && pulouParede)
        {
            Debug.Log("zerou");
        }
        //Debug.Log();

        moveY += gravity * Time.deltaTime;

        move.y = moveY;

        move += characterVelocityMomentum;


        Vector3 diagonal = transform.forward + transform.up;
        Vector3 diagonalTste = new Vector3(0f, 10f, 20f);
            

        Debug.DrawRay(transform.position, diagonalTste, Color.red, 10f);
        Debug.DrawRay(transform.position, diagonal, Color.yellow, 10f);
        
        //move.y = moveY;

        //move += characterVelocityMomentum;

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

    }

    private void HookshotStart()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, hookshotRange))
        {
            hookshotEnabled = true;
        }
        else
        {
            hookshotEnabled = false;
        }

        Debug.DrawRay(transform.position, playerCamera.transform.forward * hookshotRange, Color.red);

        if (TestInputHookshot())
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit))
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

        hookshotSize += hookshotThrowSpeed * Time.deltaTime;
        hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);

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

    bool TestJumpHold()
    {
        return Input.GetKey(KeyCode.Space);
    }

    private void ResetGravity()
    {
        moveY = 0f;
        //Debug.Log("reset gravity executed");
    }

    //

    void JogandoCidadao()
    {

        if (cidadao == null)
        {
            cidadaoScrpt = null;
        }
        else
        {
            cidadaoScrpt = cidadao.GetComponent<SavingCidadao>();

            if (cidadaoScrpt.cidadaoNoOmbro == true)
            {

                if (Input.GetKeyDown(KeyCode.Q))
                {

                    Destroy(cidadao);

                    GameObject cidadaoPrefabInst = Instantiate(cidadaoPrefab, carregandoCidadaoOmbro.position, Quaternion.identity) as GameObject;

                    Rigidbody[] cidadaoRB = cidadaoPrefabInst.GetComponentsInChildren<Rigidbody>();
                    foreach (var rigidbody in cidadaoRB)
                    {
                        rigidbody.AddForce(Camera.main.transform.forward * forcaJogar);
                    }

                }
            }
        }

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

        if (hit.gameObject.CompareTag("Predio"))
        {
            impactPoint.position = hit.point;

            bateuPredio = true;

            //gravity = 0f;
            //moveY = 0f;
            //characterVelocityMomentum = Vector3.zero;
        }
        else
        {
            bateuPredio = false;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cidadao"))
        {
            cidadao = other.gameObject;

            carregarCidadao = true;

            Debug.Log("encostou cidadao");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cidadao"))
        {
            carregarCidadao = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);

        Gizmos.DrawSphere(paredeCheck.position, paredeDistance);

    }

}
