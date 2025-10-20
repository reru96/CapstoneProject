using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core;
using Gameplay;
using UnityEngine;

public class DamagePopUp: MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public float moveSpeed = 1.5f;
    public float fadeDuration = 0.6f;

    public float scaleUp = 1.3f;    
    public float scaleDownSpeed = 3f;

    private float timer;
    private Color startColor;
    private Vector3 startScale;

    private bool isCritical;

    public void Setup(float damage, bool criticalHit = false)
    {
        isCritical = criticalHit;

        textMesh.text = damage.ToString("0");
        startColor = criticalHit ? Color.red : Color.yellow;

        textMesh.color = startColor;
        startScale = transform.localScale;

        if (criticalHit)
            transform.localScale = startScale * scaleUp;
    }

    private void Update()
    {
   
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (isCritical && transform.localScale.magnitude > startScale.magnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * scaleDownSpeed);
        }

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= fadeDuration)
            Destroy(gameObject);
    }

  
    private void LateUpdate()
    {
        if (Camera.main)
            transform.LookAt(Camera.main.transform);
    }

}
