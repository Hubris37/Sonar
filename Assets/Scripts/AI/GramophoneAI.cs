using UnityEngine;
using System.Collections.Generic;
using System;

public class GramophoneAI : EnemyAI {

    [Header("Walking modifiers")]
    public float movementSpeed = 1.4f;
    public float chasingSpeedMultiplier = 1.2f;
    public float runAnimSpeed = 2.0f;

    private bool hasStartled = false;
    private AudioSource music;

    private float musicLoudness;
    private float[] musicSampleData;
    private int musicSampleAmount = 1024;

    private StaticSoundSource soundEmitter;

    public Transform head;

    // Fixa inte opera i alla rum

    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        //AudioSource[] sources = GetComponents<AudioSource>(); never used
        musicSampleData = new float[musicSampleAmount];
        head = searchForBone(transform, "Bone.002");
        rigid = GetComponent<Rigidbody>();

        soundEmitter = GetComponent<StaticSoundSource>();
    }

    void FixedUpdate() {
        findPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking")) {
            move();
        } else {
            rigid.velocity = Vector3.zero;
        }
        checkAggro();
    }

    void LateUpdate() {
        volumeScale();
        soundEmitter.multAudioParameters(musicLoudness, musicLoudness);
    }

    private void volumeScale() {
        if (music == null) {
            music = GetComponent<StaticSoundSource>().getAudio();
        }
        else {
            music.clip.GetData(musicSampleData, music.timeSamples);
            musicLoudness = 0f;
            foreach (var s in musicSampleData) {
                musicLoudness += Mathf.Abs(s);
            }
            musicLoudness /= musicSampleAmount;
            musicLoudness = Mathf.Clamp(musicLoudness * 100, 1, 1.9f);
            //    float scaleLimit = 0.05f;
            Vector3 newScale = Vector3.one * musicLoudness;
            //  if (Mathf.Abs(newScale.magnitude - head.localScale.magnitude) > scaleLimit) {
            //    newScale = (newScale.magnitude > head.localScale.magnitude) ? head.localScale + (Vector3.one * scaleLimit) : head.localScale - (Vector3.one * scaleLimit);
            // }
            newScale.y = 1;
            head.localScale = newScale;
        }
    }
    

    protected override void onAggro() {
        List<MazeCell> map = currentPositionCell.room.getCells();
        movementPath = aStar(findMaxCostCell(currentPositionCell, map),map);
        anim.SetTrigger("startled");
        anim.speed = runAnimSpeed;
        rigid.drag = 2;
    }

    protected override void onLoseAggro() {
        anim.speed = 1.0f;
        rigid.drag = 10;
    }

    public override void move() {
        Vector3 dif, movePoint;
        Vector3 curPos = transform.position;
        float movementMultiplier = (isAggroed) ? chasingSpeedMultiplier : 1.0f;
      
        // Move on calculated path
        movePoint = getPathTargetPoint(curPos);
        dif = movePoint - curPos;

        curPos.y = 0;
       // transform.Translate(curPos);
        movePoint.y = curPos.y;
        if (dif.normalized != transform.forward) {
            transform.LookAt(movePoint);
        }
        rigid.AddForce( dif.normalized * movementSpeed * movementMultiplier);
       // transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime, Space.World);
    }

    private Transform searchForBone(Transform current, string name) {
        if (current.name == name) {
            return current;
        }
        foreach (Transform c in current) {
            Transform found = searchForBone(c, name);
            if (found != null) {
                return found;
            }
        }
        return null;
    }
}
