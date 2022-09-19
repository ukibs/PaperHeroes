using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActorController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Moving,
        Count
    }

    private Vector3 destination;
    private float inactiveTime;
    private float currentInactiveTime;
    private State state = State.Idle;

    private SpriteController spriteController;

    // Start is called before the first frame update
    void Start()
    {
        spriteController = GetComponentInChildren<SpriteController>();
        DecideNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        switch (state)
        {
            case State.Moving:
                if ((transform.position - destination).sqrMagnitude > 1)
                {
                    transform.position += (destination - transform.position).normalized * 1 * dt;
                }
                else
                {
                    state = State.Idle;
                    spriteController.status = SpriteController.Status.Idle;
                }
                break;
            case State.Idle:
                currentInactiveTime += dt;
                if(currentInactiveTime >= inactiveTime)
                {
                    DecideNewDestination();
                    currentInactiveTime = 0;
                }
                break;
        }
        
    }

    void DecideNewDestination()
    {
        //
        destination = new Vector3(Random.Range(-3, 3), 0.5f, Random.Range(-3, 3));
        //
        state = State.Moving;
        spriteController.status = SpriteController.Status.Moving;
        //
        inactiveTime = Random.Range(1f, 5f);
        currentInactiveTime = 0;
    }
}
