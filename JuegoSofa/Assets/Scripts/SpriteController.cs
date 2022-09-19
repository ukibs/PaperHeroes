using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    public enum Status
    {
        Idle,
        Moving,
        Dead,

        Count
    }

    public Status status;
    public bool inMap = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private SpriteRenderer spriteRenderer;
    private MeshRenderer meshRenderer;

    private float fadeDuration = 0.5f;
    private float currentFadeDuration = 0;
    private Color spritecolor = Color.white;

    private float extraTime;

    public Color SpriteColor { get { return spritecolor; } }

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        //
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        //
        extraTime = UnityEngine.Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //
        UpdateDamageColor(Time.deltaTime);

        //
        switch (status)
        {
            case Status.Idle:
                transform.localPosition = originalPosition;
                break;

            case Status.Moving:
                //transform.localPosition = Vector3.up * 0.5f * Math.Abs(Mathf.Sin(Time.time * 10));
                transform.localScale = new Vector3(originalScale.x + (originalScale.x * 0.1f) * Mathf.Sin((Time.time + extraTime) * 10),
                                                    originalScale.y + (originalScale.y * 0.1f) * -Mathf.Sin((Time.time + extraTime) * 10),
                                                    originalScale.z + (originalScale.z * 0.1f) * -Mathf.Sin((Time.time + extraTime) * 10));
                break;
        }
        //
        if(status != Status.Dead && !inMap && CameraPivotController.Instance)
            //transform.LookAt(transform.position + Vector3.back);
            //transform.LookAt(transform.position + Vector3.forward);
            //transform.LookAt(transform.position + CameraPivotController.Instance.transform.forward);
            transform.rotation = CameraPivotController.Instance.transform.rotation;
    }

    #region Methods

    void UpdateDamageColor(float dt)
    {
        //
        if(currentFadeDuration > 0)
        {
            currentFadeDuration -= dt;
            spritecolor = new Color(1, 1f - (currentFadeDuration / fadeDuration), 1f - (currentFadeDuration / fadeDuration));

            if (spriteRenderer)
            {
                spriteRenderer.color = spritecolor;
            }

            if (meshRenderer)
            {
                meshRenderer.material.color = spritecolor;
            }
        }
    }

    public void ShowDamage()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.red;
        }

        if (meshRenderer)
        {
            meshRenderer.material.color = Color.red;
        }

        currentFadeDuration = fadeDuration;
    }

    #endregion
}
