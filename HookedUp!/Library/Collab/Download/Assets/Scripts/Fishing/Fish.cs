using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    public FishFile.WhatKindOfFish whatFish;

    public enum FishState { neutral, seeBait, nibble, gotcha}
    public FishState fishState;
    public enum CatchState { youPull, fishPulls}
    //[HideInInspector]
    public CatchState catchState;
    public float fishStrenghtLevel;
    public float fishPullStrenght = 100;
    public float playerPullStrenght = 100;

    public Vector3 startPosition;
    public float moveSpeed;
    public float moveAreaRadius;
    public float newPositionRate;
    Vector3 randomPosition;
    public float noticeDistance;

    Vector3 mouseLastPos;
    bool mouseIsMoving;


	// Use this for initialization
	void Start () {

        startPosition = transform.position;
        StartCoroutine("WaitAndGetNewRandomPos");
	}
	
	// Update is called once per frame
	void Update () {

        if(Vector3.Distance(InputManager.instance.MouseLocation(), mouseLastPos) > 1f)
        {
            mouseIsMoving = true;
        }
        else
        {
            mouseIsMoving = false;
        }
        mouseLastPos = InputManager.instance.MouseLocation();

        switch (fishState)
        {

            case FishState.neutral:
                Neutral();
                break;

            case FishState.seeBait:
                SeeBait();
                break;

            case FishState.nibble:
                Nibble();
                break;

            case FishState.gotcha:
                Gotcha();
                break;
        }
	}

    void Neutral()
    {
        if (Vector3.Distance(transform.position, Rod.instance.bobber.transform.position) < noticeDistance)
        {
            if (InputManager.instance.clickState == InputManager.ClickStates.press)
            {
                if (Random.Range(0, 3) == 0)
                {
                    fishState = FishState.seeBait;
                }
            }
        }


        float step = Time.deltaTime * moveSpeed;

        transform.position = Vector3.MoveTowards(transform.position, randomPosition, step);
        Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.position - randomPosition, 0.02f, 0.0f);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    void SeeBait()
    {
        if (Vector3.Distance(transform.position, Rod.instance.bobber.transform.position) > noticeDistance)
        {
            fishState = FishState.neutral;
        }
        if (Vector3.Distance(transform.position, Rod.instance.bobber.transform.position) < 0.5f)
        {
            fishState = FishState.nibble;
        }


        float step = Time.deltaTime * moveSpeed * 3f;

        Vector3 newPos = Rod.instance.bobber.transform.position;
        newPos.y = 0;
        transform.position = Vector3.MoveTowards(transform.position, newPos, step);
        Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.position - newPos, 0.06f, 0.0f);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    void Nibble()
    {
        if (Vector3.Distance(transform.position, Rod.instance.bobber.transform.position) > 0.5f)
        {
            fishState = FishState.seeBait;
        }


        float step = Time.deltaTime * moveSpeed * 5f;

        Vector3 newPos = Rod.instance.bobber.transform.position;
        newPos.y = 0;
        transform.position = Vector3.MoveTowards(transform.position, newPos, step);
        Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.position - newPos, 0.06f, 0.0f);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);


        if(InputManager.instance.clickState == InputManager.ClickStates.press)
        {
            if(Random.Range(0,5) == 0)
            {
                //What happens when fish takes the bait
                fishState = FishState.gotcha;
                catchState = CatchState.fishPulls;
                transform.SetParent(Rod.instance.bobber.transform);
            }
            else
            {
                if (Random.Range(0, 6) == 0)
                {
                    fishState = FishState.neutral;

                }
            }
        }
    }

    void Gotcha()
    {
        transform.localPosition = Vector3.zero;

        switch (catchState)
        {

            case CatchState.youPull:
                Rod.instance.fishPullsBack = false;

                if (!mouseIsMoving)
                {
                    if (Random.Range(0, 6) == 0)
                    {
                        catchState = CatchState.fishPulls;
                        fishPullStrenght = 40;
                    }
                }

                break;


            case CatchState.fishPulls:

                Rod.instance.fishPullsBack = true;

                //Fish Reels freely
                if (InputManager.instance.clickState != InputManager.ClickStates.hold)
                {
                    float step = Time.deltaTime * moveSpeed * 4;

                    Vector3 newPos = Rod.instance.transform.position;
                    newPos.y = 0;
                    Transform bobberTr = Rod.instance.bobber.transform;
                    bobberTr.position = Vector3.MoveTowards(bobberTr.position, bobberTr.position - newPos, step);
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.position - newPos, 0.06f, 0.0f);
                    newDir.y = 0;
                    transform.rotation = Quaternion.LookRotation(newDir);
                }
                //Player Fights the fish
                else if(playerPullStrenght > 0)
                {

                    if(mouseIsMoving)
                    {
                        fishPullStrenght -= 5 - fishStrenghtLevel * Time.deltaTime;
                    }
                    else
                    {
                        float step = Time.deltaTime * moveSpeed * 2;

                        Vector3 newPos = Rod.instance.transform.position;
                        newPos.y = 0;
                        Transform bobberTr = Rod.instance.bobber.transform;
                        bobberTr.position = Vector3.MoveTowards(bobberTr.position, bobberTr.position - newPos, step);
                        Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.position - newPos, 0.06f, 0.0f);
                        newDir.y = 0;
                        transform.rotation = Quaternion.LookRotation(newDir);

                        //lisää että pelaajan ongen heiluttelu vähentäisi kalan strenght

                        playerPullStrenght -= 1 * Time.deltaTime;
                    }

                }

                if(fishPullStrenght < 0.01f)
                {
                    catchState = CatchState.youPull;
                }

                break;

        }
    }


    IEnumerator WaitAndGetNewRandomPos()
    {
        float randomX = Random.Range(startPosition.x - moveAreaRadius, startPosition.x + moveAreaRadius);
        float randomZ = Random.Range(startPosition.z - moveAreaRadius, startPosition.z + moveAreaRadius);
        randomPosition = new Vector3(startPosition.x + randomX, startPosition.y, startPosition.z + randomZ);

        yield return new WaitForSeconds(newPositionRate);

        StartCoroutine("WaitAndGetNewRandomPos");

    }


}
