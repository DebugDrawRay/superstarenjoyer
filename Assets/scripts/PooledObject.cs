using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public GameObject MyParent;

    public void ReturnToPool()
    {
        transform.SetParent(MyParent.transform);
    }
}
