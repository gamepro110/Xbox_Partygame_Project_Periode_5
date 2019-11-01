using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetable : Targetable
{
    [SerializeField] private Transform m_centerLoc = null;
    [SerializeField] private Vector3 m_axis = Vector3.up;
    [SerializeField] private float m_radius = 2.0f;
    [SerializeField] private float m_rotationSpeed = 80;

    private void Start()
    {
        transform.position = (transform.position - m_centerLoc.position).normalized * m_radius + m_centerLoc.position;
    }

    private void Update()
    {
        transform.RotateAround(m_centerLoc.position, m_axis, m_rotationSpeed * Time.deltaTime);
    }

    public override void Hit()
    {
        Debug.Log("hit");
    }
}

//https://answers.unity.com/questions/463704/smooth-orbit-round-object-with-adjustable-orbit-ra.html