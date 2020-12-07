using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public TMPro.TMP_Text hl;
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;

    [SerializeField] private const float NORMAL_FOV = 60f;
    [SerializeField] private const float HOOKSHOT_FOV = 120f;

    public ParticleSystem speedLinesParticleSystem;
    public Animator anim;
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

    public GameObject hookshotAnim;

    private State state;

    [Header("Health")]
    public int health;

    [Header("succing")]
    public float radius;
    public float strength;
    public float falloffDistance;
    public float succAngle;
    public ZombieAI suckingTarget;
    public float timeSucking;
    private float finishSuckTime;
    public GameObject gunProjectileThing;
    public float projectileForce;
    public int projectileAmmo;
    public Transform gunPoint;
    [Header("animation")]
    public float succAnimationPos;
    public float shotAnimationPos;
    public float idleAnimationPos;
    public Transform gun;

    [Header("Pickign NPC")]
    public float pickupRange;

    public ZombieAI carryNPC;
    public Transform shoulderTransform;
    public Transform safePoint;
    public LayerMask carryLayerMask;
    public Text tex;

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

    void Update()
    {
        hl.text = string.Format("Vida: {0}", health);
        switch (state)
        {
            default:
            case State.Normal:
                Movement();
                HookshotStart();
                hookshotAnim.SetActive(false);
                break;
            case State.HookshotThrown:
                hookshotAnim.SetActive(false);
                HandleHookshotThrow();
                Movement();
                break;
            case State.HookshotFlyingPlayer:
                hookshotAnim.SetActive(true);
                HookshotMovement();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Suck();
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            anim.SetBool("Sucking", true);
            KeepSucking();
        }
        else
        {
            anim.SetBool("Sucking", false);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopSucking();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootProjectile();
        }

        // animation hack
        var p = gun.localPosition;
        var animState = anim.GetCurrentAnimatorStateInfo(0);
        if(animState.IsName("Succ"))
        {
            p.z = succAnimationPos;
        }
        else if(animState.IsName("Shoot"))
        {
            p.z = shotAnimationPos;
        }
        else if(animState.IsName("Idle"))
        {
            p.z = idleAnimationPos;
        }
        gun.localPosition = p;
    }

    public void Suck()
    {
        ZombieAI nearest = null;
        // get succables nearby
        foreach(var obj in Physics.OverlapSphere(transform.position,radius))
        {
            if (obj.GetComponent<ZombieAI>() && obj.GetComponent<ZombieAI>().hostile)
            {
                float ang = Vector3.Angle(transform.forward, (obj.transform.position - gunPoint.position).normalized);
                if (ang < succAngle)
                    nearest = obj.GetComponent<ZombieAI>();
            }
        }

        if(nearest != null)
        {
            finishSuckTime = Time.time + timeSucking;
            suckingTarget = nearest;
            suckingTarget.suckEffect.gameObject.SetActive(true);
            suckingTarget.StartSuck();
            suckingTarget.Stun();
        }
    }

    public void KeepSucking()
    {
        if(suckingTarget == null)
        {
            Suck();
            return;
        }
        if(Vector3.Distance(transform.position,suckingTarget.transform.position) > falloffDistance)
        {
            StopSucking();
            return;
        }
        float fac = (finishSuckTime - Time.time) / timeSucking;

        Vector3 newPos = Vector3.Lerp(gunPoint.position, suckingTarget.transform.position, fac);
        suckingTarget.suckEffect.position = newPos;
        if (Time.time > finishSuckTime)
        {
            suckingTarget.suckEffect.gameObject.SetActive(false);
            suckingTarget.TakeEnemyDamage();
            suckingTarget.StopStun();
            projectileAmmo++;
            suckingTarget.EndSuck();
            suckingTarget = null;
        }
    }

    public void StopSucking()
    {
        if (suckingTarget == null)
        {
            return;
        }
        suckingTarget.CancelSuck();
        suckingTarget.suckEffect.gameObject.SetActive(false);
        suckingTarget.StopStun();
        suckingTarget = null;
    }

    public void ShootProjectile()
    {
        if(projectileAmmo <= 0)
        {
            return;
        }
        anim.SetTrigger("Shot");

        projectileAmmo--;
        GameObject obj = Instantiate(gunProjectileThing);
        obj.transform.position = gunPoint.position;
        obj.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * projectileForce, ForceMode.Impulse);
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

    public void TakeDamage()
    {
        health--;
        if(health <= 0)
        {
            foreach(ZombieAI ai in GameObject.FindObjectsOfTypeAll(typeof(ZombieAI)))
            {
                if(ai.hostile)
                {
                    ai.anim.SetTrigger("Meme");
                    ai.enabled = false;
                }
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
