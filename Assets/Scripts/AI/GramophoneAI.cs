﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class GramophoneAI : EnemyAI {

    [Header("Walking modifiers")]
    public float movementSpeed = 1.4f;
    public float chasingSpeedMultiplier = 1.2f;

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
        soundEmitter = GetComponent<StaticSoundSource>();
    }

    void Update() {
   //     handleStartle();
        findPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking")) {
            move();
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

    private void handleStartle() {
        if (isAggroed && !hasStartled) {
            anim.SetBool("startled", true);
            hasStartled = true;
        } else if (!isAggroed) {
            hasStartled = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Startled")) {
            anim.SetBool("startled", false);
        }
    }

    protected override void onAggro() {
        List<MazeCell> map = currentPositionCell.room.getCells();
        movementPath = aStar(findMaxCostCell(currentPositionCell, map),map);
        anim.SetTrigger("startled");
        anim.speed = chasingSpeedMultiplier;
    }

    protected override void onLoseAggro() {
        anim.speed = 1.0f;
    }

    public override void move() {
        Vector3 dif, movePoint;
        float movementMultiplier = (isAggroed) ? chasingSpeedMultiplier : 1.0f;
        
        // Move on calculated path
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
        
        movePoint.y = transform.position.y;
        transform.LookAt(movePoint);
        transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime, Space.World);
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
