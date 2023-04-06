using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    List<Tile> tiles;
    public Room(List<Tile> _tiles)
    {
        tiles = _tiles;
    }
}
