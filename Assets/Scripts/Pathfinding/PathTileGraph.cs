using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTileGraph {

    public Dictionary<MoveableTile, PathNode<MoveableTile>> nodes;


    public PathTileGraph(World world, int unitSize) {

        Debug.Log("Path_TileGraph");

        nodes = new Dictionary<MoveableTile, PathNode<MoveableTile>>();

        for (int x = 0; x < world.Width; x++) {
            for (int y = 0; y < world.Height; y++) {

                MoveableTile t = world.getTile(x, y);
                if (t.tilebase != null) {
                    PathNode<MoveableTile> n = new PathNode<MoveableTile>();
                    n.data = t;
                    nodes.Add(t, n);
                }

            }
        }

        Debug.Log("Path_TileGraph: Created " + nodes.Count + " nodes.");

        int edgeCount = 0;

        foreach (MoveableTile t in nodes.Keys) {
            PathNode<MoveableTile> n = nodes[t];

            List<PathEdge<MoveableTile>> edges = new List<PathEdge<MoveableTile>>();

            MoveableTile[] neighbours = world.getNeighboursOfTile(t); 

            for (int i = 0; i < neighbours.Length; i++) {
                if (neighbours[i] != null && neighbours[i].tilebase != null  && !IsClippingCorner(world, t, neighbours[i])) {
                    MoveableTile neighbour = neighbours[i];

                    bool makeEdge = true;

                    if (unitSize > 1) {
                        for (int x = neighbour.x - (unitSize / 2); x < neighbour.x + (unitSize / 2); x++) {
                            for (int y = neighbour.y - (unitSize / 2); y < neighbour.y + (unitSize / 2); y++) {
                                if(world.getTile(x, y) == null || world.getTile(x,y).tilebase == null) {
                                    makeEdge = false;
                                }
                            }
                        }
                    }
                    if (makeEdge) {
                        PathEdge<MoveableTile> e = new PathEdge<MoveableTile>();
                        e.cost = 1;
                        e.node = nodes[neighbours[i]];

                        edges.Add(e);
                   
                        edgeCount++;
                    }
                }
            }

            n.edges = edges.ToArray();
        }

        Debug.Log("Path_TileGraph: Created " + edgeCount + " edges.");

    }

    bool IsClippingCorner(World world, MoveableTile curr, MoveableTile neigh) {
        // If the movement from curr to neigh is diagonal (e.g. N-E)
        // Then check to make sure we aren't clipping (e.g. N and E are both walkable)

        int dX = curr.x - neigh.x;
        int dY = curr.y - neigh.y;

        if (Mathf.Abs(dX) + Mathf.Abs(dY) == 2) {
            // We are diagonal

            if (world.getTile(curr.x - dX, curr.y) == null  || world.getTile(curr.x - dX, curr.y).tilebase == null) {
                // East or West is unwalkable, therefore this would be a clipped movement.
                return true;
            }

            if (world.getTile(curr.x, curr.y - dY) == null || world.getTile(curr.x, curr.y - dY).tilebase == null) {
                // North or South is unwalkable, therefore this would be a clipped movement.
                return true;
            }

            // If we reach here, we are diagonal, but not clipping
        }

        // If we are here, we are either not clipping, or not diagonal
        return false;
    }

}
