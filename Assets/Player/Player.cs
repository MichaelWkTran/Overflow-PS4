using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;

    [Header("Dash Variables")]
    public float dashSpeed;
    public float dashHeight;
    public float dashInputDistance; //How far does the player has to draw their finger or mouse

    [Header("Grounded Variables")]
    [SerializeField] bool showGroundCheck;
    public bool isGrounded { get; private set; }
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] LayerMask groundLayerMask;

    [Header("Particles")]
    [SerializeField] ParticleSystem jumpParticle;
    public ParticleSystem deathParticle;

    [Header("Audio")]
    [SerializeField] AudioClip m_jumpSound;
    [SerializeField] AudioClip m_landSound;
    [SerializeField] AudioClip m_deathSound;

    [Header("Other")]
    public SpriteRenderer m_hatSpriteRenderer;

    GameManager m_gameManager;
    Rigidbody2D m_rb;
    Animator m_animator;
    AudioSource m_m_audioSource;

    void Start()
    {
        //Get Components
        m_gameManager = FindObjectOfType<GameManager>();
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_m_audioSource = GetComponent<AudioSource>();

        //Get the Current Skin
        //if (ShopItem.SkinData.m_currentSkin != null) ShopItem.SkinData.m_currentSkin.m_skin.SetSkin();
    }

    void OnDestroy()
    {
        //Play the death animation
        {
            deathParticle.transform.parent = transform.parent;
            deathParticle.gameObject.SetActive(true);
            deathParticle.Play();
        }

        //Play the screen flash animation
        {
            m_gameManager.m_screenFlash.gameObject.SetActive(true);
            m_gameManager.m_screenFlash.GetComponent<Animator>().Play("Screen Flash");
            Destroy(m_gameManager.m_screenFlash, 1.0f);
        }
        
        //Play the death sound effect
        {
            AudioSource deathSoundSource = new GameObject().AddComponent<AudioSource>();
            deathSoundSource.outputAudioMixerGroup = m_m_audioSource.outputAudioMixerGroup;
            deathSoundSource.clip = m_deathSound;
            deathSoundSource.transform.position = transform.position;
            Destroy(deathSoundSource.gameObject, m_deathSound.length);
            deathSoundSource.Play();
        }

        //Open the game over screen
        m_gameManager.Invoke("triggerGameOver", 1.0f);
    }

    void Update()
    {
        //Check whether the player is on the ground
        isGrounded = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0, groundLayerMask);

        //Stop the player velocity when grounded
        if (isGrounded && m_rb.velocity.y <= 0)
        {
            m_rb.velocity = new Vector2(0.0f, m_rb.velocity.y);
        }

        //Player Movment
        if (InputUtilities.CanUseJumpInput() && isGrounded)
        {
            if (m_rb.velocity.y <= 0.0f) m_gameManager.AddScore(1U);
            Jump();
        }

        //Defeat the player if they fall down a pit
        if (transform.position.y < -2.0f) Destroy(gameObject);
    }

    void LateUpdate()
    {
        //Set Animations
        if (isGrounded && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            m_animator.Play("Land");
            if (m_rb.velocity.y <= 0.0f)
            {
                m_m_audioSource.clip = m_landSound;
                m_m_audioSource.Play();
            }
            
        }
        else if (isGrounded) { }
        else if (m_rb.velocity.y > 0.0f) m_animator.Play("Jump");
        //else m_animator.Play("Fall");
    }

    public void Jump()
    {
        Vector2 velocity;
        velocity.x = moveSpeed;
        velocity.y = Mathf.Sqrt(2.0f * Physics2D.gravity.magnitude * m_rb.gravityScale * jumpHeight);

        m_rb.velocity = velocity;
        jumpParticle.Play();
        m_m_audioSource.Stop();
        m_m_audioSource.clip = m_jumpSound;
        m_m_audioSource.Play();
    }

    void OnDrawGizmosSelected()
    {
        //Show Ground Check
        if (showGroundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckOffset, groundCheckSize);
        }
    }

    void OnTriggerEnter2D(Collider2D _collision)
    {
        //Kill player
        if (_collision.tag == "Obstacle")
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D _collision)
    {
        //Kill player
        if (_collision.transform.tag == "Obstacle")
        {
            Destroy(gameObject);
            return;
        }
    }
}
