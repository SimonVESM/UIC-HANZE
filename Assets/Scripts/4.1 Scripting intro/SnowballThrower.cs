using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballThrower : MonoBehaviour
{
    [SerializeField] private Rigidbody snowballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float snowballSpeed = 15;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody snowball = Instantiate(snowballPrefab, spawnPoint.position, spawnPoint.rotation);
            snowball.velocity = spawnPoint.forward * snowballSpeed;
            
            Destroy(snowball.gameObject, 2);
        }
    }
}
