using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAStar {

    Stack<MoveableTile> path;

    public PathAStar(World world, MoveableTile tileStart, MoveableTile tileEnd) {

        // if tileEnd is null, then we are simply scanning for the nearest objectType.
        // We can do this by ignoring the heuristic component of AStar, which basically
        // just turns this into an over-engineered Dijkstra's algo

        // Check to see if we have a valid tile graph
        if (world.tileGraph == null) {
            world.tileGraph = new PathTileGraph(world, 1);
        }

        // A dictionary of all valid, walkable nodes.
        Dictionary<MoveableTile, PathNode<MoveableTile>> nodes = world.tileGraph.nodes;

        // Make sure our start/end tiles are in the list of nodes!
        if (nodes.ContainsKey(tileStart) == false) {
            Debug.LogError("Path_AStar: The starting tile isn't in the list of nodes!" + tileStart.x + "  "  + tileStart.y);

            return;
        }


        PathNode<MoveableTile> start = nodes[tileStart];

           if (nodes.ContainsKey(tileEnd) == false) {
                Debug.LogError("Path_AStar: The ending tile isn't in the list of nodes!");
                return;
            }
        // if tileEnd is null, then we are simply looking for an inventory object
        // so just set goal to null.
        PathNode<MoveableTile> goal = nodes[tileEnd];

 


        // Mostly following this pseusocode:
        // https://en.wikipedia.org/wiki/A*_search_algorithm

        List<PathNode<MoveableTile>> ClosedSet = new List<PathNode<MoveableTile>>();

        /*		List<PathNode<MoveableTile>> OpenSet = new List<PathNode<MoveableTile>>();
                OpenSet.Add( start );
        */

        SimplePriorityQueue<PathNode<MoveableTile>> OpenSet = new SimplePriorityQueue<PathNode<MoveableTile>>();
        OpenSet.Enqueue(start, 0);

        Dictionary<PathNode<MoveableTile>, PathNode<MoveableTile>> Came_From = new Dictionary<PathNode<MoveableTile>, PathNode<MoveableTile>>();

        Dictionary<PathNode<MoveableTile>, float> g_score = new Dictionary<PathNode<MoveableTile>, float>();
        foreach (PathNode<MoveableTile> n in nodes.Values) {
            g_score[n] = Mathf.Infinity;
        }
        g_score[start] = 0;

        Dictionary<PathNode<MoveableTile>, float> f_score = new Dictionary<PathNode<MoveableTile>, float>();
        foreach (PathNode<MoveableTile> n in nodes.Values) {
            f_score[n] = Mathf.Infinity;
        }
        f_score[start] = heuristic_cost_estimate(start, goal);

        while (OpenSet.Count > 0) {
            PathNode<MoveableTile> current = OpenSet.Dequeue();

            if (current == goal) {
                // We have reached our goal!
                // Let's convert this into an actual sequene of
                // tiles to walk on, then end this constructor function!
                reconstruct_path(Came_From, current);
                return;
            }

            ClosedSet.Add(current);

            foreach (PathEdge<MoveableTile> edge_neighbor in current.edges) {
                PathNode<MoveableTile> neighbor = edge_neighbor.node;

                if (ClosedSet.Contains(neighbor) == true)
                    continue; // ignore this already completed neighbor

                float movement_cost_to_neighbor = 1 * dist_between(current, neighbor);

                float tentative_g_score = g_score[current] + movement_cost_to_neighbor;

                if (OpenSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor])
                    continue;

                Came_From[neighbor] = current;
                g_score[neighbor] = tentative_g_score;
                f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);

                if (OpenSet.Contains(neighbor) == false) {
                    OpenSet.Enqueue(neighbor, f_score[neighbor]);
                } else {
                    OpenSet.UpdatePriority(neighbor, f_score[neighbor]);
                }

            } // foreach neighbour
        } // while

        // If we reached here, it means that we've burned through the entire
        // OpenSet without ever reaching a point where current == goal.
        // This happens when there is no path from start to goal
        // (so there's a wall or missing floor or something).

        // We don't have a failure state, maybe? It's just that the
        // path list will be null.
    }

    float heuristic_cost_estimate(PathNode<MoveableTile> a, PathNode<MoveableTile> b) {
        if (b == null) {
            // We have no fixed destination (i.e. probably looking for an inventory item)
            // so just return 0 for the cost estimate (i.e. all directions as just as good)
            return 0f;
        }

        return Mathf.Sqrt(
            Mathf.Pow(a.data.x - b.data.x, 2) +
            Mathf.Pow(a.data.y - b.data.y, 2)
        );

    }

    float dist_between(PathNode<MoveableTile> a, PathNode<MoveableTile> b) {
        // We can make assumptions because we know we're working
        // on a grid at this point.

        // Hori/Vert neighbours have a distance of 1
        if (Mathf.Abs(a.data.x - b.data.x) + Mathf.Abs(a.data.y - b.data.y) == 1) {
            return 1f;
        }

        // Diag neighbours have a distance of 1.41421356237	
        if (Mathf.Abs(a.data.x - b.data.x) == 1 && Mathf.Abs(a.data.y - b.data.y) == 1) {
            return 1.41421356237f;
        }

        // Otherwise, do the actual math.
        return Mathf.Sqrt(
            Mathf.Pow(a.data.x - b.data.x, 2) +
            Mathf.Pow(a.data.y - b.data.y, 2)
        );

    }

    void reconstruct_path(
        Dictionary<PathNode<MoveableTile>, PathNode<MoveableTile>> Came_From,
        PathNode<MoveableTile> current
    ) {
        // So at this point, current IS the goal.
        // So what we want to do is walk backwards through the Came_From
        // map, until we reach the "end" of that map...which will be
        // our starting node!
        Stack<MoveableTile> total_path = new Stack<MoveableTile>();
        total_path.Push(current.data); // This "final" step is the path is the goal!

        while (Came_From.ContainsKey(current)) {
            // Came_From is a map, where the
            //    key => value relation is real saying
            //    some_node => we_got_there_from_this_node
           // Debug.Log(current.data.x);
            current = Came_From[current];
            total_path.Push(current.data);
        }

        path = total_path;
        total_path.Pop();
        // At this point, total_path is a queue that is running
        // backwards from the END tile to the START tile, so let's reverse it.


    }

    public MoveableTile Dequeue() {
        if (path == null) {
            Debug.LogError("Attempting to dequeue from an null path.");
            return null;
        }
        if (path.Count <= 0) {
            Debug.LogError("what???");
            return null;
        }
        return path.Pop();
    }

    public int Length() {
        if (path == null)
            return 0;

        return path.Count;
    }


}
