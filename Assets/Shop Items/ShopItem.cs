using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SaveSystem;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ShopItemData m_data; public ShopItemData m_Data { get { return m_data; } }
    [SerializeField] Button m_itemButton; //The button from which the player uses to purchase the item
    [SerializeField] Sprite m_equipSprite;
    [SerializeField] RectTransform m_purchaseMessage; //The message that appears when the player selects that item to purchase
    [SerializeField] TMPro.TMP_Text m_priceText; //The text that shows the price of the item
    
    void Start()
    {
        //Display the price of the shop item
        m_priceText.text = m_data.m_Price.ToString();

        if (m_data.m_purchased)
        {
            GetComponent<Image>().sprite = m_equipSprite;
            Destroy(m_priceText);
        }
    }

    void Update()
    {
        //Disable the shop item button when the player does not have enough carrots
        m_itemButton.interactable = m_data.m_purchased || m_data.m_Price <= SaveSystem.m_data.m_carrots;

        //Disable the purchase message when the button is purchased or deselected
        if (m_data.m_purchased || EventSystem.current.currentSelectedGameObject != m_itemButton.gameObject)
            m_purchaseMessage.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        //Checks whether the player first selects the button, if so enable the purchase message
        if (!m_purchaseMessage.gameObject.activeSelf && !m_data.m_purchased)
        {
            m_purchaseMessage.gameObject.SetActive(true);
            return;
        }

        //If the purchase message is open, then purchase the item
        if (m_purchaseMessage.gameObject.activeSelf && !m_data.m_purchased)
        {
            SaveSystem.m_data.m_carrots -= m_data.m_Price;
            m_data.m_purchased = true;
            GetComponent<Image>().sprite = m_equipSprite;
            Destroy(m_priceText);

            //Save Data
            SaveSystem.m_data.AddPurchasedItem(m_data.name);
        }

        //Use the item
        if (m_data.m_purchased) m_data.OnClick();
    }
}

public abstract class ShopItemData : ScriptableObject
{
    [SerializeField] string[] m_previousNames;
    [SerializeField] uint m_price; //The price of the shop item
    public bool m_purchased; //Whether the item has been purchased

    public uint m_Price { get { return m_price; } }

    public virtual void Init() { }
    public abstract void OnClick();
    public virtual void Load()
    {
        //If save data contains previous name, then replace that name with the new name
        {
            bool replaceName = false;
            foreach (string previousName in m_previousNames)
            {
                if (m_data.m_purchasedItems.Contains(previousName))
                {
                    m_data.m_purchasedItems.Remove(previousName);
                    replaceName = true;
                }
            }

            if (replaceName) m_data.AddPurchasedItem(name);
        }
        

        //Load the data of whether the player previously purchased this shop item
        if (!m_purchased) m_purchased = m_data.m_purchasedItems.Contains(name);
    }
}