using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFollower : MonoBehaviour
{
    private BaseAbility baseAbility;

    // Start is called before the first frame update
    void Start()
    {
        baseAbility = GetComponent<BaseAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = baseAbility.user.transform.position;
    }
}
