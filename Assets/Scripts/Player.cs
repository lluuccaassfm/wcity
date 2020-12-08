using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 5;
    private int direction = 1;
    [Space]
    [Header("Jump")]
    public float jumpForce = 85;
    public bool grounded;
    public Transform groundedCheck;
    public float groundedCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    public AudioClip jumpSound;
    [Space]
    [Header("Shooting")]
    public GameObject wormPrefab;
    public Transform shootingPosition;
    public Transform shootingPositionRight;
    public Transform shootingPositionLeft;
    public float wormSpeed = 5;
    public AudioClip shootingSound;
    [Space]
    [Header("Health")]
    public int playerHP = 3;
    public bool isAlive = true;
    public Slider hpBar;
    public AudioClip dyingSound;
    public AudioClip damageSound;
    public AudioClip lifeSound;
    [Space]
    [Header("Coins")]
    public int qtdCoins = 0;
    public AudioClip coinSound;
    public Text textCoins;

    //REFERENCES
    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spr;
    private AudioSource aud;


    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        aud = GetComponent<AudioSource>();       
        hpBar.maxValue = playerHP;
        textCoins.text = "0";
    }

    void FixedUpdate()
    {
        hpBar.value = playerHP;
        if(isAlive){
            float h = Input.GetAxis("Horizontal");  

            rig.velocity = new Vector2(movementSpeed*h, rig.velocity.y);
            anim.SetFloat("speed", Mathf.Abs(h));

            if(h > 0) Flip(true);
            else if(h < 0) Flip(false);

            grounded = Physics2D.OverlapCircle(groundedCheck.position, groundedCheckRadius, whatIsGround);

            anim.SetBool("grounded", grounded);

            if(Input.GetButton("Jump") && grounded){
                aud.clip = jumpSound;
                aud.Play();
                rig.AddForce(Vector2.up * jumpForce); 
            }           
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            aud.clip = shootingSound;
            aud.Play();
            anim.SetTrigger("shooting");
            GameObject worm;
            if(direction == 1){
                worm = Instantiate(wormPrefab, shootingPositionRight.position, Quaternion.identity);
            }else{
                worm = Instantiate(wormPrefab, shootingPositionLeft.position, Quaternion.identity);
                worm.GetComponent<SpriteRenderer>().flipX = false;
            }
            worm.GetComponent<Rigidbody2D>().velocity = new Vector2(wormSpeed * direction, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Dead"){
            DestroyPlayer();
        }

        if(other.gameObject.tag == "Finish"){
            SceneManager.LoadScene(3);
        }

        if(other.gameObject.tag == "Coin"){
            Destroy(other.gameObject);
            TakeCoin();
        }

        if(other.gameObject.tag == "HeartLife"){
            Destroy(other.gameObject);
            TakeLife();
        }
    }

    void Flip(bool faceRight){
        if(faceRight){
            direction = 1;
            spr.flipX = false;
        }else{
            direction = -1;
            spr.flipX = true;
        }
    }

    public void TakeCoin(){
        aud.clip = coinSound;
        aud.Play();
        qtdCoins += 1;
        textCoins.text = qtdCoins.ToString();
    }

    public void TakeLife(){
        if(playerHP < 3){
            playerHP += 1;
        }
        aud.clip = lifeSound;
        aud.Play();
    }

    public void TakeDamage(int damage){
        if(isAlive){
            playerHP -= damage;
            aud.clip = damageSound;
            aud.Play();
        }

        if(playerHP <= 0 && isAlive){
            aud.clip = dyingSound;
            aud.Play();
            anim.SetTrigger("dead");  
            isAlive = false;  
            Invoke("DestroyPlayer",1.5f);
        }
    }

    void DestroyPlayer(){
        gameObject.SetActive(false);
        Invoke("RespawnPlayer", 1);
        SceneManager.LoadScene(2);
    }

    void RespawnPlayer(){
        gameObject.SetActive(true);
        isAlive = true;
        playerHP = 3;
    }
}
