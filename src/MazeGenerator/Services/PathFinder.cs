using MazeGenerator.Models;

namespace MazeGenerator.Services;

public class PathFinder
{
    public List<(int x, int y)> FindPath(MazeSession session, int[,] maze)
    {
        var startCell = (1, 0);
        var endCell = (session.Size - 2, session.Size - 1);

        return this.AStarSearch(maze, startCell, endCell, session.Size);
    }

    private List<(int x, int y)> AStarSearch(int[,] maze, (int x, int y) start, (int x, int y) end, int size)
    {
        var openSet = new PriorityQueue<Node, int>();
        var closedSet = new HashSet<(int x, int y)>();
        var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();
        var gScore = new Dictionary<(int x, int y), int>();
        var fScore = new Dictionary<(int x, int y), int>();

        gScore[start] = 0;
        fScore[start] = this.Heuristic(start, end);
        openSet.Enqueue(new Node(start.x, start.y, fScore[start]), fScore[start]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            var currentPos = (current.X, current.Y);

            if (currentPos.Equals(end))
            {
                return this.ReconstructPath(cameFrom, currentPos);
            }

            closedSet.Add(currentPos);

            foreach (var neighbor in this.GetNeighbors(currentPos, maze, size))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                var tentativeGScore = gScore[currentPos] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentPos;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + this.Heuristic(neighbor, end);

                    if (!openSet.UnorderedItems.Any(x => x.Element.X == neighbor.x && x.Element.Y == neighbor.y))
                    {
                        openSet.Enqueue(new Node(neighbor.x, neighbor.y, fScore[neighbor]), fScore[neighbor]);
                    }
                }
            }
        }

        return new List<(int x, int y)>();
    }

    private List<(int x, int y)> ReconstructPath(Dictionary<(int x, int y), (int x, int y)> cameFrom, (int x, int y) current)
    {
        var path = new List<(int x, int y)> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private List<(int x, int y)> GetNeighbors((int x, int y) pos, int[,] maze, int size)
    {
        var neighbors = new List<(int x, int y)>();
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };

        for (var i = 0; i < 4; i++)
        {
            var nx = pos.x + dx[i];
            var ny = pos.y + dy[i];

            if (nx >= 0 && nx < size && ny >= 0 && ny < size && maze[nx, ny] == 0)
            {
                neighbors.Add((nx, ny));
            }
        }

        return neighbors;
    }

    private int Heuristic((int x, int y) a, (int x, int y) b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    private class Node
    {
        public int X { get; }
        public int Y { get; }
        public int FScore { get; }

        public Node(int x, int y, int fScore)
        {
            this.X = x;
            this.Y = y;
            this.FScore = fScore;
        }
    }
}