using UnityEngine;

namespace JumpeeIsland
{
    using UnityEngine;
    using System.Collections.Generic;

    public class AStar
    {
        private Dictionary<Vector3, Node> grid; // Use a dictionary to store nodes by position

        public void InitializeGrid(List<MovableTile> tiles)
        {
            grid = new Dictionary<Vector3, Node>();

            // Determine the size of your grid based on your game world
            // For simplicity, let's assume the grid consists of nodes at integer positions

            // Create the grid nodes
            for (int x = -5; x <= 4; x++)
            {
                for (int z = -5; z <= 4; z++)
                {
                    Vector3 nodePosition = new Vector3(x, 0, z);
                    bool isWalkable = tiles.Find(t => Vector3.Distance(t.GetPosition(), nodePosition) < 0.1f);

                    grid[nodePosition] = new Node(nodePosition, isWalkable);
                }
            }
        }

        public void UpdateObstacle()
        {
            foreach (var node in grid)
            {
                node.Value.isWalkable = GameFlowManager.Instance.GetEnvManager().FreeToMove(node.Value.position);
            }
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = new Node(startPos,true);
            Node targetNode = NodeFromVector3(targetPos);

            if (targetNode == null)
                return null;
            
            // Debug.Log($"Start node: {startPos} ,Target node: {targetNode.position}");

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    // Path found, reconstruct and return it
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // No path found
            return null;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        private List<Node> GetNeighbors(Node node)
        {
            Debug.Log($"Neighbor node: {node}");
            List<Node> neighbors = new List<Node>();

            // Define the possible neighbor positions (assuming 4-directional movement)
            Vector3[] neighborPositions = new Vector3[]
            {
                new Vector3(0, 0, 1), // Up
                new Vector3(1, 0, 0), // Right
                new Vector3(0, 0, -1), // Down
                new Vector3(-1, 0, 0) // Left
            };

            foreach (Vector3 offset in neighborPositions)
            {
                Vector3 neighborPosition = node.position + offset;

                if (grid.ContainsKey(neighborPosition))
                {
                    neighbors.Add(grid[neighborPosition]);
                }
            }

            return neighbors;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(Mathf.RoundToInt(nodeA.position.x) - Mathf.RoundToInt(nodeB.position.x));
            int dstZ = Mathf.Abs(Mathf.RoundToInt(nodeA.position.z) - Mathf.RoundToInt(nodeB.position.z));

            return dstX + dstZ;
        }

        private Node NodeFromVector3(Vector3 vector3Position)
        {
            // Round the Vector3 position to the nearest integer coordinates
            Vector3 roundedPosition =
                new Vector3(Mathf.RoundToInt(vector3Position.x), 0, Mathf.RoundToInt(vector3Position.z));

            // Check if the node exists in the grid
            if (grid.ContainsKey(roundedPosition))
            {
                return grid[roundedPosition];
            }
            else
            {
                // Handle cases where the Vector3 position is not in the grid
                return null;
            }
        }
    }

    public class Node
    {
        public Vector3 position; // World position
        public bool isWalkable;
        public int gCost;
        public int hCost;
        public Node parent;

        public int fCost => gCost + hCost;

        public Node(Vector3 position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }
    }
}