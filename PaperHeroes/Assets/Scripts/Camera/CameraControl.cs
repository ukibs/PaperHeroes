using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public enum State 
    { 
        Idle, Pull, Push, Count    
    }

    private Vector3 originalPosition;
    private PlayersManager playersManager;
    private Camera myCam;
    private State currentState = State.Idle;

    // Start is called before the first frame update
    void Start()
    {
        playersManager = FindObjectOfType<PlayersManager>();
        myCam = GetComponent<Camera>();

        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersManager.PlayerControllerList.Count > 0)
        {
            //
            CheckPlayersInScreen();

            //
            if (currentState == State.Push)
            {
                transform.Translate(Vector3.back * Time.deltaTime);
            }
            else if (currentState == State.Pull) 
            {
                transform.Translate(Vector3.forward * Time.deltaTime);
                if (transform.localPosition.sqrMagnitude < originalPosition.sqrMagnitude)
                {
                    currentState = State.Idle;
                }
            }
        }
    }

    // funcion calcular lejania
    void CheckPlayersInScreen()
    {
        //
        int playersOutOfScreen = 0;
        //
        for (int i = 0; i < playersManager.PlayerControllerList.Count; i++)
        {
            //
            if (!playersManager.PlayerControllerList[i].isDead)
            {
                Vector3 screenPos = myCam.WorldToScreenPoint(playersManager.PlayerControllerList[i].transform.position);

                screenPos = new Vector3(screenPos.x - (Screen.width / 2f), screenPos.y - (Screen.height / 2f), 0f);

                if (screenPos.sqrMagnitude > Mathf.Pow(Screen.height / 2.5f, 2))
                {
                    playersOutOfScreen++;
                    //currentState = State.Push;
                }
                //else if(currentState != State.Idle)
                //{
                //    currentState = State.Pull;
                //}
            }
        }
        //
        if(playersOutOfScreen > 0)
        {
            currentState = State.Push;
        }
        //else if (currentState != State.Idle)
        else if (currentState == State.Push)
        {
            currentState = State.Pull;
        }
    }


}
