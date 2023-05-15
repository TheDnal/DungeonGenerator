using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SliderCounter : MonoBehaviour
{
    public enum dataType
    {
        //GridData
        gridWidth,
        gridHeight,

        //Terrain Data
        perlinSeed,
        perlinScale,
        perlinThreshold,
        perlinOffsetX,
        perlineOffsetY,
        //Rand data
        randThreshold,
        randSeed,
        //Voronoi data
        voronoiSeed,
        voronoiRadius,
        voronoiDensity,
        voronoiOffsetX,
        voronoiOffsetY
    }
    public enum roundDisplayType
    {
        integer,
        one_decimal_place,
        two_decimal_place
    }
    public roundDisplayType roundingType = roundDisplayType.integer;
    public dataType sliderType;
    public float min = 0, max =1;
    public TextMeshProUGUI counter;
    public void UpdateCounter()
    {
        float sliderVal = this.GetComponent<Slider>().value;
        float val = Mathf.Lerp(min,max,sliderVal);
        switch(roundingType)
        {
            case roundDisplayType.integer:
                val = Mathf.Round(val);
                break;
            case roundDisplayType.one_decimal_place:
                val = Mathf.Round(val * 10) / 10;
                break;
            case roundDisplayType.two_decimal_place:
                val = Mathf.Round(val * 100) / 100;
                break;
        }
        counter.text = val.ToString();
        switch(sliderType)
        {

            case dataType.gridWidth:
                GridGenerator.instance.SetWidth(Mathf.RoundToInt(val));
                break;
            case dataType.gridHeight:
                GridGenerator.instance.SetHeight(Mathf.RoundToInt(val));
                break;


            case dataType.perlinSeed:
                GridTerrainGenerator.instance.SetPerlinSeed(Mathf.RoundToInt(val));
                break;    
            case dataType.perlinScale:
                GridTerrainGenerator.instance.SetPerlinScale(val);
                break;
            case dataType.perlinThreshold:
                GridTerrainGenerator.instance.SetPerlinThreshold(val);
                break;
            case dataType.perlinOffsetX:
                GridTerrainGenerator.instance.SetPerlinOffsetX(val);
                break;
            case dataType.perlineOffsetY:
                GridTerrainGenerator.instance.SetPerlinOffsetY(val);
                break;


            case dataType.randThreshold:
                GridTerrainGenerator.instance.SetRandThreshold(val);
                break;
            case dataType.randSeed:
                GridTerrainGenerator.instance.SetRandSeed(Mathf.RoundToInt(val));
                break;
                
            case dataType.voronoiSeed:
                GridTerrainGenerator.instance.SetVoronoiSeed(Mathf.RoundToInt(val));
                break;
            case dataType.voronoiRadius:
                GridTerrainGenerator.instance.SetVoronoiRadius(Mathf.RoundToInt(val));
                break;
            case dataType.voronoiDensity:
                GridTerrainGenerator.instance.SetVoronoiDensity(Mathf.RoundToInt(val));
                break;
            case dataType.voronoiOffsetX:
                GridTerrainGenerator.instance.SetVoronoiOffsetX(Mathf.RoundToInt(val));
                break;
            case dataType.voronoiOffsetY:
                GridTerrainGenerator.instance.SetVoronoiOffsetY(Mathf.RoundToInt(val));
                break;
        }
    }
}
