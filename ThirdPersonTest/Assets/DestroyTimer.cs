using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    // Start is called before the first frame update

    public float destroyDelay = 1f; // Time in seconds before the object gets destroyed

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

}
