using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public enum State{ idle, fishing, date}
    public State state;

    State lastState;
    bool once;

    public GameObject rodFolder;
    public GameObject dialogieFolder;

	// Use this for initialization
	void Awake () {
        
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
        if(lastState != state)
        {
            once = false;
        }

        switch(state)
        {
            case State.idle:
                Idle();
                break;

            case State.fishing:
                Fishing();
                break;


            case State.date:
                Dialogue();
                break;
        }

        lastState = state;
	}

    void Idle()
    {
        
    }

    void Fishing()
    {
        if (!once)
        {
            EnableFishingDisableDialogue();
            once = true;
        }
    }

    void Dialogue()
    {
        if(!once)
        {
            EnableDialogueDisableFishing();
            DialogueManager.instance.state = DialogueManager.DialogueState.dialogueStart;
            once = true;
        }
    }

    void EnableFishingDisableDialogue()
    {
        dialogieFolder.SetActive(false);

        rodFolder.SetActive(true);
        Rod.instance.bobber.SetActive(true);
    }

    void EnableDialogueDisableFishing()
    {
        rodFolder.SetActive(false);
        Rod.instance.bobber.SetActive(false);

        dialogieFolder.SetActive(true);

    }
}
