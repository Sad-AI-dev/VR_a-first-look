using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    public float lifeTime = 1f;

    private void Start()
    {
        StartCoroutine(LifeTimeCo());
    }

    private IEnumerator LifeTimeCo()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy();
    }

    //------------destroy-----------------
    public void Destroy()
    {
        StartCoroutine(DestroyCo());
    }

    private IEnumerator DestroyCo()
    {
        yield return null;
        Destroy(gameObject);
    }
}
