using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [field: SerializeField] public float rotateSpeed { get; private set; } = 50;
    
    public bool isClicked = false;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isClicked = !isClicked;
        }

        if (isClicked)
        {
            transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.one);
        }
    }
}
