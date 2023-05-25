using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    public float currentHue,currentSat,currentVal;
    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;
    [SerializeField]
    private Slider hueSlider;
    [SerializeField]
    private TMP_InputField hexInputField;
    
    private Texture2D hueTexture,svTexture,outputTexture;
    public static ColorPickerControl instance;
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
        CreateHueImage();

        CreateSVImage();

        CreateOuputImage();

        UpdateOutputImage();
    }
    private void CreateHueImage()
    {
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
        currentSat = _S;
        currentVal = _V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
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
        return Color.HSVToRGB(currentHue,currentSat,currentVal);
    }
}
