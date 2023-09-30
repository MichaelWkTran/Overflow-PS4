using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    [SerializeField] float segmentWidth; //How is the level segment
    [SerializeField, Range(0.0f, 100.0f)] float m_generateCarrotChance; //What percentage will carrots be generated
    [SerializeField] Transform m_carrotSegmentContainer; //The container from which the carrot segments will be stored
    [SerializeField] bool createdNextSegment = false; //Whether the next level segment has been created or not
    static float segmentGizmoHeight = 10.0f;  //The height of the level segment box displayed in gizmos
    GameManager gameManager;
    
    void Start()
    {
        //Get the game manager
        gameManager = FindObjectOfType<GameManager>();

        //Generate Carrot
        GenerateCarrot();
    }

    void Update()
    {
        //If the level segment appears on screen, create the next segment
        if (!createdNextSegment && transform.position.x < Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect))
        {
            createdNextSegment = true;
            Instantiate
            (
                gameManager.m_levelSegments[Random.Range(0, gameManager.m_levelSegments.Length)],
                transform.position + (Vector3.right * segmentWidth), Quaternion.identity
            ).transform.parent = transform.parent;
        }

        //Destroy the segment if it goes off screen
        if (transform.position.x + segmentWidth < Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect))
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube
        (
            transform.position + (new Vector3(segmentWidth, segmentGizmoHeight) /2.0f),
            new Vector3(segmentWidth, segmentGizmoHeight, 1.0f)
        );
    }

    void GenerateCarrot()
    {
        if (m_generateCarrotChance <= 0) { Destroy(m_carrotSegmentContainer.gameObject); return; }
        
        m_carrotSegmentContainer.gameObject.SetActive(true);
        if (m_carrotSegmentContainer.childCount <= 0) { Destroy(m_carrotSegmentContainer.gameObject); return; }

        if (Random.value * 100.0f > m_generateCarrotChance) { Destroy(m_carrotSegmentContainer.gameObject); return; }

        int carrotSegmentIndex = Random.Range(0, m_carrotSegmentContainer.childCount);
        m_carrotSegmentContainer.GetChild(carrotSegmentIndex).parent = transform;
        Destroy(m_carrotSegmentContainer.gameObject);

    }
}
