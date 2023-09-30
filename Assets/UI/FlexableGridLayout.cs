using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexableGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    
    public FitType m_fitType;

    public int m_rows, m_columns;
    public Vector2 m_cellSize;
    public Vector2 m_spacing;

    public bool m_fitX, m_fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (m_fitType == FitType.Width || m_fitType == FitType.Height || m_fitType == FitType.Uniform)
        {
            m_fitX = true;
            m_fitY = true;
            
            float Sqrt = Mathf.Sqrt(transform.childCount);
            m_rows = m_columns = Mathf.CeilToInt(Sqrt);
        }

        switch (m_fitType)
        {
            case FitType.Width:
            case FitType.FixedColumns:
                m_rows = Mathf.CeilToInt(transform.childCount / (float)m_columns);
                break;
            case FitType.Height:
            case FitType.FixedRows:
                m_columns = Mathf.CeilToInt(transform.childCount / (float)m_rows);
                break;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)m_columns) - ((m_spacing.x / (float)m_columns) * 2) - 
                          (padding.left / (float)m_columns) - (padding.right / (float)m_columns);
        float cellHeight = (parentHeight / (float)m_rows) - ((m_spacing.y / (float)m_rows) * 2) - 
                           (padding.top / (float)m_rows) - (padding.bottom / (float)m_rows);

        m_cellSize.x = m_fitX ? cellWidth : m_cellSize.x;
        m_cellSize.y = m_fitY ? cellHeight : m_cellSize.y;

        int rowCount, columnCount;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / m_columns;
            columnCount = i % m_columns;

            var item = rectChildren[i];

            var posX = (m_cellSize.x * columnCount) + (m_spacing.x * columnCount) + padding.left;
            var posY = (m_cellSize.y * rowCount) + (m_spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, posX, m_cellSize.x);
            SetChildAlongAxis(item, 1, posY, m_cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
