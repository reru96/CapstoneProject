using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Poolable : MonoBehaviour
{

    [SerializeField] private float returnDelay = 0f;
    private Coroutine returnCoroutine;

    private void OnEnable()
    {
        if (returnDelay > 0f)
            returnCoroutine = StartCoroutine(AutoReturn());
    }

    private void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    private IEnumerator AutoReturn()
    {
        yield return new WaitForSeconds(returnDelay);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler != null)
            pooler.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }

    public void SetReturnDelay(float delay)
    {
        returnDelay = delay;
    }
}
