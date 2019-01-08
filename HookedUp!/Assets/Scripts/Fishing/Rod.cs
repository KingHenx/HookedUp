using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rod : MonoBehaviour {

    public static Rod instance;

    private LineRenderer rodLine;
    private LineRenderer line;
    private GameObject rodEnd;
    public GameObject bobber;
    private float rodDepth;

    Rigidbody bobberRb;

    BezierPoint rodEndPoint;
    BezierPoint bobberEndPoint;

    Vector3 bobberStartPos, rodEndStartPos;

    Vector3 rodLastPosition;
    Vector3 bobberLastPosition;

    float rodEndTempY;

    [HideInInspector]
    public float speed;
    public float throwForceModifier = 1;

    float lastX;
    float lastY;

    bool towardsRight;
    bool towardsDown;

    bool clockwiseOrCounter;

    SpringJoint lineJoint;

    public enum States { idle, press, hold, release, reelBack}
    public States states;
    public float reelBackSpeed;
    public float reelInDistance;
    [HideInInspector]
    public bool fishPullsBack;

    public GameObject waterEffect;
    bool isThereWaterEffect;

    BezierCurve bezier;

    InputManager inputManager;

    int turnDirection;

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

        rodEnd = GameObject.Find("RodEnd");
        rodLine = GetComponent<LineRenderer>();
        line = rodEnd.GetComponent<LineRenderer>();
        rodLine.positionCount = 2;
        line.positionCount = 2;

        bobberRb = bobber.GetComponent<Rigidbody>();

        bobberStartPos = bobber.transform.position;
        rodEndStartPos = rodEnd.transform.position;

        rodEndPoint = GameObject.Find("Point 0").GetComponent<BezierPoint>();
        bobberEndPoint = GameObject.Find("Point 1").GetComponent<BezierPoint>();

        lineJoint = bobber.GetComponent<SpringJoint>();

        bezier = bobber.GetComponent<BezierCurve>();

        inputManager = Camera.main.GetComponent<InputManager>();
	}
	
	// Update is called once per frame
	void Update () {

        Camera.main.transform.Rotate(Vector3.up * turnDirection * Time.deltaTime); 

        speed = Mathf.Clamp(bobber.GetComponent<Rigidbody>().velocity.magnitude, 0, 100) * 0.4f;
       
        if (bobber.transform.position.z > rodEnd.transform.position.z)
        {
            line.sortingOrder = 0;
        }
        else
        {
            line.sortingOrder = 2;
        }

        switch (states)
        {

            case States.idle:

                //reset locations
                if(!bobber.GetComponent<SpringJoint>())
                {
                    Quaternion cameraRotationBefore = Camera.main.transform.rotation;
                    Camera.main.transform.rotation = Quaternion.identity;

                    rodEnd.transform.position = rodEndStartPos;
                    bobber.transform.position = rodEnd.transform.position;
                    bobber.transform.SetParent(Camera.main.transform);

                    Camera.main.transform.rotation = cameraRotationBefore;

                    bobber.transform.SetParent(null);

                    lineJoint = bobber.AddComponent<SpringJoint>();
                    lineJoint.spring = 1;
                    lineJoint.connectedBody = rodEnd.GetComponent<Rigidbody>();

                    isThereWaterEffect = false;
                }
                LineVisuals();
                if(InputManager.instance.clickState == InputManager.ClickStates.press)
                    states = States.press;
                break;

            case States.press:
                PressPhase();
                states = States.hold;
                break;

            case States.hold:
                HoldPhase();
                if (InputManager.instance.clickState == InputManager.ClickStates.release)
                    states = States.release;
                break;

            case States.release:
                ReleasePhase();
                states = States.reelBack;
                break;

            case States.reelBack:
                ReelBackPhase();
                break;



        }
        bezier.DrawBezierLine();


    }

    void IdlePhase()
    {
        if(lineJoint != null)
            lineJoint.spring = Mathf.Clamp(0.5f * speed, 0.5f, 1);

       
    }

    void PressPhase()
    {
        rodEndTempY = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                                         Input.mousePosition.y, 1 - rodDepth)).y;
    }

    void HoldPhase()
    {
        LineVisuals();
        RodVisual();

        lineJoint.spring = Mathf.Clamp(0.5f * speed, 0.5f, 1);

        lineJoint.spring = Mathf.Clamp(0.5f * speed, 0.5f, 1);

    }


    void ReleasePhase()
    {
        bobberRb.AddForce((rodEnd.transform.position - transform.position) 
                          * (rodEnd.transform.position - bobber.transform.position).magnitude * 0.05f 
                          * (speed * 0.6f) * throwForceModifier, ForceMode.Impulse);
        lineJoint.spring = 0;

        Destroy(lineJoint);
    }


    void ReelBackPhase()
    {
        if (Vector3.Distance(rodEnd.transform.position, bobber.transform.position) < reelInDistance)
        {
            if (isThereWaterEffect)
            {
                states = States.idle;
                InputManager.instance.clickState = InputManager.ClickStates.idle;
            }
        }
        else
        {
            if (InputManager.instance.clickState == InputManager.ClickStates.hold && !fishPullsBack)
            {
                bobber.transform.position = Vector3.MoveTowards(bobber.transform.position, rodEnd.transform.position, reelBackSpeed * Time.deltaTime);
            }
            else
            {
                LookAtBobber();
            }
        }
        
        RodVisual();

        // straightens the line
        rodEndPoint.handle1 = Vector3.Lerp(rodEndPoint.handle1, Vector3.zero, 0.01f);
        bobberEndPoint.handle1 = Vector3.Lerp(bobberEndPoint.handle1, Vector3.zero, 0.01f);

        //rodEnd.transform.position = Vector3.Lerp(rodEnd.transform.position, new Vector3(rodEnd.transform.position.x, 1.5f, rodEnd.transform.position.z), 0.05f);

        rodLine.SetPosition(0, transform.position);
        rodLine.SetPosition(1, rodEnd.transform.position);


        line.SetPosition(0, rodEnd.transform.position);



        if (bobber.transform.position.y > -0.1f && bobber.transform.position.y < 0.001f)
        {


            if (!isThereWaterEffect)
            {
                bobberRb.velocity = Vector3.zero;
                GameObject newSplash = Instantiate(waterEffect);
                newSplash.transform.position = bobber.transform.position;
                isThereWaterEffect = true;
            }
        }


        if (bobber.transform.position.y < 0)
        {
            // In progress


            //bobberRb.useGravity = false;

            bobber.transform.position = new Vector3(bobber.transform.position.x, -0.01f, bobber.transform.position.z);

            //bobber.transform.position = Vector3.Lerp(bobber.transform.position, new Vector3(bobber.transform.position.x, 0, bobber.transform.position.z), 0.2f);
            //bobber.transform.position = new Vector3(bobber.transform.position.x, bobber.transform.position.y + 2f * Time.deltaTime, bobber.transform.position.z);
            //bobberRb.AddForce(new Vector3(0, -bobber.transform.position.y * Time.deltaTime * 5, 0));
        }
        else if (bobber.transform.position.y > 0.1f)
        {
            //bobberRb.useGravity = true;
        }
    }


    void RodVisual()
    {
        Vector3 rodEndPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                                            Input.mousePosition.y, 1 - rodDepth));
        if (Input.touchCount > 0)
        {
            rodEndPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x,
                                                                        Input.GetTouch(0).position.y, 1 - rodDepth));
        }

        rodEnd.transform.position = rodEndPosition;

        rodDepth = (rodEndTempY - rodEndPosition.y) * 2;

        rodDepth = Mathf.Clamp(rodDepth, -1.5f, 1.5f);
    }

    void LineVisuals()
    {
        rodLine.SetPosition(0, transform.position);
        rodLine.SetPosition(1, rodEnd.transform.position);

        line.SetPosition(0, rodEnd.transform.position);

        if (inputManager.clickState == InputManager.ClickStates.hold)
        {
            MouseMovementCircleCheck();
        }

        // makes line resemble actual fishing line
        if (clockwiseOrCounter)
        {
            //clockwise
            rodEndPoint.handle2 = Vector3.Lerp(rodEndPoint.handle2, (rodEnd.transform.position - bobber.transform.position) * speed, 0.25f);
            rodEndPoint.handle2 = new Vector3(-rodEndPoint.handle2.z, 0, rodEndPoint.handle2.x);

            bobberEndPoint.handle1 = Vector3.Lerp(bobberEndPoint.handle1, (bobber.transform.position - bobberLastPosition) * 10 * speed, 0.25f);

            bobberLastPosition = bobber.transform.position;
        }
        else
        {
            //counterclockwise
            rodEndPoint.handle2 = Vector3.Lerp(rodEndPoint.handle2, (rodEnd.transform.position - bobber.transform.position) * speed, 0.25f);
            rodEndPoint.handle2 = new Vector3(rodEndPoint.handle2.z, 0, -rodEndPoint.handle2.x);

            bobberEndPoint.handle1 = Vector3.Lerp(bobberEndPoint.handle1, (bobber.transform.position - bobberLastPosition) * 10 * speed, 0.25f);

            bobberLastPosition = bobber.transform.position;

        }
    }

    void MouseMovementCircleCheck()
    {
        DirectionCheck();


            if (lastX < InputManager.instance.MouseLocation().x)
            {
                towardsRight = true;
                //print("towardsRight");
            }
            if (lastX > InputManager.instance.MouseLocation().x)
            {
                towardsRight = false;
                //print("towardsLeft");
            }

            if (lastY > InputManager.instance.MouseLocation().y)
            {
                towardsDown = true;
                //print("towardsDown");
            }
            if (lastY < InputManager.instance.MouseLocation().y)
            {
                towardsDown = false;

                //print("towardsUp");
            }
            lastX = InputManager.instance.MouseLocation().x;
            lastY = InputManager.instance.MouseLocation().y;
        



    }

    void DirectionCheck()
    {
        if (lastX > InputManager.instance.MouseLocation().x && lastY > InputManager.instance.MouseLocation().y && 
            towardsRight && towardsDown)
        {
            clockwiseOrCounter = true;
        }
        else if (lastX > InputManager.instance.MouseLocation().x && lastY > InputManager.instance.MouseLocation().y && 
                 !towardsRight && !towardsDown)
        {
            clockwiseOrCounter = false;
        }


        if (lastX > InputManager.instance.MouseLocation().x && lastY < InputManager.instance.MouseLocation().y &&
               !towardsRight && towardsDown)
        {
            clockwiseOrCounter = true;
        }
        else if (lastX > InputManager.instance.MouseLocation().x && lastY < InputManager.instance.MouseLocation().y &&
               towardsRight && !towardsDown)
        {
            clockwiseOrCounter = false;
        }



        if (lastX < InputManager.instance.MouseLocation().x && lastY < InputManager.instance.MouseLocation().y &&
               !towardsRight && !towardsDown)
        {
            clockwiseOrCounter = true;
        }
        else if (lastX < InputManager.instance.MouseLocation().x && lastY < InputManager.instance.MouseLocation().y &&
               towardsRight && towardsDown)
        {
            clockwiseOrCounter = false;
        }



        if (lastX < InputManager.instance.MouseLocation().x && lastY > InputManager.instance.MouseLocation().y &&
               towardsRight && !towardsDown)
        {
            clockwiseOrCounter = true;
        }
        else if (lastX < InputManager.instance.MouseLocation().x && lastY > InputManager.instance.MouseLocation().y &&
               !towardsRight && towardsDown)
        {
            clockwiseOrCounter = false;
        }

        


    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    bool ReelBackYCheck()
    {
        if(lastY < InputManager.instance.MouseLocation().y)
        {
            lastY = InputManager.instance.MouseLocation().y;
            return true;
        }
        else
        {
            lastY = InputManager.instance.MouseLocation().y;
            return false;
        }

    }

    void LookAtBobber()
    {
        Vector3 bobberLocMod = bobber.transform.position;
        bobberLocMod.y = Camera.main.transform.position.y;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, bobberLocMod - Camera.main.transform.position, 2, 0.0f);
        Camera.main.transform.rotation = Quaternion.LookRotation(newDir);
    }


    public void TurnCamera(int direction)
    {
        turnDirection = direction;
    }
}
