using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skateboard : MonoBehaviour
{

    private Player player;
    public GameObject playerFeet;


    public float standingOffsetX;
    public float ollieOffsetY;
    public bool grounded = false; //Set by SkateboardGroudCheck object

    private Animation currentAnimation = Animation.None;

    private IEnumerator currentRoutine;
    private RotationInstruction[] ollieInstructions = {
                    new RotationInstruction() { angle=20f,axis=Axis.OllieAxis,inTime=0.25f,startAt=0f },
                    new RotationInstruction() { angle=360f,axis=Axis.KickflipAxis,inTime=0.25f, startAt=0.25f }};
    RotationInstruction[] wobbleInstructions = {
                    new RotationInstruction() { angle=40f,axis=Axis.KickflipAxis,inTime=0.1f,startAt=0f },
                    new RotationInstruction() { angle=-80f,axis=Axis.KickflipAxis,inTime=0.1f,startAt=0.1f },
                    new RotationInstruction() { angle=40f,axis=Axis.KickflipAxis,inTime=0.1f,startAt=0.2f },
                };

    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    void Update()
    {
        //Update position based on animation state

        if (currentAnimation == Animation.None)
        {
            transform.rotation = Quaternion.identity;
            transform.position = RightUnderFeet(0, 0f);
        }
        else if (currentAnimation == Animation.Ollie)
        {
            //transform.position = RightUnderPlayer(0, 0);
            transform.position = RightUnderFeet(0, -0.01f);
        }
        transform.position = RightUnderFeet(0, 0f);



    }

    public void SetAnimation(Animation animation)
    {
        if (this.currentAnimation != animation && currentRoutine != null)
        {
          //StopCoroutine(currentRoutine);
            StopAllCoroutines();
        }
        this.currentAnimation = animation;


        //If NOT on ground do trick, ground check happens in FixedUpdate
        //Board will move differently based on tricks!
        //However, as soon as we hit ground (in FixedUpdate), board needs to very quickly return to starting state



        switch (currentAnimation)
        {
            case Animation.None:
                transform.rotation = Quaternion.identity;
                break;
            case Animation.Ollie:
                grounded = false; //Needed to stop board from sticking to ground after ollie, a bit ugly tbh
                currentRoutine = Ollie();
                StartCoroutine(currentRoutine);
                break;
            default:
                break;
        }
    }
    private Vector3 RightUnderFeet(float xOffset, float yOffset)
    {
        if (grounded)
        {
            return new Vector3(player.transform.position.x + xOffset, transform.position.y, transform.position.z);
        }

        float newY = Mathf.Lerp(transform.position.y, playerFeet.transform.position.y + yOffset, 15 * Time.deltaTime);
        if (newY > playerFeet.transform.position.y)
        {
            newY = playerFeet.transform.position.y;
        }
        return new Vector3(player.transform.position.x + xOffset, newY, transform.position.z);
    }



    IEnumerator Ollie()
    {
        yield return StartCoroutine(RotateSB(ollieInstructions, false));
        currentAnimation = Animation.AirWobble;
        currentRoutine = AirWobble();
        yield return StartCoroutine(currentRoutine);

    }

    IEnumerator AirWobble()
    {
        yield return StartCoroutine(RotateSB(wobbleInstructions, true));
    }

    IEnumerator ZeroRotation()
    {
        yield return null;
    }

    //SET Skateboard Rotation to X degrees, not Rotate. Give instructions for how much, how long rotation will take,
    //and when specifically in the sequence it will occur (e.g. kickflip rotation starts 0.1seconds in after the ollie)
    //IEnumerator SetRotateSB(RotationInstruction[] instructions, bool loop)
    //{
    //    //The + 3 is to add 3 resetting instructions to bring rotation to 0,0,0
    //    RotationInstruction[] newInstructions = new RotationInstruction[instructions.Length + 3];
    //    newInstructions[instructions.Length] = new RotationInstruction()
    //    {
    //        angle = 0,
    //        axis = Axis.GrindAxis,
    //        inTime = 1f,
    //        startAt = 0f
    //    };
    //    newInstructions[instructions.Length+1] = new RotationInstruction()
    //    {
    //        angle = 0,
    //        axis = Axis.GrindAxis,
    //        inTime = 1f,
    //        startAt = 0f
    //    };
    //    newInstructions[instructions.Length+2] = new RotationInstruction()
    //    {
    //        angle = 0,
    //        axis = Axis.GrindAxis,
    //        inTime = 1f,
    //        startAt = 0f
    //    };
    //    for (int i = 0; i < instructions.Length; i++)
    //    {
    //        if (instructions[i].axis == Axis.GrindAxis)
    //        {
                
    //            newInstructions[instructions.Length] = new RotationInstruction()
    //            {
    //                angle = -transform.rotation.eulerAngles.y,
    //                axis = instructions[i].axis,
    //                inTime = instructions[i].inTime,
    //                startAt = instructions[i].startAt
    //            };
    //        }
    //        else if (instructions[i].axis == Axis.KickflipAxis)
    //        {
    //            newInstructions[instructions.Length + 1] = new RotationInstruction()
    //            {
    //                angle = -transform.rotation.eulerAngles.x,
    //                axis = instructions[i].axis,
    //                inTime = instructions[i].inTime,
    //                startAt = instructions[i].startAt
    //            };
    //        }
    //        else
    //        {
    //            newInstructions[instructions.Length + 2] = new RotationInstruction()
    //            {
    //                angle = -transform.rotation.eulerAngles.z,
    //                axis = instructions[i].axis,
    //                inTime = instructions[i].inTime,
    //                startAt = instructions[i].startAt
    //            };
    //        }

    //        //Goal rotation - current rotation = how much rotation you need
    //        newInstructions[i] = new RotationInstruction()
    //        {
    //            angle = instructions[i].angle,
    //            axis = instructions[i].axis,
    //            inTime = instructions[i].inTime,
    //            startAt = instructions[i].startAt
    //        };


    //    }
    //    yield return StartCoroutine(RotateSB(newInstructions, loop));
    //}

    //Rotate Skateboard BY X degrees, not SET.
    IEnumerator RotateSB(RotationInstruction[] instructions, bool loop)
    {
        float elapsedTime = 0f;
        float[] deltaAngles = new float[instructions.Length];
        for (int i = 0; i < instructions.Length; i++)
        {
            deltaAngles[i] = 0;
        }
        int completeCount = 0;
        Quaternion startRotation = transform.rotation;
        while (completeCount < instructions.Length || loop)
        {
            Quaternion totalRotation = Quaternion.identity;
            for (int i = 0; i < instructions.Length; i++)
            {
                if (elapsedTime >= instructions[i].startAt)
                {
                    // calculate rotation speed
                    float rotationSpeed = instructions[i].angle / instructions[i].inTime;
                    if (instructions[i].angle >= 0)
                    {
                        if (deltaAngles[i] < instructions[i].angle)
                        {
                            // rotate until reaching angle
                            deltaAngles[i] += rotationSpeed * Time.deltaTime;
                            deltaAngles[i] = Mathf.Min(deltaAngles[i], instructions[i].angle);
                            if (deltaAngles[i] == instructions[i].angle)
                            {
                                completeCount++;
                            }
                        }
                    }
                    else if (deltaAngles[i] > instructions[i].angle)
                    {
                        // rotate until reaching angle
                        deltaAngles[i] += rotationSpeed * Time.deltaTime;
                        deltaAngles[i] = Mathf.Max(deltaAngles[i], instructions[i].angle);
                        if (deltaAngles[i] == instructions[i].angle)
                        {
                            completeCount++;
                        }
                    }

                }
                Vector3 axis;
                if (instructions[i].axis == Axis.GrindAxis)
                {
                    axis = new Vector3(0, 1, 0);
                }
                else if (instructions[i].axis == Axis.KickflipAxis)
                {
                    axis = new Vector3(1, 0, 0);
                }
                else
                {
                    axis = new Vector3(0, 0, 1); //OllieAxis
                }
                totalRotation *= Quaternion.AngleAxis(deltaAngles[i], axis);

            }
            transform.rotation = startRotation * totalRotation;
            elapsedTime += Time.deltaTime;
            if (  completeCount==instructions.Length&& loop)
            {
                
                Debug.Log("Reset");
                completeCount = 0;
                elapsedTime = 0;
                startRotation = transform.rotation;
                for (int i = 0; i < instructions.Length; i++)
                {
                    deltaAngles[i] = 0;
                }
                
                transform.rotation = startRotation;
                //yield return StartCoroutine(RotateSB(instructions, loop));
            }

            yield return null;


        }
        yield return new WaitForSeconds(0);
        // delay here
        //}
    }


    struct RotationInstruction
    {
        public float angle;
        public Axis axis;
        public float inTime;
        public float startAt;
    }

    public enum Animation
    {
        None,
        Ollie,
        AirWobble
    }
    private enum Axis
    {
        KickflipAxis,
        OllieAxis,
        GrindAxis
    }
}
//IEnumerator RotateObject(float angle, Vector3 axis, float inTime)
//{
//    // calculate rotation speed
//    float rotationSpeed = angle / inTime;

//    //while (true)
//    //{
//    // save starting rotation position
//    Quaternion startRotation = transform.rotation;

//    float deltaAngle = 0;

//    // rotate until reaching angle
//    while (deltaAngle < angle)
//    {
//        deltaAngle += rotationSpeed * Time.deltaTime;
//        deltaAngle = Mathf.Min(deltaAngle, angle);

//        transform.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

//        yield return null;
//    }

//    // delay here
//    yield return new WaitForSeconds(1);
//    //}
//}