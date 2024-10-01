using UnityEngine;

public class Player_controler : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float decelerationSpeed = 5f;
    [SerializeField]
    float frenado = 5f;

    [SerializeField]
    float dashDistance = 5.0f;
    [SerializeField]
    float dashDuration = 0.3f;
    [SerializeField]
    float dashForce = 10f;
    [SerializeField]
    float dashForceForAttacks = 15f;
    [SerializeField]
    bool isDashing;
    [SerializeField]
    public bool requestDashAI { get; set; }

    [SerializeField]
    float jumpForce = 1.0f;

    Rigidbody rb;
    Vector3 dashStartPosition;
    Vector3 dashEndPosition;
    float dashStartTime;
    bool isGrounded;
    bool isInvincible = false;
    Animator animator;
    bool isRunning = false;
    bool isRecievingAttack = false;

    [SerializeField]
    public bool isAttack;

    [SerializeField]
    Animator animatorCam;
    // Establece el fps objetivo (por ejemplo, 60 fps)
    public int targetFramerate = 60;
    public Vector3 moveDirection;
    void Start()
    {
        // Establece el fps objetivo al iniciar el juego
        Application.targetFrameRate = targetFramerate;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

    }
    public void ActivateDash()
    {
        requestDashAI = true;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (!isDashing)
        {
            // Movimiento
            //float horizontalInput = Input.GetAxis("Horizontal");
            //float verticalInput = Input.GetAxis("Vertical");
            //moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            //if (Input.GetButtonDown("Dash"))
            //{
            //    if (CanDash())
            //    {
            //        StartDash(moveDirection);
            //        startDash = true;
            //        audioSource.PlayOneShot(dash_clip);
            //    }
            //}
            if (requestDashAI)
            {
                if (CanDash())
                {
                    StartDash(moveDirection);
                    //audioSource.PlayOneShot(dash_clip);
                }

            }
            requestDashAI = false;

        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            // Movimiento
            //float horizontalInput = Input.GetAxis("Horizontal");
            //float verticalInput = Input.GetAxis("Vertical");
            //moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            
            // Gira el personaje hacia la dirección del movimiento
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            if (!isAttack)
            {
                rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
                if (moveDirection != Vector3.zero)
                {
                    StartRun();
                }
                else
                {
                    StopRun();
                    //Debug.Log("parao");
                }
            }
        }
        else
        {
            if (isRecievingAttack)
                PerformDash(dashForceForAttacks);
            else
                PerformDash();

        }

    }



    bool CanDash()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
    {
        if (isGrounded)
        {
            return true;
        }
        return false;
    }
    void StartRun()
    {        
        isRunning = true;

        //Debug.Log("corriendo");
        animator.SetFloat("XSpeed", 1);
            animator.SetBool("IsRunning", true);
        
    }

    public void StopRun()
    {
        isRunning = false;

        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsRunning", false);
        }
        //rb.velocity = new Vector3(0, transform.position.y, 0);

    }
    void StartDash(Vector3 dashDirection)
    {
        //dashImage.sprite = dashEnabledSprite;
        //dashPS.Play();
        isDashing = true;
        dashStartPosition = transform.position;
        // Agregar un pequeño impulso vertical
        float verticalImpulse = 0.5f; // Modifica este valor según sea necesario
        Vector3 dashDirectionWithImpulse = dashDirection.normalized + Vector3.up * verticalImpulse;
        dashEndPosition = transform.position + dashDirection.normalized * dashDistance;
        dashStartTime = Time.fixedTime;
        // Activa la invulnerabilidad durante el dash.
        isInvincible = true;
        // Activa la animación de dash
        if (animator != null)
        {
            animator.SetTrigger("DashTrigger"); 
        }
    }

    void PerformDash()
    {
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;

        if (dashProgress < 1.0f)
        {
            // Calcula la dirección del dash en el plano horizontal
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            dashDirection.y = 0; // Ignora completamente el movimiento vertical

            // Aplica un impulso al Rigidbody en la dirección del dash
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            // Aplica un pequeño impulso vertical para mantener al jugador pegado al suelo
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            // Finaliza el dash y restablece el movimiento normal.
            isDashing = false;
            isInvincible = false;

            // Detiene completamente el movimiento en el eje Y para evitar cualquier elevación o caída
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //dashImage.sprite = dashDisabledSprite;

        }
    }
    void PerformDash(float strengthDash)
    {
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;


        if (dashProgress < 1.0f)
        {
            // Interpola suavemente la posición del personaje durante el dash.
            // Utiliza la curva de interpolación para un movimiento más suave 
            //float interpolation = dashCurve.Evaluate(dashProgress);

            // Calcula la dirección del dash y aplica un impulso al Rigidbody.
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            rb.AddForce(dashDirection * strengthDash /***/ /*interpolation*/, ForceMode.Impulse);
        }
        else
        {
            // Finaliza el dash y restablece el movimiento normal.
            isDashing = false;
            isInvincible = false;
            isRecievingAttack = false;
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
        }
    }
    public Vector3 ReturnVelocityRigidbody()
    {
        return rb.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica si el jugador está en el suelo y no es invulnerable.
        if (collision.collider.CompareTag("Ground") && !isInvincible)
        {
            isDashing = false;
            isInvincible = false;
        }      
    }
}
