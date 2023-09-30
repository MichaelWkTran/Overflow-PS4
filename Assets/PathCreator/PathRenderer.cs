using PathCreation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PathCreator), typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    LineRenderer m_lineRenderer;
    PathCreator m_pathCreator;

    void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_pathCreator = GetComponent<PathCreator>();
        m_lineRenderer.positionCount = m_pathCreator.path.NumPoints;
        UpdateLineRenderer();
    }
    
    void Update()
    {
        UpdateLineRenderer();
    }

    public void UpdateLineRenderer()
    {
        for (int i = 0; i < m_pathCreator.path.NumPoints; i++)
        {
            m_lineRenderer.SetPosition(i, m_pathCreator.path.GetPoint(i));
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PathRenderer))]
[CanEditMultipleObjects]
public class PathRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PathRenderer pathRenderer = (PathRenderer)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Update Line Renderer"))
        {
            pathRenderer.GetComponent<LineRenderer>().positionCount = pathRenderer.GetComponent<PathCreator>().path.NumPoints;
            pathRenderer.UpdateLineRenderer();
        }
    }
}
#endif