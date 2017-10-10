using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    TotallyUsefullAiCompnent ai;
    WorldManager worldManager;
    AudioSource coinPing;

    [Header("UI Components")]
    public Text scoreText;
    public Transform welcomeScreen;
    public Transform deathScreen;
    public Transform hud;
    public Text deathCoins;
    public Text deathMeters;
    public Text deathSpeed;
    public Toggle deathAIToggle;

    int currentPos = 0; // -1 = left, 1 = right
    int score;
    bool alive;

    //jump animation var
    bool jumping;
    [Header("Jump animation curve")]
    public AnimationCurve jumpCurve;
    float jumpTimer;

    //move animation var
    bool moving;
    float moveTimer;
    int workingpos;

    [Header("Sounds")]
    public AudioClip moveClip;
    public AudioClip jumpClip;
    public AudioSource movejumpSource;

    //mobile swiping
    Vector2 firstPressPos;

    //input
    [Header("Player/AI input")]
    public bool left;
    public bool right;
    public bool jump;

    //ai on = true
    bool playWithAI;

    void Start () {
        worldManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<WorldManager>();
        ai = GameObject.FindGameObjectWithTag("Manager").GetComponent<TotallyUsefullAiCompnent>();
        coinPing = GetComponent<AudioSource>();

    }
	
	void Update () {
        if (alive) {
            //moving and animation
            if (moving) {
                moveTimer += (1.0f / 6.0f) * worldManager.worldSpeedMultiplier;

                transform.position = Vector3.Lerp(new Vector3(currentPos, 0, 0), new Vector3(workingpos, 0, 0), moveTimer);
                transform.GetChild(0).localEulerAngles = new Vector3(0, Mathf.Clamp((moveTimer * (workingpos - currentPos)) * 90.0f, -90, 90), 0);

                if (moveTimer >= 1) {
                    currentPos = workingpos;
                    moveTimer = 0;
                    moving = false;
                }
            } //jumping and animation
            else if (jumping) {
                jumpTimer += Time.deltaTime * 1.5f * worldManager.worldSpeedMultiplier;

                transform.position = new Vector3(currentPos, jumpCurve.Evaluate(jumpTimer) * 1.5f, 0);

                transform.GetChild(0).localEulerAngles = new Vector3(Mathf.Clamp(jumpTimer * 90.0f, 0, 90), 0, 0);

                if (jumpTimer >= 1) {
                    jumpTimer = 0;
                    jumping = false;
                }
            } 
            else {
                if (!ai.enabled) {
                    //keyboar input
                    left = (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
                    right = (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));
                    jump = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);

                    //simple touch input
                    if (Input.touches.Length > 0) {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began) {
                            firstPressPos = touch.position;
                        }
                        if (touch.phase == TouchPhase.Ended) {
                            Vector3 currentSwipe = new Vector3(touch.position.x - firstPressPos.x, touch.position.y - firstPressPos.y);

                            currentSwipe.Normalize();

                            if (currentSwipe.y > 0.2f && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) { // up
                                jump = true;
                            }
                            if (currentSwipe.x < 0.2f && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) { // left
                                left = true;
                            }
                            if (currentSwipe.x > 0.2f && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) { //right
                                right = true;
                            }
                        }
                    }
                }

                if (currentPos >= 0 && left) { //when pressed left
                    workingpos--;
                    moving = true;
                    PlaySound(moveClip);
                }

                if (currentPos <= 0 && right) { //when pressed right
                    workingpos++;
                    moving = true;
                    PlaySound(moveClip);
                }

                if (jump) { // when pressed jump
                    jumping = true;
                    PlaySound(jumpClip);
                }
            }

            scoreText.text = score.ToString();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Dead();
            }
        } 
    }

    /// <summary>
    /// When click Play on worldScreen
    /// </summary>
    public void Play() {
        welcomeScreen.gameObject.SetActive(false);
        Reset();
    }

    /// <summary>
    /// When click Restart on deathScreen
    /// </summary>
    public void Reset() {
        deathScreen.gameObject.SetActive(false);
        hud.gameObject.SetActive(true);
        alive = true;

        transform.position = new Vector3(0, 0, 0);
        transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
        moving = false;
        moveTimer = 0;
        jumping = false;
        jumpTimer = 0;
        currentPos = 0;
        workingpos = 0;
        score = 0;

        if(playWithAI)
            ai.enabled = true;

        worldManager.Reset();
    }

    /// <summary>
    /// On Dead
    /// </summary>
    void Dead() {
        ai.enabled = false;

        alive = false;
        worldManager.StopWorld();
        deathScreen.gameObject.SetActive(true);
        hud.gameObject.SetActive(false);

        deathAIToggle.isOn = playWithAI;

        deathCoins.text = string.Format("You collected {0} coins!", score);

        deathMeters.text = string.Format("you moved {0} meters", worldManager.TravelledMeters.ToString("F1"));

        deathSpeed.text = string.Format("highest speed recorded: {0}", worldManager.HighSpeed.ToString("F1"));
    }

    /// <summary>
    /// Toggles AI on and off
    /// </summary>
    public void ToggleAI() {
        playWithAI = !playWithAI;
    }

    /// <summary>
    /// Play a soundclip
    /// </summary>
    void PlaySound(AudioClip clip) {
        movejumpSource.clip = clip;
        movejumpSource.Play();
    }

    void OnTriggerEnter(Collider col) {
        if (col != null) {
            switch (col.tag) {
                case "Coin": {
                        score += 1;

                        coinPing.Play();

                        Destroy(col.gameObject);
                        return;
                    };
                case "Wall": {
                        Dead();
                        return;
                    };
            }
        }
    }

    /// <summary>
    /// Get current position of player
    /// </summary>
    public int CurrentPosition {
        get {
            return currentPos;
        }
    }

    /// <summary>
    /// Check if player is alive
    /// </summary>
    public bool Alive {
        get {
            return alive;
        }
    }
}
