using UnityEngine;
using static SaveSystem;

[CreateAssetMenu(fileName = "Hat Item", menuName = "Shop Items/Hat")]
public class HatItem : ShopItemData
{
    public static HatItem m_currentHat = null;
    [SerializeField] Sprite m_hatSprite;

    public Sprite m_HatSprite { get { return m_hatSprite; } }

    public void SetHat()
    {
        //Find the player
        Player player = FindObjectOfType<Player>();

        //Stop the function if the player does not exist
        if (player == null) return;

        //Save Data
        m_currentHat = this;
        m_data.m_currentHatName = name;

        //Set the skin of the player
        player.m_hatSpriteRenderer.sprite = m_hatSprite;
    }
    public override void OnClick()
    {
        //Save Data
        SetHat();
    }
    public override void Load()
    {
        base.Load();

        //Load data on whether the player had enabled this hat
        if (m_data.m_currentHatName == name)
        {
            SetHat();
        }
    }
}