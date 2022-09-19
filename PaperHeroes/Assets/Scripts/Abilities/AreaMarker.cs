using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMarker : MonoBehaviour
{
    //
    public float area = 1;
    public Transform markerCircle;

    //
    private void Start()
    {
        if(markerCircle == null)
        {
            SpawnAreaMarker();
        }
        //
        markerCircle.localScale = markerCircle.localScale * area;
    }

    void SpawnAreaMarker()
    {
        //GameObject areaPrefab = 
    }
}
