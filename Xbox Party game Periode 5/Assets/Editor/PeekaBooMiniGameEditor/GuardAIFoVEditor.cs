using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode, CustomEditor(typeof(Guard_AI))]
public class GuardAIFoVEditor : Editor
{
    private void OnSceneGUI()
    {
        Guard_AI FoV = (Guard_AI)target;
        Handles.color = Color.clear;
        Handles.DrawWireArc(FoV.transform.position, Vector3.up, Vector3.forward, 360f, FoV.m_viewRadius);

        Vector3 viewAngleA = FoV.DirFromAngle(-FoV.m_viewAngle / 2, false);
        Vector3 viewAngleB = FoV.DirFromAngle(FoV.m_viewAngle / 2, false);

        Handles.DrawLine(FoV.transform.position, FoV.transform.position + viewAngleA * FoV.m_viewRadius);
        Handles.DrawLine(FoV.transform.position, FoV.transform.position + viewAngleB * FoV.m_viewRadius);
    }
}