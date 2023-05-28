using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    /*
        Followed tutorial from : https://www.youtube.com/watch?v=otDHGmncBQY&list=LL&index=2
        for this script.
        This script allows the user to select a color from the color selector UI. It then sends 
        that information to the ColorPickerControl script that calculates the color the user
        just selected
    */
    [SerializeField]
    private Image cursorImage; //ui image of the cursor
    private RawImage SVImage; //The image that shows the colors
    private ColorPickerControl control; //The script that calculates the colors

    private RectTransform rectTransform, cursorTranform; //Tranform references
    
    private void Awake()
    {
        //Reset everything and get references
        SVImage = GetComponent<RawImage>();
        control = FindObjectOfType<ColorPickerControl>();
        rectTransform = GetComponent<RectTransform>();

        cursorTranform = cursorImage.GetComponent<RectTransform>();
        cursorTranform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    
    }
    private void UpdateColour(PointerEventData _eventData)
    {
        //Get the position of the cursor where the mouse dragged it to
        Vector3 pos = rectTransform.InverseTransformPoint(_eventData.position);
        
        //clamp its position to be within the rect transform
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
        //Change the color so that it doesn't match the background
        cursorImage.color = Color.HSVToRGB(0,0,1 - yNorm);
        //Set the saturation value of the color controller.
        control.SetSV(xNorm,yNorm);
    }
    public void OnDrag(PointerEventData _eventData) //When the mouse drags the cursor, update the color
    {
        UpdateColour(_eventData);
    }
    public void OnPointerClick(PointerEventData _eventData) //When the mouse clicks, update the color
    {
        UpdateColour(_eventData);
    }
}
