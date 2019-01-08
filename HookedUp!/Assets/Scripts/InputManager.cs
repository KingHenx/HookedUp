using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager instance;

    public enum ClickStates { idle, press, hold, release, released, slider}
    public ClickStates clickState;
    public float sliderHeight = 100;

    [HideInInspector]
    public Camera mainCamera;

    public DialogueBlock block;
    

	private void Awake()
	{

        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;   
        }

        mainCamera = Camera.main;
	}

	void Update () {

        if (clickState == ClickStates.release)
        {
            clickState = ClickStates.released;
        }

        if (!Application.isEditor)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.position.y > sliderHeight)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            clickState = ClickStates.press;
                            break;

                        case TouchPhase.Ended:
                            clickState = ClickStates.release;
                            break;

                        default:
                            clickState = ClickStates.hold;
                            break;
                    }
                }
                else
                {
                    clickState = ClickStates.slider;
                }

            }
        }
        else
        {

            if (Input.mousePosition.y > sliderHeight)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clickState = ClickStates.press;


                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        clickState = ClickStates.hold;
                    }
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            clickState = ClickStates.release;

                        }
                    }
                }
            }
            else
            {
                clickState = ClickStates.slider;
            }

        }

        switch (clickState)
        {

            //case ClickStates.idle:
            //    Rod.instance.states = Rod.States.idle;
            //    break;

            //case ClickStates.press:
            //    Rod.instance.states = Rod.States.press;
            //    break;

            //case ClickStates.hold:
            //    Rod.instance.states = Rod.States.hold;
            //    break;

            //case ClickStates.release:
                //Rod.instance.states = Rod.States.release;
                //break;

        }
    }

    public Vector3 MouseLocation()
    {
        if (Application.isEditor)
        {
            return Input.mousePosition;
        }
        else
        {
            return Input.GetTouch(0).position;
        }
    }

    }
