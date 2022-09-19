using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachedEffect : MonoBehaviour
{
    //
    public float lifeTime = 1;
    //
    [HideInInspector] public Transform objective;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        //
        //transform.parent = objective;
        //transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (objective)
        {
            //transform.position = objective.position;
        }
        //
        if(objective && transform.parent == null)
        {
            transform.parent = objective;
            transform.localPosition = Vector3.zero;
        }
    }
}
