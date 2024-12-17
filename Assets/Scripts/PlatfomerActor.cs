using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfomerActor : MonoBehaviour
{
    [SerializeField]
    public string NamePlatfomer;

    public bool isActive;

    Rigidbody body;

    private void Awake()
    {
        NamePlatfomer = gameObject.name;
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isActive)
        {
            body.isKinematic = !isActive;
        }
    }
}
