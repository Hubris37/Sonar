using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChefAI : EnemyAI {

    [Header("Jumping modifiers")]
    public float jumpDistance = 0.5f;
    public float jumpChaseMultiplier = 1.5f;
    public float jumpHeight = 0.4f;
    private float jumpOffsetCounter = 0;
    //private Vector3 jumpStartPos;
    private bool makeSound = false;

    public AudioSource audioThump;
    public AudioSource audioSniff;


    // Use this for initialization
    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();
        rigid = GetComponent<Rigidbody>();

        audioThump = sources[0];
        audioSniff = sources[1];
        audioStartle = sources[2];
    }

    // Update is called once per frame
    void Update() {
        findPath();
        move();
        checkAggro();
    }

    protected override void onAggro() {
        anim.speed = jumpChaseMultiplier;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Seek")) {
            anim.SetTrigger("chasing");
        }
        makeDetectionSound();
    }

    protected override void onLoseAggro() {
        anim.speed = 1.0f;
    }

    public override void move() {
   //     chasingAround = isAggroed && !straightLineToPlayer();
        Vector3 dif, movePoint;
        Vector3 curPos = transform.position;
        float jumpDist = jumpDistance;
        // If chasing player, move towards him/her
        if (isAggroed) {
            dif = playerPos - curPos;
            dif.y = 0;
            movePoint = playerPos;
            //movementMultiplier = chasingSpeedMultiplier;
            jumpDist *= jumpChaseMultiplier;
        }
        else {
         /*   if (chasingAround) {
                movementPath = pathToPlayer();
            }*/
            // Else, move on calculated path
            int tilesLeft = movementPath.Count;
            if (tilesLeft == 0) return;
            float thresh = 0.5f;
            movePoint = movementPath[0].transform.position;
            dif = movePoint - curPos;
            dif.y = 0;
            if (dif.magnitude < thresh) {
                currentPositionCell = movementPath[0];
                movementPath.RemoveAt(0);
                if (movementPath.Count == 0) return;
                // movementPath = tryDiagonal(movementPath);
            }
        }
        tryGrabPlayer();
        movePoint.y = curPos.y;
        if (dif.normalized != transform.forward) {
            transform.LookAt(movePoint);
        }
        //transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime, Space.World);
        jumpHandler(movePoint, jumpDist);
    }

    private void jumpHandler(Vector3 movePoint, float dist) {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            rigid.velocity = Vector3.zero;
      //      jumpStartPos = transform.position;
            anim.SetTrigger("jump");
            jumpOffsetCounter = getAnimationLength("Chesschef Jump") / (3 * anim.speed);
            if (makeSound) {
                makeLandingSound();
                makeSound = false;
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
            if (jumpOffsetCounter <= 0) {
            //    Vector3 dir = movePoint - jumpStartPos;
        //        float distance = Mathf.Min(Math.Abs(dir.magnitude), dist);
               // jumpOffsetCounter = 1000000;

                rigid.velocity = transform.forward * dist;
                //rigid.velocity = transform.TransformDirection(dir.normalized * 1.0f);
                // transform.Translate(dir.normalized * distance * Time.deltaTime, Space.World);
                //rigid.MovePosition(transform.TransformDirection(dir.normalized * distance * Time.deltaTime));
                // transform.Move(dir.normalized * distance * Time.deltaTime, Space.World);
                
                makeSound = true;
            }
            else {
                jumpOffsetCounter -= Time.deltaTime;
            }
        }
    }

    private void makeLandingSound() {
        audioThump.Play();
        blastHit(transform.position, 750, .2f);
    }

    private void makeSniffingSound() {
        audioSniff.Play();
        blastHit(transform.position, 450, .18f);
    }

}
