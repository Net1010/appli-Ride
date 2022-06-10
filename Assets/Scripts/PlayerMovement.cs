using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private SectionSpawner sectionSpawner;

    private bool layoutTouch;       //indicates if ui element is being touched

    public static float heightThreshold, gravityScale = Physics.gravity.y * 0.02f, jumpForce = 2f;

    [SerializeField]
    public CharacterController controller;

    public GoalScreen goalScreen;
    public bool reachedGoal;

    private float xTilt;
    private const int maxJump = 2;
    private int currJump;

    private bool touchCheck = false;
    private Vector3 moveDir;
    private Vector3 rotationDir;

    private float forwardSpd;

    private float jumpSpd;
    private float jumpOverSpd;

    //public HealthBar healthBar;
    [System.NonSerialized]
    public int maxHealth;
    [System.NonSerialized]
    public int health;
    [System.NonSerialized]
    public bool isInvincible;
    [System.NonSerialized]
    public int damageAmount;
    [System.NonSerialized]
    public float yDis;

    [System.NonSerialized]
    public float invincibilityDurationSeconds;
    [System.NonSerialized]
    public bool warping;
    [System.NonSerialized]
    public bool warpFlip;

    [SerializeField]
    private float invincibilityDeltaTime;

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private LandingParticle landingParticle;

    private AudioManager audioManager;
    private LevelData levelData;

    public void Start()
    {
        heightThreshold = -350f;
        currJump = 0;
        reachedGoal = false;
        isInvincible = false;
        damageAmount = 1;
        invincibilityDurationSeconds = 1.5f;
        maxHealth = 3;
        yDis = 26f;

        jumpSpd = gravityScale * 8 / 7;
        jumpOverSpd = gravityScale / 2 * 3;
        warping = false;
        warpFlip = false;

        audioManager = AudioManager.instance;
        levelData = LevelData.instance;
        if (levelData.up) { WarpDir(yDis); }
        forwardSpd = levelData.speed;
        health = levelData.health;
    }

    public void Update()
    {
        if (health < 0) { return; }
        ProcessInputs();
        // removes sections
        if (transform.position.x > sectionSpawner.endPoints.Peek())
        {
           sectionSpawner.RemoveFirst();
        }

    }

    public void FixedUpdate()
    {
        //check if out of bounds, instant game over
        if (controller.transform.position.y < heightThreshold)
        {
            health = -1;
        }
        if (health < 0) { return;  }
        rotationDir = new Vector3(xTilt, 0f, -20f);
        model.transform.Rotate(rotationDir, Space.World);
        controller.Move(Vector3.right * forwardSpd);

        //jump if touched (not ui) and has jump or is grounded
        if (touchCheck && (controller.isGrounded || maxJump > currJump) && !layoutTouch)
        {
            moveDir.y = jumpForce / (currJump + 1);
            ++currJump;
            touchCheck = false;
        }
        controller.Move(moveDir);

        // gravity-higher if jumped, lower rolling over an edge
        if (!controller.isGrounded)
        {
            if (currJump > 0) { moveDir.y += jumpSpd; }
            else { moveDir.y += jumpOverSpd; }
        }
        //resets jump and gravity
        else
        {
            if (currJump > 0) {
                Landing();
            }
            currJump = 0;
            moveDir.y = 0;
        }
    }
    private void Landing()
    {
        landingParticle.PlayLanding();
        audioManager.Play("Jump");
    }
    private void ProcessInputs()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            touchCheck = true;
            layoutTouch = EventSystem.current.currentSelectedGameObject;
        }

        //adjusts tilt sensitivity
        xTilt = Input.acceleration.x * -8f;
        if(xTilt > 5)
        {
            xTilt = 5f;
        }
        else if(xTilt < -5)
        {
            xTilt = -5f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            TakeDamage();
        }
        else if (other.CompareTag("GoUp") & warping == false)
        {
            StartCoroutine(Warp(yDis));
        }
        else if (other.CompareTag("GoDown") & warping == false & controller.transform.position.y > 14f)
        {
            StartCoroutine(Warp(-yDis));
        }
        else if (other.CompareTag("WarpFlip") & warpFlip == false)
        {
            warpFlip = true;
            float yDisWarp = transform.position.y > 14 ? -yDis : yDis;
            StartCoroutine(Warp(yDisWarp));
            StartCoroutine(WarpFlipWait());

        }
        else if (other.CompareTag("Goal") & reachedGoal == false)
        {
            GameObject.FindObjectOfType<PauseResume>().RemoveControlLayout();
            goalScreen.Setup();
            reachedGoal = true;
        }
    }

    //warp flip wait time to make sure the player doesn't warp in a row
    private IEnumerator WarpFlipWait()
    {
        yield return new WaitForSeconds(5f);
        warpFlip = false;
    }

    private IEnumerator Warp(float yD)
    {
        warping = true;
        StartCoroutine(BecomeTempInvinsible());
        yield return new WaitForSeconds(invincibilityDurationSeconds);
        WarpDir(yD);
        warping = false;
    }

    private void WarpDir(float yD)
    {
        controller.enabled = false;
        this.gameObject.transform.position = this.transform.position + new Vector3(0, yD, 0);
        controller.enabled = true;
    }
    public void TakeDamage()
    {
        if (isInvincible) return;
        health -= damageAmount;
        if (health < -1)
        {
            health = -1;
        }
        StartCoroutine(BecomeTempInvinsible());
    }

    private IEnumerator BecomeTempInvinsible()
    {
        Behaviour halo = (Behaviour) model.GetComponent("Halo");
        isInvincible = true;
        // scales model between 0 to 1 as invincibility animation
        for (float i =0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            if (model.transform.localScale == Vector3.one) {
                halo.enabled = false;
                ScaleModelTo(Vector3.zero); 
            }
            else {
                ScaleModelTo(Vector3.one);
                halo.enabled = true;
            }
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }
        ScaleModelTo(Vector3.one);
        isInvincible = false;
        halo.enabled = true;
    }

    private void ScaleModelTo(Vector3 scale)
    {
        model.transform.localScale = scale;
    }
}
