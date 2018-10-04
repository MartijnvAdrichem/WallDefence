using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour {

    public static World world;

    public MoveableTile[,] worldTiles;
    public PathTileGraph tileGraph;
    public Tilemap tilemap;

    public int Height { get { return worldTiles.GetLength(1); }}
    public int Width { get { return worldTiles.GetLength(0); }}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public MoveableTile getTile(int x, int y) {
        if(worldTiles == null) return null;
        if (x < 0 || x >= worldTiles.GetLength(0) || y < 0 || y >= worldTiles.GetLength(1)) return null;
        return worldTiles[x, y];
    }

    public MoveableTile[] getNeighboursOfTile(MoveableTile tile) {
        MoveableTile[] tiles = new MoveableTile[8];
        tiles[0] = getTile(tile.x + 1, tile.y);
        tiles[1] = getTile(tile.x, tile.y + 1);
        tiles[2] = getTile(tile.x - 1, tile.y);
        tiles[3] = getTile(tile.x, tile.y - 1);
        tiles[4] = getTile(tile.x - 1, tile.y + 1);
        tiles[5] = getTile(tile.x + 1, tile.y + 1);
        tiles[6] = getTile(tile.x - 1, tile.y - 1);
        tiles[7] = getTile(tile.x + 1, tile.y - 1);

        return tiles;

    }
}
