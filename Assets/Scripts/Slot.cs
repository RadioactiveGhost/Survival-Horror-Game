using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Slot : MonoBehaviour
{

    private Stack<Item> items;

    public Text stackText;

    public Sprite slotEmpty;
    public Sprite slotHighlight;

    public bool IsEmpty
    {
        get { return items.Count == 0; }
    }

    public bool isAvailable
    {
        get { return CurrentItem.maxSize > items.Count; }
    }

    public Item CurrentItem
    {
        get { return items.Peek(); }
    }
    // Start is called before the first frame update
    void Start()
    {
        items = new Stack<Item>();
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform textRect = stackText.GetComponent<RectTransform>();

        int textScale = (int)(slotRect.sizeDelta.x * 0.80);
        stackText.resizeTextMaxSize = textScale;
        stackText.resizeTextMinSize = textScale;

        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(Item item)
    {
        items.Push(item);

        if (items.Count > 1)
        {
            stackText.text = items.Count.ToString();
        }

        ChangeSprite(item.spriteNeutral, item.spriteHighlight);
    }

    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;
        SpriteState st = new SpriteState();

        st.highlightedSprite = highlight;

        st.pressedSprite = neutral;

        GetComponent<Button>().spriteState = st;
    }
}
