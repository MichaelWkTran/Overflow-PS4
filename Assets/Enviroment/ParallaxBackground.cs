using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    public Vector2 m_parallaxMultiplier;
    public bool m_infiniteHorizontal;
    public bool m_infiniteVertical;

    Transform m_cameraTransform;
    Vector3 m_lastCameraPosition;
    Vector2 m_textureUnitSize;

    void Start()
    {
        m_cameraTransform = Camera.main.transform;
        m_lastCameraPosition = m_cameraTransform.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        m_textureUnitSize.x = texture.width * transform.lossyScale.x;
        m_textureUnitSize.y = texture.height * transform.lossyScale.y;
        m_textureUnitSize = m_textureUnitSize / sprite.pixelsPerUnit;
    }

    public void ClearLastCameraPosition()
    {
        m_lastCameraPosition = m_cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = m_cameraTransform.position - m_lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * m_parallaxMultiplier.x, deltaMovement.y * m_parallaxMultiplier.y, 0);
        m_lastCameraPosition = m_cameraTransform.position;

        //Loop the texture horizontally
        if (m_infiniteHorizontal)
        {
            if (Mathf.Abs(m_cameraTransform.position.x - transform.position.x) >= m_textureUnitSize.x)
            {
                float offsetPositionX = (m_cameraTransform.position.x - transform.position.x) % m_textureUnitSize.x;
                transform.position = new Vector3(m_cameraTransform.position.x + offsetPositionX, transform.position.y, 0);
            }
        }

        //Loop the texture vertically
        if (m_infiniteVertical)
        {
            if (Mathf.Abs(m_cameraTransform.position.y - transform.position.y) >= m_textureUnitSize.y)
            {
                float offsetPositionY = (m_cameraTransform.position.y - transform.position.y) % m_textureUnitSize.y;
                transform.position = new Vector3(transform.position.x, m_cameraTransform.position.y + offsetPositionY, 0);
            }
        }
    }
}
