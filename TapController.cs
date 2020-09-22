using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public GameObject frontWheel;
    public GameObject backWheel;
    public float tapForce = 10;
    public float tiltSmooth = 5;
    public float gravScale = 3;
    public float jumpTimer = 0.5f;
    public Vector3 startPos;

    private bool isGrounded = false;
    private bool pressedJump = false;
    private bool releasedJump = false;
    private bool startTimer = false;
    private float timer;
        
    Rigidbody2D rb2d;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
    Animator backAnim;
    Animator frontAnim;

    

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, 0);
        forwardRotation = Quaternion.Euler(0, 0, 50);
        timer = jumpTimer;
        game = GameManager.Instance;
        rb2d.simulated = false;

        frontAnim = frontWheel.GetComponent<Animator>();
        backAnim = backWheel.GetComponent<Animator>();
        frontAnim.enabled = false;
        backAnim.enabled = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rb2d.velocity = Vector3.zero;
        rb2d.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.GameOver) return;
        if (game.IsPaused) return;
        
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            pressedJump = true;

        }
        if (Input.GetMouseButtonUp(0)) 
        {
            releasedJump = true;
        }
        
        if (startTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                releasedJump = true;
            }
        }
        if (isGrounded)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
            frontAnim.enabled = true;
            backAnim.enabled = true;
        }
        
        
    }

    void FixedUpdate()
    {
        if (pressedJump && isGrounded)
        {
            StartJump();
        }

        if (releasedJump)
        {
            StopJump();
        }
        pressedJump = false;
    }
    

    void StartJump()
    {
        rb2d.gravityScale = 0;
        transform.rotation = forwardRotation;
        rb2d.AddForce(Vector2.up * tapForce, ForceMode2D.Impulse);
        pressedJump = false;
        startTimer = true;
    }

    void StopJump()
    {
        rb2d.gravityScale = gravScale;
        releasedJump = false;
        timer = jumpTimer;
        startTimer = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Scorezone")
        {
            //register score event
            OnPlayerScored(); //event sent to GameManager
            //play a sound
        }

        if (col.gameObject.tag == "Deadzone")
        {
            rb2d.simulated = false;
            frontAnim.enabled = false;
            backAnim.enabled = false;
            //dead event
            OnPlayerDied(); //event sent to GameManager
            //play sound
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.transform.tag == "Ground")
        {
            isGrounded = false;
        }
    }

}
