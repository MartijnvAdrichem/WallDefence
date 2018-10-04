using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour {

    public GameObject go;
    MoveableTile destTile;  // If we aren't moving, then destTile = currTile
    MoveableTile nextTile;  // The next tile in the pathfinding sequence
    PathAStar pathAStar;
    float speed = 5f;
    float movementPercentage;

    public MoveableTile currTile
    {
        get; protected set;
    }

    public float X {
        get{
            return Mathf.Lerp(currTile.x, nextTile.x, movementPercentage);
        }
    }

    public float Y {
        get{
            return Mathf.Lerp(currTile.y, nextTile.y, movementPercentage);
        }
    }

    void Update_DoMovement(float deltaTime) {
        if (currTile == destTile) {
            pathAStar = null;
            return; // We're already were we want to be.
        }

        if (nextTile == null || nextTile == currTile) {
            // Get the next tile from the pathfinder.
            if (pathAStar == null || pathAStar.Length() == 0) {
                // Generate a path to our destination
                pathAStar = new PathAStar(World.world, currTile, destTile); // This will calculate a path from curr to dest.
                if (pathAStar.Length() == 0) {
                    Debug.LogError("Path_AStar returned no path to destination!");
                    pathAStar = null;
                    return;
                }
            }

            // Grab the next waypoint from the pathing system!
            nextTile = pathAStar.Dequeue();

            if (nextTile == currTile) {
                Debug.LogError("Update_DoMovement - nextTile is currTile?");
            }
        }

        float distToTravel = Mathf.Sqrt(
            Mathf.Pow(currTile.x - nextTile.x, 2) +
            Mathf.Pow(currTile.y - nextTile.y, 2)
        );


        // How much distance can be travel this Update?
        float distThisFrame = speed * deltaTime;

        // How much is that in terms of percentage to our destination?
        float percThisFrame = distThisFrame / distToTravel;

        // Add that to overall percentage travelled.
        movementPercentage += percThisFrame;

        if (movementPercentage >= 1) {
            // We have reached our destination

            // TODO: Get the next tile from the pathfinding system.
            //       If there are no more tiles, then we have TRULY
            //       reached our destination.

            currTile = nextTile;
            movementPercentage = 0;
            // FIXME?  Do we actually want to retain any overshot movement?
        }


    }

    // Use this for initialization
    void Start () {
        currTile = World.world.getTile(5, 5);
        destTile = nextTile = currTile;
    }



        // Update is called once per frame
        void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 poss = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveableTile tile = World.world.getTile((int)(poss.x * 4), (int)(poss.y * 4));
            if (tile != null) {
                destTile = tile;
                pathAStar = null;
            }
        }

        Update_DoMovement(Time.deltaTime);
        //Vector3 pos = World.world.tilemap.CellToWorld(new Vector3Int((int)(X / 4), (int)(Y / 4), 0));
        go.transform.position = new Vector3((X / 4), (Y / 4), 0);

    }
}
