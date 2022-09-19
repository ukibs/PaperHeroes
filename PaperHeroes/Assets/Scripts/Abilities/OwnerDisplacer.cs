using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerDisplacer : MonoBehaviour
{
    //
    private BaseAbility baseAbility;
    // Start is called before the first frame update
    void Start()
    {
        baseAbility = GetComponent<BaseAbility>();
        baseAbility.user.movementStatus = BaseController.MovementStatus.ByAbility;
    }

    // Update is called once per frame
    void Update()
    {
        // Si no nos han matado al usuario...
        if(baseAbility.user)
            baseAbility.user.transform.position = transform.position;
    }

    private void OnDestroy()
    {
        baseAbility.user.movementStatus = BaseController.MovementStatus.Normal;
    }
}
