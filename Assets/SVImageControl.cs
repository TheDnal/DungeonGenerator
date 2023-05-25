using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Image cursorImage;
    private RawImage SVImage;
    private ColorPickerControl control;

    private RectTransform rectTransform, cursorTranform;
    
    private void Awake()
    {
        SVImage = GetComponent<RawImage>();
        control = FindObjectOfType<ColorPickerControl>();
        rectTransform = GetComponent<RectTransform>();

        cursorTranform = cursorImage.GetComponent<RectTransform>();
        cursorTranform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    
    }
    private void UpdateColour(PointerEventData _eventData)
    {
        Vector3 pos = rectTransform.InverseTransformPoint(_eventData.position);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        if(pos.x < -deltaX)
        {
            pos.x = -deltaX;
        }
        else if(pos.x > deltaX)
        {
            pos.x = deltaX;
        }

        if(pos.y < -deltaY)
        {
            pos.y = -deltaY;
        }
        else if(pos.y > deltaY)
        {
            pos.y = deltaY;
        }

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / rectTransform.sizeDelta.x;
        float yNorm = y / rectTransform.sizeDelta.y;

        cursorTranform.localPosition = pos;
        cursorImage.color = Color.HSVToRGB(0,0,1 - yNorm);

        control.SetSV(xNorm,yNorm);
    }
    public void OnDrag(PointerEventData _eventData)
    {
        UpdateColour(_eventData);
    }
    public void OnPointerClick(PointerEventData _eventData)
    {
        UpdateColour(_eventData);
    }
}
