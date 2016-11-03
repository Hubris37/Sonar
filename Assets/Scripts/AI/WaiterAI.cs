using UnityEngine;
using System.Collections.Generic;
using System;

public class WaiterAI : EnemyAI {

    [Header("Shout modifiers")]
    public float shoutInterval;
    private float shoutCounter;
    public float shoutVolume = 0.3f;

    [Header("Walking modifiers")]
    public float movementSpeed = 1.4f;
    public float chasingSpeedMultiplier = 1.2f;
    public float runAnimSpeed = 2.0f;
    public float chaseStopDistance = 0.5f;

    public delegate void Alert(Vector3 position, float noiseLevel);
    public static event Alert shout;

    public AudioClip audioScream;

    // Fixa inte opera i alla rum

    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();
        rigid = GetComponent<Rigidbody>();

   //     audioStartle = sources[0];
     //   audioScream = sources[1];
    }

    void FixedUpdate() {
        findPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) {
            move();
        }
        else {
          //  rigid.velocity = Vector3.zero;
        }
        checkAggro();
        if (isAggroed) {
            checkShout();
        }
    }
    
    private void checkShout() {
        shoutCounter -= Time.deltaTime;
        if (shoutCounter <= 0) {
            shoutCounter = shoutInterval;
            Vector3 curPos = transform.position;
            anim.SetTrigger("shout");
            blastHit(curPos, 600, shoutVolume);
            AudioManager.instance.PlaySound(audioScream, transform.position);
            if (shout != null)
                shout(curPos, 8);
        }
    }
    
    public bool tooCloseToPlayer() {
        if ((transform.position-playerPos).magnitude < chaseStopDistance) {
          //  rigid.velocity = Vector3.zero;
            return true;
        }
        return false;
    }

    protected override void onAggro() {
        anim.speed = runAnimSpeed;
        anim.SetTrigger("alert");
        makeDetectionSound();
        shoutCounter = shoutInterval;
        rigid.drag = 4;
    }

    protected override void onLoseAggro() {
        anim.speed = 1.0f;
        rigid.drag = 10;
    }

    public override void move() {
        Vector3 dif, movePoint;
        Vector3 curPos = transform.position;
        float totalMS = movementSpeed;
        // If chasing player, move towards him/her
        if (isAggroed) {
            dif = playerPos - curPos;
            dif.y = 0;
            movePoint = playerPos;
            totalMS *= chasingSpeedMultiplier;
        }
        else {
            movePoint = getPathTargetPoint(curPos);
            dif = movePoint - curPos;
        }
        movePoint.y = curPos.y;
        if (dif.normalized != transform.forward) {
            transform.LookAt(movePoint);
        }
        if (tooCloseToPlayer()) {
            dif = -dif;
            totalMS /= 2;
        }
        rigid.AddForce(dif.normalized * totalMS);
    }

}
