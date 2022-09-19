using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
    private static CameraPivotController instance;

    private PlayersManager playersManager;

    private BossController currentBossController;

    public static CameraPivotController Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        playersManager = FindObjectOfType<PlayersManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //
        if(playersManager.PlayerControllerList.Count > 0)
        {
            //transform.position = playersManager.PlayerControllerList[0].transform.position;
            FollowPlayers();
        }
        //
        if (EnemiesManager.Instance.bossGroup.activeSelf)
        {
            //
            if (!currentBossController)
                currentBossController = FindObjectOfType<BossController>();
            //
            transform.LookAt(currentBossController.transform);
        }
        else
        {
            transform.LookAt(Vector3.zero);
        }
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    //
    void FollowPlayers()
    {
        // declaramos cosas
        Vector3 finalposition = new Vector3();
        finalposition = Vector3.zero;
        int alivePlayers = 0;

        for (int i = 0; i < playersManager.PlayerControllerList.Count; i++)
        {
            // Si está muerto no lo contamos
            if (!playersManager.PlayerControllerList[i].isDead)
            {
                finalposition += playersManager.PlayerControllerList[i].transform.position;
                alivePlayers++;
            }
                
        }
        //
        if(alivePlayers > 0)
        {
            //finalposition /= playersManager.PlayerControllerList.Count;
            finalposition /= alivePlayers;
            transform.position = finalposition;
        }
        
    }
}
