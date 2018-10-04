using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class MoveableTile {

    public TileBase tilebase;
    public int x;
    public int y;

    
    public MoveableTile(TileBase tilebase, int x, int y) {
        this.tilebase = tilebase;
        this.x = x;
        this.y = y;
    }


}
