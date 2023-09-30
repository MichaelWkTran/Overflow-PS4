using UnityEngine;
using static SaveSystem;

[CreateAssetMenu(fileName = "Skin Item", menuName = "Shop Items/Skin")]
public class SkinItem : ShopItemData
{
    public static SkinItem m_currentSkin = null;
    [SerializeField] RuntimeAnimatorController m_skin;
    [SerializeField] Sprite m_deathSprite;

    public RuntimeAnimatorController m_Skin { get { return m_skin; } }
    public Sprite m_DeathSprite { get { return m_deathSprite; } }

    public void SetSkin()
    {
        //Find the player
        Player player = FindObjectOfType<Player>();

        //Stop the function if the player does not exist
        if (player == null) return;

        //Save Data
        m_currentSkin = this;
        m_data.m_currentSkinName = name;

        //Set the skin of the player
        player.GetComponent<Animator>().runtimeAnimatorController = m_skin;
        player.deathParticle.textureSheetAnimation.SetSprite(0, m_deathSprite);
    }
    public override void OnClick()
    {
        SetSkin();
    }
    public override void Load()
    {
        base.Load();

        //Load data on whether the player had enabled this skin
        if (m_data.m_currentSkinName == name) SetSkin();
    }
}