using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateEscenario2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         transform.Rotate(new Vector3(0, 1.5f, 1.5f));
    }
}
