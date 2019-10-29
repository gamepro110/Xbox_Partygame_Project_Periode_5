using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    protected Transform m_targetable;
    protected Vector3 M_Speed;

    public Vector3 GetMovementSpeed()
    {
        return M_Speed;
    }

    public Transform GetTargetTransform()
    {
        return m_targetable;
    }

    public abstract float GetDeadzone();

    public abstract GameObject GetGameObject();
}