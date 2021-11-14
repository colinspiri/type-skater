using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skateboard : MonoBehaviour
{

    private Player player;
    public GameObject playerFeet;

    //private int animationStick = 0; //Like the talking stick but for animations!
    public float standingOffsetX;
    public float ollieOffsetY;
    public bool grounded = false; //Set by SkateboardGroudCheck object

    private Animation currentAnimation = Animation.None;

    private float lastAngleKickflip = 0f;
    private float lastAngleOllie = 0f;
    private float lastAngleGrind = 0f;



    private RotationInstruction[] ollieInstructions =  {
                    new RotationInstruction() { angle=20f,axis=Axis.OllieAxis,inTime=0.25f,startAt=0f },
                    new RotationInstruction() { angle=0f,axis=Axis.KickflipAxis,inTime=0.2f, startAt=0f } };
    
    private RotationInstruction[] wobbleInstructions = {
                    new RotationInstruction() { angle=40f, axis=Axis.KickflipAxis,inTime=0.1f,startAt=0f },
                    new RotationInstruction() { angle=-80f, axis=Axis.KickflipAxis,inTime=0.1f,startAt=0.1f },
                    new RotationInstruction() { angle=40f, axis=Axis.KickflipAxis,inTime=0.1f,startAt=0.2f },
                    new RotationInstruction() { angle=20f, axis=Axis.OllieAxis,inTime=0.1f,startAt=0f }
    };

    private RotationInstruction[] kickFlipInstructions = {
                    new RotationInstruction() { angle=360f,axis=Axis.KickflipAxis,inTime=0.25f,startAt=0f }
    };



    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }


    private bool axisExists(RotationInstruction[] instructions, Axis axis)
    {
        foreach (RotationInstruction r in instructions)
        {
            if (r.axis == axis)
            {
                return true;
            }

        }
        return false;
    }
    private RotationInstruction[] CreateRotationInstructions(RotationInstruction[] instructions)
    {
        int count = 0;
        if (!axisExists(instructions, Axis.GrindAxis)) count++;
        if (!axisExists(instructions, Axis.KickflipAxis)) count++;
        if (!axisExists(instructions, Axis.OllieAxis)) count++;

        RotationInstruction[] newInstructions = new RotationInstruction[instructions.Length + count];
        for (int i = 0; i < instructions.Length; i++)
        {
            newInstructions[i] = instructions[i];
        }
        int track = 0;
        if (!axisExists(instructions, Axis.GrindAxis))
        {
            newInstructions[instructions.Length] =
                new RotationInstruction() { angle = lastAngleGrind, axis = Axis.GrindAxis, inTime = 0.2f, startAt = 0f };
            track++;
        }
        if (!axisExists(instructions, Axis.KickflipAxis))
        {
            newInstructions[instructions.Length + track] =
    new RotationInstruction() { angle = lastAngleKickflip, axis = Axis.KickflipAxis, inTime = 0.2f, startAt = 0f };
            track++;
        }
        if (!axisExists(instructions, Axis.OllieAxis))
        {
            newInstructions[instructions.Length + track] =
    new RotationInstruction() { angle = lastAngleOllie, axis = Axis.OllieAxis, inTime = 0.2f, startAt = 0f };
        }
        return newInstructions;
    }

    void Update()
    {
        //Update position based on animation state
        if (currentAnimation == Animation.Ollie)
        {
            transform.position = RightUnderFeet(0, -0.01f);
        }
        else
        {
            transform.position = RightUnderFeet(0, 0f);
        }

        //Have to do level check
        if (Input.GetKey(KeyCode.Return) && player.state == Player.State.Midair && currentAnimation != Animation.LevelOut)
        {
            SetAnimation(Animation.LevelOut);
            Debug.Log("SA");
        }

    }

    public void SetAnimation(Animation animation)
    {

        StopAllCoroutines();
        this.currentAnimation = animation;
        switch (currentAnimation)
        {
            case Animation.None:
                Debug.Log("None!");
                transform.rotation = Quaternion.identity;
                break;
            case Animation.Ollie:
                grounded = false; //Needed to stop board from sticking to ground after ollie, a bit ugly tbh
                StartCoroutine(Ollie());
                break;
            case Animation.LevelOut:
                Debug.Log("LEveling Out!");
                StartCoroutine(LevelOut());
                break;
            case Animation.Kickflip:
                StartCoroutine(Kickflip());
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

    IEnumerator LevelOut()
    {
        RotationInstruction[] inst = {
            new RotationInstruction() { angle=0, axis=Axis.KickflipAxis, inTime=0.2f, startAt=0f},
            new RotationInstruction() { angle=0, axis=Axis.GrindAxis, inTime=0.2f, startAt=0f},
            new RotationInstruction() { angle=0, axis=Axis.OllieAxis, inTime=0.2f, startAt=0f},};
        yield return StartCoroutine(SetRotation(inst, false));
    }


    IEnumerator Kickflip()
    {
        yield return StartCoroutine(SetRotation(CreateRotationInstructions(kickFlipInstructions), false));
        SetAnimation(Animation.AirWobble);
    }

    IEnumerator Ollie()
    {
        yield return StartCoroutine(SetRotation(CreateRotationInstructions( ollieInstructions), false));
        SetAnimation(Animation.AirWobble);

    }

    IEnumerator AirWobble()
    {
        yield return StartCoroutine(SetRotation(CreateRotationInstructions( wobbleInstructions), true));
    }

    //SET Skateboard Rotation to X degrees, not Rotate. Give instructions for how much, how long rotation will take. Prob more useful
    IEnumerator SetRotation(RotationInstruction[] instructions, bool loop)
    {
        (float,float) lastAngleKickflip = (0,-1);
        (float, float) lastAngleOllie= (0, -1);
        (float, float) lastAngleGrind= (0, -1);
        for (int i =0; i< instructions.Length; i++)
        {
            float time = instructions[i].inTime + instructions[i].startAt;
            if (instructions[i].axis == Axis.GrindAxis && lastAngleGrind.Item2 < time)
            {
                lastAngleGrind = (instructions[i].angle, time);
            }
            else if(instructions[i].axis == Axis.OllieAxis && lastAngleOllie.Item2 < time)
            {
                lastAngleOllie= (instructions[i].angle, time);

            }
            else if(instructions[i].axis == Axis.KickflipAxis&& lastAngleKickflip.Item2 < time)
            {
                lastAngleKickflip= (instructions[i].angle, time);
            }
        }
        this.lastAngleGrind = lastAngleGrind.Item1;
        this.lastAngleOllie= lastAngleOllie.Item1;
        this.lastAngleKickflip= lastAngleKickflip.Item1;


        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion result = Quaternion.identity;
        float endTime = -1f;
        foreach(RotationInstruction i in instructions)
        {
            if (i.axis == Axis.GrindAxis)
            {
                result *= Quaternion.Euler(0, i.angle, 0);
            }
            else if (i.axis == Axis.KickflipAxis )
            {
                result*= Quaternion.Euler(i.angle, 0, 0);
            }
            else 
            {
                result *= Quaternion.Euler(0, 0, i.angle);
            }

            if (i.startAt + i.inTime > endTime)
            {
                endTime = i.startAt + i.inTime;
            }
        }

        while (elapsedTime<endTime)
        {
            Quaternion totalRotation = Quaternion.identity;
            for (int i = 0; i < instructions.Length; i++)
            {
                if (elapsedTime >= instructions[i].startAt)
                {
                    Quaternion quat = Quaternion.identity;
                    float rotationAmount = instructions[i].angle*((elapsedTime-instructions[i].startAt )/(instructions[i].inTime));
                    if (elapsedTime - instructions[i].startAt > instructions[i].inTime)
                    {
                        rotationAmount = instructions[i].angle;
                    }
                    if (instructions[i].axis == Axis.GrindAxis)
                    {
                        quat = Quaternion.Euler(0, rotationAmount, 0);
                    }
                    else if (instructions[i].axis == Axis.KickflipAxis)
                    {
                        quat = Quaternion.Euler(rotationAmount, 0, 0);
                    }
                    else if(rotationAmount != 0)
                    {
                        quat = Quaternion.Euler(0, 0, rotationAmount);
                    }
                    totalRotation *= quat;
                }
            }
            Quaternion finalRotation = Quaternion.Lerp(startRotation, totalRotation, (1 / endTime) * elapsedTime);
            transform.rotation = finalRotation;
            elapsedTime += Time.deltaTime;
            if (elapsedTime>endTime&& loop)
            {
                elapsedTime = 0;
                startRotation = transform.rotation;
            }
            yield return null;
        }
        transform.rotation = result;

        yield return new WaitForSeconds(0);
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
            if (completeCount == instructions.Length && loop)
            {

                Debug.Log("Reset");
                completeCount = 0;
                elapsedTime = 0;
                startRotation = transform.rotation;
                for (int i = 0; i < instructions.Length; i++)
                {
                    deltaAngles[i] = 0;
                }

                //transform.rotation = startRotation; ??
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
        LevelOut,
        Ollie,
        AirWobble,
        Kickflip
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