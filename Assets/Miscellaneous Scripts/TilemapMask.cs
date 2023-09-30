using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMask : MonoBehaviour
{
    [SerializeField] Tilemap m_tilemap;
    public Tilemap TileMap { get { return m_tilemap; } }

    void Start()
    {
        if (m_tilemap == null) m_tilemap = GetComponent<Tilemap>();
        Vector3Int origin = m_tilemap.origin;

        //Iterate over each cell
        for (int x = origin.x; x < origin.x + m_tilemap.size.x; x++)
            for (int y = origin.y; y < origin.y + m_tilemap.size.y; y++)
                //Check if the cell isn't empty
                if (m_tilemap.GetTile(new Vector3Int(x, y, origin.z)) != null)
                {
                    //Create maskCell on the cell coords
                    Vector3 coord = m_tilemap.CellToWorld(new Vector3Int(x, y, origin.z)) + 
                        new Vector3(0.5f * m_tilemap.cellSize.x, 0.5f * m_tilemap.cellSize.y, 0);

                    GameObject maskedCell = new GameObject();
                    maskedCell.transform.position = coord;
                    maskedCell.transform.parent = transform;
                    maskedCell.AddComponent<SpriteMask>().sprite = m_tilemap.GetSprite(new Vector3Int(x, y, origin.z));
                }
    }
}
