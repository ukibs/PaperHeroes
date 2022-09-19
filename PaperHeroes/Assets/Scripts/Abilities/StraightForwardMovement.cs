using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightForwardMovement : MonoBehaviour
{
    // Parametros
    [Header("Base Parameters")]
    public float lifeTime = 2;
    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
