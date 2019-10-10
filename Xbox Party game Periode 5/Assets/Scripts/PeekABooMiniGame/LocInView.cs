using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocInView : MonoBehaviour
{
    private GuardAI coneView = null;

    private void OnDrawGizmos()
    {
        if (coneView == null)
        {
            coneView = FindObjectOfType<GuardAI>();
            if (coneView == null)
            {
                return;
            }
        }

        Gizmos.color = coneView.ConeVisual(transform.position) ? Color.magenta : Color.black;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}