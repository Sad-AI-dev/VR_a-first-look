using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;

    private void Start()
    {
        StartCoroutine(LifeTimeCo());
    }

    private IEnumerator LifeTimeCo()
    {
        yield return new WaitForSeconds(lifeTime);
        Die();
    }

    public void Die()
    {
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        yield return null;
        Destroy(gameObject);
    }

    //------collision------
    private void OnCollisionEnter(Collision collision)
    {
        Die();
    }
}
