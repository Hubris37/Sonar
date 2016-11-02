using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ChefAI : EnemyAI {

    [Header("Jumping modifiers")]
    public float jumpDistance = 0.5f;
    public float jumpChaseMultiplier = 1.5f;
    public float jumpHeight = 0.4f;
    private float jumpOffsetCounter = 0;
    //private Vector3 jumpStartPos;
    private bool makeSound = false;

    public AudioClip audioThump;
    public AudioClip audioSniff;

    [Header("Visuals")]
    public Sprite happy;
    public Sprite angry;
    public Image statusImage;

    // Use this for initialization
    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();
        rigid = GetComponent<Rigidbody>();

        //audioThump = sources[0];
        //audioSniff = sources[1];
        //audioStartle = sources[2];
    }

    void OnEnable() {
        WaiterAI.shout += investigatePoint;
        statusImage.color = new Color(1,1,1,1);
    }


    void OnDisable() {
        WaiterAI.shout -= investigatePoint;
        statusImage.color = new Color(1,1,1,0);
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
        statusImage.sprite = angry;
    }

    protected override void onLoseAggro() {
        anim.speed = 1.0f;
        statusImage.sprite = happy;
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
            movePoint = getPathTargetPoint(curPos);
            dif = movePoint - curPos;
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
            makeSound = true;
            anim.SetTrigger("jump");
            jumpOffsetCounter = getAnimationLength("Chesschef Jump") / (3 * anim.speed);            
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
            if (jumpOffsetCounter <= 0) {
            //    Vector3 dir = movePoint - jumpStartPos;
        //        float distance = Mathf.Min(Math.Abs(dir.magnitude), dist);
               // jumpOffsetCounter = 1000000;

                rigid.velocity = transform.forward * dist;
                // Offset for landing sound
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8) {
                    if (makeSound) {
                        makeLandingSound();
                        makeSound = false;
                    }
                }
                //rigid.velocity = transform.TransformDirection(dir.normalized * 1.0f);
                // transform.Translate(dir.normalized * distance * Time.deltaTime, Space.World);
                //rigid.MovePosition(transform.TransformDirection(dir.normalized * distance * Time.deltaTime));
                // transform.Move(dir.normalized * distance * Time.deltaTime, Space.World);

            }
            else {
                jumpOffsetCounter -= Time.deltaTime;
            }
        }
    }

    private void makeLandingSound() {
        AudioManager.instance.PlaySound(audioThump, transform.position);
        blastHit(transform.position, 750, .8f);
    }

    private void makeSniffingSound() {
        AudioManager.instance.PlaySound(audioSniff, transform.position);
        blastHit(transform.position, 450, .18f);
    }

}
