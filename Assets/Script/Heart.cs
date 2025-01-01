using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int life = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void hit(int damage)
    {
        life -= damage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
