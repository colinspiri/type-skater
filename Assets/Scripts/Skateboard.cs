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

    private string currentTrick = "";

    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    void FixedUpdate()
    {
        if (player.state == Player.State.OnGround || player.state == Player.State.OnRail)
        {
            currentTrick = "";
            transform.rotation = Quaternion.identity;
        }
        if (currentTrick == "ollie")
        {
            //transform.position = RightUnderPlayer(0, 0);
        }
        transform.position = TopUnderFeet();

    }

    public void SetTrick(string trick)
    {
        trick = trick.ToLower(); //to make function case-insensitive
        this.currentTrick = trick;

        //If NOT on ground do trick, ground check happens in FixedUpdate
        //Board will move differently based on tricks!
        //However, as soon as we hit ground (in FixedUpdate), board needs to very quickly return to starting state

        Vector3 kickFlipAxis = new Vector3(1, 0, 0);
        Vector3 ollieAxis = new Vector3(0, 0, 1);

        switch (currentTrick)
        {
            case "ollie":
                RotationInstruction[] ollieInstructions = { new RotationInstruction() { angle=20f,axis=ollieAxis,inTime=0.25f },
                new RotationInstruction() { angle=360f,axis=kickFlipAxis,inTime=0.25f }};
                StartCoroutine(RotateObject(ollieInstructions));
                //StartCoroutine(RotateObject(360f, kickFlipAxis, 0.25f));

                break;
            default:
                break;
        }
    }

    private Vector3 TopUnderFeet()
    {
        return new Vector3(player.transform.position.x, playerFeet.transform.position.y, transform.position.z);
    }
    private Vector3 RightUnderPlayer(float xOffset, float yOffset)
    {
        float newY = Mathf.Lerp(transform.position.y, playerFeet.transform.position.y+yOffset, 20* Time.deltaTime);

        return new Vector3(player.transform.position.x + xOffset, newY, transform.position.z);
    }


    IEnumerator RotateObject(RotationInstruction[] instructions)
    {
        float[] deltaAngles = new float[instructions.Length];
        for (int i = 0; i < instructions.Length; i++)
        {
            deltaAngles[i] = 0;
        }
        int completeCount = 0;
        Quaternion startRotation = transform.rotation;
        while (completeCount < instructions.Length)
        {
            Quaternion angleAxisSum = Quaternion.identity;
            for (int i = 0; i < instructions.Length; i++)
            {

                // calculate rotation speed
                float rotationSpeed = instructions[i].angle / instructions[i].inTime;

                // rotate until reaching angle
                if (deltaAngles[i] < instructions[i].angle)
                {
                    deltaAngles[i] += rotationSpeed * Time.deltaTime;
                    deltaAngles[i] = Mathf.Min(deltaAngles[i], instructions[i].angle);
                    angleAxisSum *= Quaternion.AngleAxis(deltaAngles[i], instructions[i].axis); ;
                }
                else
                {
                    completeCount++;
                }
            }
            if (completeCount!=instructions.Length)
            {
                transform.rotation = startRotation * angleAxisSum;
            }
            yield return null;

        }
        // delay here
        yield return new WaitForSeconds(1);
        //}
    }

    IEnumerator RotateObject(float angle, Vector3 axis, float inTime)
    {
        // calculate rotation speed
        float rotationSpeed = angle / inTime;

        //while (true)
        //{
        // save starting rotation position
        Quaternion startRotation = transform.rotation;

        float deltaAngle = 0;

        // rotate until reaching angle
        while (deltaAngle < angle)
        {
            deltaAngle += rotationSpeed * Time.deltaTime;
            deltaAngle = Mathf.Min(deltaAngle, angle);

            transform.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

            yield return null;
        }

        // delay here
        yield return new WaitForSeconds(1);
        //}
    }
    struct RotationInstruction
    {
        public float angle;
        public Vector3 axis;
        public float inTime;
    }

}
