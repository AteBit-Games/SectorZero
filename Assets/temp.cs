using System.Collections;
using System.Collections.Generic;
using Runtime.AI;
using UnityEngine;

public class temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NoiseEmitter>().EmitLocal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
