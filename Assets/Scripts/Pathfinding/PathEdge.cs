using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEdge<T> {

    public float cost;  // Cost to traverse this edge (i.e. cost to ENTER the tile)

    public PathNode<T> node;
}
