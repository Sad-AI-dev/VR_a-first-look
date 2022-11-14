using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    [SerializeField] UnityEvent onHit;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Projectile")) {
            onHit?.Invoke();
            StartCoroutine(DieCo());
        }
    }

    private IEnumerator DieCo()
    {
        yield return null;
        Destroy(gameObject);
    }

    //-------score--------
    public void AddScore(float toAdd)
    {
        if (ScoreManager.instance != null) {
            ScoreManager.instance.AddScore(toAdd);
        }
    }
}
