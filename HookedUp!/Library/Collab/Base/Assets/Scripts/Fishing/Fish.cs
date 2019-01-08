using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    public FishFile.WhatKindOfFish whatFish;

    public float moveSpeed;
    public float newPositionRate;
    Vector3 randomPosition;

	// Use this for initialization
	void Start () {
        StartCoroutine("WaitAndGetNewRandomPos");
	}
	
	// Update is called once per frame
	void Update () {

        float step = Time.deltaTime * moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, randomPosition, step);

	}

    IEnumerator WaitAndGetNewRandomPos()
    {
        float randomX = Random.Range(randomPosition.x - 3, randomPosition.x + 3);
        float randomZ = Random.Range(randomPosition.z - 3, randomPosition.z + 3);
        randomPosition = new Vector3(randomX, 0, randomZ);

        yield return new WaitForSeconds(newPositionRate);

        StartCoroutine("WaitAndGetNewRandomPos");

    }


}
