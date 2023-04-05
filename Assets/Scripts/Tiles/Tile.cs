using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    #region Variables
    private Vector2Int coords;
    private GameObject tileObject;
    private bool active = false;
    #endregion
    #region Constructor/Destructor
    public Tile(Vector2Int _coords, GameObject _tileObject, bool _active = false)
    {
        coords = _coords;
        tileObject = _tileObject;
        active = _active;
    }
    public void DenInitialise()
    {
        GameObject.Destroy(tileObject);
    }
    public void SetColor(Color _col)
    {
        if(tileObject == null){return;}
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        tileObject.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_Color", _col);
        tileObject.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    #endregion
    public void UpdateRenderer()
    {
        if(tileObject == null){return;}
        tileObject.GetComponent<Renderer>().enabled = active;
    }
    #region Setters
    public void SetActive(bool _active)
    {
        active = _active;
    }
    #endregion
    #region Getters
    public bool GetActive() {return active;}
    
    #endregion

}