using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseXTimes : BaseAbility
{
    public enum Type
    {
        Inmediate,
        OverTime,
        OneForPlayer,

        Count
    }

    public Type type;
    public int times;
    public float interval;
    public GameObject prefabToUse;

    // Start is called before the first frame update
    void Start()
    {
        switch(type)
        {
            case Type.Inmediate:
                for(int i = 0; i < times; i++)
                {

                }
                break;
            case Type.OneForPlayer:
                for (int i = 0; i < PlayersManager.Instance.PlayerControllerList.Count; i++)
                {
                    GameObject nextUse = Instantiate(prefabToUse, transform.position, Quaternion.identity);
                    nextUse.transform.LookAt(PlayersManager.Instance.PlayerControllerList[i].transform);
                    nextUse.transform.eulerAngles = new Vector3(0, nextUse.transform.eulerAngles.y, 0);
                    BaseAbility baseAbility = nextUse.GetComponent<BaseAbility>();
                    if (baseAbility)
                    {
                        baseAbility.GetDataFromPreviousAbility(this);
                    }
                }
                break;
            case Type.OverTime:
                StartCoroutine(OverTimeCoroutine());
                break;
        }
        //
        if (type != Type.OverTime)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //
        transform.LookAt(objective);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    IEnumerator OverTimeCoroutine()
    {
        for(int i = 0; i < times; i++)
        {
            //
            GameObject nextUse = Instantiate(prefabToUse, transform.position, transform.rotation);
            BaseAbility baseAbility = nextUse.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.GetDataFromPreviousAbility(this);
            }
            //
            yield return new WaitForSeconds(interval);
        }
        //
        Destroy(gameObject);
    }
}
