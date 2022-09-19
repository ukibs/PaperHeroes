using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    //
    public enum State
    {
        Moving,
        Stationary,

        Count
    }

    //
    public float speed = 1;
    public float stationaryTime = 1;
    public float movingTime = 1;
    public State initialState;

    private State currentState;
    private float currentStateTime;

    // Start is called before the first frame update
    void Start()
    {
        DecideNewDirection();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        currentStateTime += dt;

        float timeToCheck = 0;
        switch (currentState)
        {
            case State.Moving: timeToCheck = movingTime; break;
            case State.Stationary: timeToCheck = stationaryTime; break;
        }

        if(currentStateTime >= timeToCheck)
        {
            switch (currentState)
            {
                case State.Moving: currentState = State.Stationary; break;
                case State.Stationary: 
                    currentState = State.Moving;
                    DecideNewDirection();
                    break;
            }
            currentStateTime = 0;
        }

        if(currentState == State.Moving)
        {
            transform.Translate(Vector3.forward * dt * speed);
        }
    }

    void DecideNewDirection()
    {
        float newAngle = Random.Range(0f, 360f);
        float newX = Mathf.Cos(newAngle);
        float newZ = Mathf.Sin(newAngle);
        transform.LookAt(transform.position + new Vector3(newX, 0, newZ));
    }
}
