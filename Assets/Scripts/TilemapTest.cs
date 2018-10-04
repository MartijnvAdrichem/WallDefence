using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTest : MonoBehaviour {

    public Tilemap tilemap;
    public World world;

	// Use this for initialization
	void OnEnable () {
         world = new World();
        //BoundsInt bounds = new BoundsInt(0, 0, 0, tilemap.cellBounds.xMax, tilemap.cellBounds.yMax, 0);
        TileBase[] tileBase = tilemap.GetTilesBlock(tilemap.cellBounds);
        MoveableTile[,] moveableTiles = new MoveableTile[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];
       
            for (int x = 0; x < tilemap.cellBounds.size.x; x++) {
                 for (int y = 0; y < tilemap.cellBounds.size.y; y++) {
                moveableTiles[x, y] = new MoveableTile(tileBase[x + y * tilemap.cellBounds.size.x], x, y);
            }
        }

        world.worldTiles = moveableTiles;
        World.world = world;
        world.tilemap = tilemap;
        world.tileGraph = new PathTileGraph(world, 1);
        if (world.tileGraph == null) {
            Debug.Log(" tilegraph is null");
            return;
        }
        foreach (PathNode<MoveableTile> node in world.tileGraph.nodes.Values) {

            foreach (PathEdge<MoveableTile> edge in node.edges) {
                Debug.DrawLine(new Vector3(node.data.x / 4.0f, node.data.y / 4.0f, 0), new Vector3(edge.node.data.x / 4.0f, edge.node.data.y / 4.0f, 0), Color.red, 50000);
            }
            Debug.DrawLine(new Vector3(node.data.x / 4.0f, node.data.y / 4.0f, 0), new Vector3((node.data.x / 4.0f) + 0.01f, (node.data.y / 4.0f) + 0.05f, 0), Color.yellow, 50000);

        }
    }

    private void OnMouseDown() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(Input.mousePosition);
    }

    // Update is called once per frame
    void Update () {
        
	}
}
