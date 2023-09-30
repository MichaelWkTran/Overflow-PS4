using UnityEngine;
using static SaveSystem;

[CreateAssetMenu(fileName = "Furniture Item", menuName = "Shop Items/Furniture")]
public class FurnitureItem : ShopItemData
{
    [SerializeField] Sprite m_furnitureSprite;
    [SerializeField] Vector3 m_furniturePosition;
    [HideInInspector] public Transform m_furnitureObject; //The created furniture in the title screen
    bool m_enabled = false; //Whether the furniture is shown on the title screen
    public bool m_Enabled
    {
        get { return m_enabled; }
        set
        {
            m_enabled = value;

            //Show the furniture object or not
            if (m_furnitureObject != null) m_furnitureObject.gameObject.SetActive(m_enabled);

            //Save data on whether the furniture is enabled
            if (m_enabled) m_data.AddFurnitureItem(name);
            else m_data.m_enabledFurniture.Remove(name);
        }
    }

    public override void Init()
    {
        //Create furniture object
        m_furnitureObject = new GameObject().transform;
        m_furnitureObject.position = m_furniturePosition;
        m_furnitureObject.gameObject.AddComponent<SpriteRenderer>().sprite = m_furnitureSprite;
        m_furnitureObject.parent = FindObjectOfType<GameManager>().m_titleLevelSegment.transform;

        //Set whether the furniture is enabled
        m_furnitureObject.gameObject.SetActive(m_Enabled);
    }
    public override void OnClick()
    {
        //Enable or disable furniture
        m_Enabled = !m_Enabled;
    }
    public override void Load()
    {
        //Load data on whether the player had enabled this furniture
        if (m_data.m_enabledFurniture.Contains(name)) m_Enabled = true;
    }
}