using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlert : MonoBehaviour
{
    public Image alertIcon;
    [SerializeField] private Color alertColor = Color.yellow;
    [SerializeField] private Color chaseColor = Color.red;
    [SerializeField] private float colorLerpSpeed = 2f;

    private Color targetColor;
    private bool isVisible = false;
    private EnemyPathController enemy;

    private void Awake()
    {
        if (alertIcon != null)
        {
            alertIcon.enabled = false;
            targetColor = alertColor;
        }

        
        enemy = GetComponentInParent<EnemyPathController>();
    }

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (isVisible && alertIcon != null)
        {
            alertIcon.color = Color.Lerp(alertIcon.color, targetColor, Time.deltaTime * colorLerpSpeed);
        }
    }

    private void HandleStateChanged(EnemyState newStateObj)
    {

        switch (newStateObj)
        {
            case EnemyState.Patrolling:
                HideIcon();
                break;
            case EnemyState.Alert:
                ShowIcon(alertColor);
                break;
            case EnemyState.Chasing:
                ShowIcon(chaseColor);
                break;
        }
    }

    private void ShowIcon(Color color)
    {
        if (alertIcon == null) return;
        isVisible = true;
        alertIcon.enabled = true;
        targetColor = color;
    }

    private void HideIcon()
    {
        if (alertIcon == null) return;
        isVisible = false;
        alertIcon.enabled = false;
    }
}

