using UnityEngine;

public class Targetable : MonoBehaviour
{
    public virtual Vector3 GetMovementSpeed()
    {
        return Vector3.zero;
    }

    public virtual float GetDeadzone()
    {
        return 0;
    }

    public virtual void Hit()
    {
        Debug.Log("wrong hit");
    }
}