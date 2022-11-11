using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public GameObject prefab;

    public void Emit()
    {
        Transform t = Instantiate(prefab).transform;
        t.position = transform.position;
    }
}
