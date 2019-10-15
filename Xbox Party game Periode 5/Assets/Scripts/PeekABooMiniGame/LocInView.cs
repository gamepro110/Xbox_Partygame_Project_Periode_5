using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocInView : MonoBehaviour
{
    private Guard_AI coneView = null;

    private void OnDrawGizmos()
    {
        if (coneView == null)
        {
            coneView = FindObjectOfType<Guard_AI>();
            if (coneView == null)
            {
                return;
            }
        }

        Gizmos.color = coneView.ConeVisual(transform.position) ? Color.magenta : Color.gray;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}