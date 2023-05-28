using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SliderCounter : MonoBehaviour
{
    //Class the controls the slider counter and implements whatever value it represents
    public enum dataType //all the different data types the slider counter can represent
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
        voronoiOffsetY,
        
        //Room distribution
        Room_Count,
        Random_Room_Placement_Seed,
        Accretion_Min_Range,
        Accretion_Max_Range,
        Max_Partitions,
        //Room Shapes
        Room_Size,
        CanGenerateRoomsOverTerrain,
        CorridorSeed,
        RandomRoomSeed,
        AutomataSeed,

        //Room Details
        ErosionAmount,
        //Color
        ColorVarience
    }
    public enum roundDisplayType //How it converst the slider value
    {
        integer,
        one_decimal_place,
        two_decimal_place
    }
    public roundDisplayType roundingType = roundDisplayType.integer;
    public dataType sliderType;
    public float min = 0, max =1; //min max values
    public TextMeshProUGUI counter; //Text that shows the current value
    public void UpdateCounter()
    {
        float sliderVal = this.GetComponent<Slider>().value; //update the current value
        float val = Mathf.Lerp(min,max,sliderVal); //Clamp the value
        switch(roundingType) //Round the value 
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
        if(counter != null)
        {
            counter.text = val.ToString(); //update the counter
        }
        switch(sliderType) //Apply slider value to whatever other class requires it
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


            case dataType.Room_Count:
                RoomDistributor.instance.SetRoomCount(Mathf.RoundToInt(val));
                break;
            case dataType.Random_Room_Placement_Seed:
                RoomDistributor.instance.SetSeed(Mathf.RoundToInt(val));
                break;
            case dataType.Accretion_Min_Range:
                RoomDistributor.instance.SetAccretionMinRange(Mathf.RoundToInt(val));
                break;
            case dataType.Accretion_Max_Range:
                RoomDistributor.instance.SetAccretionMaxRange(Mathf.RoundToInt(val));
                break;
            case dataType.Max_Partitions:
                RoomDistributor.instance.SetMaxPartitions(Mathf.RoundToInt(val));
                break;
                
            
            case dataType.Room_Size:
                RoomShapeGenerator.instance.SetRoomSize(Mathf.RoundToInt(val));
                break;
            case dataType.CanGenerateRoomsOverTerrain:
                bool b = Mathf.RoundToInt(val) > 0 ? true : false;
                RoomShapeGenerator.instance.SetRoomsCanGenerateOverTerrain(b);
                break;
            case dataType.CorridorSeed:
                RoomDetailsGenerator.instance.SetCorridorSeed(Mathf.RoundToInt(val));
                break;
            case dataType.RandomRoomSeed:
                RoomShapeGenerator.instance.SetRandomSeed(Mathf.RoundToInt(val));
                break;
            case dataType.AutomataSeed:
                RoomShapeGenerator.instance.SetAutomataSeed(Mathf.RoundToInt(val));
                break;


            case dataType.ErosionAmount:
                RoomDetailsGenerator.instance.SetErosionValue(val);
                break;


            case dataType.ColorVarience:
                TileColorSetter.instance.SetTileColorVarience(val);
                break;
        }
    }
}
