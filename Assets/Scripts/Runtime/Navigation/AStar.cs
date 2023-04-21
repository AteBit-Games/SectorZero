using System;
using System.Collections.Generic;
using UnityEngine;
using PathNode = Runtime.Navigation.NavMap.PathNode;

namespace Runtime.Navigation
{
    public static class AStar
    {
        public static void CalculatePath(PathNode startNode, PathNode endNode, List<PathNode> allNodes, Action<Vector2[]> callback) 
        {
            var path = Internal_CalculatePath(startNode, endNode, allNodes);
            callback(path);
        }
        
        private static Vector2[] Internal_CalculatePath(PathNode startNode, PathNode endNode, List<PathNode> allNodes) 
        {
            var openList = new Heap<PathNode>(allNodes.Count);
            var closedList = new HashSet<PathNode>();
            var success = false;

            openList.Add(startNode);

            while ( openList.Count > 0 ) 
            {
                var currentNode = openList.RemoveFirst();
                if(currentNode == endNode ) 
                {
                    success = true;
                    break;
                }

                closedList.Add(currentNode);

                var linkIndices = currentNode.links;
                foreach (var t in linkIndices)
                {
                    var neighbour = allNodes[t];

                    if(closedList.Contains(neighbour)) continue;

                    var costToNeighbour = currentNode.gCost + ( currentNode.pos - neighbour.pos ).magnitude;
                    if(costToNeighbour < neighbour.gCost || !openList.Contains(neighbour)) 
                    {
                        neighbour.gCost = costToNeighbour;
                        neighbour.hCost = (neighbour.pos - endNode.pos).magnitude;
                        neighbour.parent = currentNode;

                        if(!openList.Contains(neighbour)) 
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            if(success) 
            {
                var path = new List<Vector2>();
                var currentNode = endNode;
                while(currentNode != startNode) 
                {
                    path.Add(currentNode.pos);
                    currentNode = currentNode.parent;
                }
                
                path.Add(startNode.pos);
                path.Reverse();
                return path.ToArray();
            }

            return null;
        }
    }
}
