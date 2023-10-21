using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    [SerializeField] private Vector3 Rotation;
    [SerializeField] float Speed;

    void Update()
    {
        transform.Rotate(Rotation * Speed * Time.deltaTime);
    }
}
