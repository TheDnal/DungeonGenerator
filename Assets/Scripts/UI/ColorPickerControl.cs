using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    /*
        Followed tutorial from : https://www.youtube.com/watch?v=otDHGmncBQY&list=LL&index=2
        for this script.
        This script calculates the color that the user clicks on in the UI, using the SVImage control script
    */
    public float currentHue,currentSat,currentVal; //Different HSV values for the color
    [SerializeField]
    private RawImage hueImage, satValImage, outputImage; //Different image references for the color picker UI
    [SerializeField]
    private Slider hueSlider; //Hue slider reference
    [SerializeField]
    private TMP_InputField hexInputField; //Input field for inputting hex values of colors
    
    private Texture2D hueTexture,svTexture,outputTexture; //Cached textures
    public static ColorPickerControl instance; //Singletong instance
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }
    private void Start()
    {
        //Restart all methods (resets all the textures)
        CreateHueImage();

        CreateSVImage();

        CreateOuputImage();

        UpdateOutputImage();
    }
    private void CreateHueImage()
    {
        //Creates hue image on the UI
        hueTexture = new Texture2D(1,16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for(int i =0 ; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0,i,Color.HSVToRGB((float)i / hueTexture.height, 1, .8f));

        }

        hueTexture.Apply();
        currentHue = 0;

        hueImage.texture = hueTexture;
    }
    private void CreateSVImage()
    {
        //Creates saturation value image for the UI.
        svTexture = new Texture2D(16,16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x,y,Color.HSVToRGB(
                                    currentHue,
                                    (float)x / svTexture.width,
                                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;
    }

    private void CreateOuputImage()
    {
        //Creates the output image for the UI.
        outputTexture = new Texture2D(1,16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColor = Color.HSVToRGB(currentHue,currentSat,currentVal);

        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0,i,currentColor);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    private void UpdateOutputImage()
    {
        //Updates the Output image for the UI
        Color currentColor = Color.HSVToRGB(currentHue,currentSat,currentVal);

        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0,i,currentColor);
        }
        
        outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColor);

        //Do stuff
    }

    public void SetSV(float _S, float _V)
    {
        //Sets the Saturation value for the UI
        currentSat = _S;
        currentVal = _V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        //Updates the Saturation value for the UI
        currentHue = hueSlider.value;

        for(int y =0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x,y,Color.HSVToRGB(
                                    currentHue,
                                    (float)x / svTexture.width,
                                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }

    public void OnTextInput()
    {
        //Applys the hexadecimal color code into the class
        if(hexInputField.text.Length < 6){return;}

        Color newCol;

        if(ColorUtility.TryParseHtmlString("#" + hexInputField.text, out newCol))
            Color.RGBToHSV(newCol, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;

        hexInputField.text = "";

        UpdateOutputImage();
    }
    public Color GetColor()
    {
        //Get the color that was picked
        return Color.HSVToRGB(currentHue,currentSat,currentVal);
    }
}
