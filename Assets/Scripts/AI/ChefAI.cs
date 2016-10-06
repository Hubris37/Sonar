using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChefAI : EnemyAI {

    [Header("Jumping modifiers")]
    public float jumpDistance = 0.5f;
    public float jumpChaseMultiplier = 1.5f;
    public float jumpHeight = 0.4f;
    private float jumpOffsetCounter = 0;
    private Vector3 jumpStartPos;

    public AudioSource audioThump;
    public AudioSource audioSniff;


    // Use this for initialization
    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();

        audioThump = sources[0];
        audioSniff = sources[1];
        audioGrunt = sources[2];
    }

    // Update is called once per frame
    void Update() {
        findPath();
        move();
        checkAggro();
    }

    public override void move() {
        Vector3 dif, movePoint;
        float jumpDist = jumpDistance;
        // If chasing player, move towards him/her
        if (isChasing) {
            dif = playerPos - transform.position;
            dif.y = 0;
            movePoint = playerPos;
            //movementMultiplier = chasingSpeedMultiplier;
            jumpDist *= jumpChaseMultiplier;
            tryGrabPlayer();
        }
        else {
            // Else, move on calculated path
            int tilesLeft = movementPath.Count;            
            if (tilesLeft == 0) return;
            float thresh = 0.5f;
            dif = movementPath[0].transform.position - transform.position;
            dif.y = 0;
            movePoint = movementPath[0].transform.position;
            if (dif.magnitude < thresh) {
                currentPositionCell = movementPath[0];
                movementPath.RemoveAt(0);
                if (movementPath.Count == 0) return;
                // movementPath = tryDiagonal(movementPath);
            }
        }
        movePoint.y = transform.position.y;
        transform.LookAt(movePoint);
        //transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime, Space.World);
        jumpHandler(movePoint, jumpDist);

    }

    private void jumpHandler(Vector3 movePoint, float dist) {
        if (isChasing) {
            anim.speed = jumpChaseMultiplier;
        } else {
            anim.speed = 1.0f;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            jumpStartPos = transform.position;
            anim.SetBool("jump", true);
            jumpOffsetCounter = getAnimationLength("Chesschef Jump") / (3 * anim.speed);
            if (makeSound) {
                makeLandingSound();
                makeSound = false;
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
            if (jumpOffsetCounter <= 0) {
                anim.SetBool("jump", false);
                Vector3 dir = movePoint - jumpStartPos;
                float distance = Mathf.Min(dir.magnitude, dist);
                transform.Translate(dir.normalized * distance * Time.deltaTime, Space.World);
                makeSound = true;
            }
            else {
                jumpOffsetCounter -= Time.deltaTime;
            }
        }
    }

    private void makeLandingSound() {
        audioThump.Play();
        blasHit(transform.position, 750, .2f);
    }

    private void makeSniffingSound() {
        audioSniff.Play();
        blasHit(transform.position, 450, .18f);
    }

}
