namespace BoggleSolver;

public sealed class BoggleSolver
{
    /// <summary>
    /// Minimum word length per the rules of Boggle (https://en.wikipedia.org/wiki/Boggle)
    /// </summary>
    private static readonly int MIN_WORD_LENGTH = 3;

    /// <summary>
    /// Movements to try when checking cells for string path
    /// </summary>
    private static (int x, int y)[] MOVEMENT =
    [
        (-1, -1), (0, -1), (+1, -1),
        (-1, 0),           (+1, 0),
        (-1, +1), (0, +1), (+1, +1)
    ];

    private readonly Node _rootNode;

    public BoggleSolver(string[] validWords)
    {
        _rootNode = new Node().Build(validWords);
    }

    /// <summary>
    /// Given a current node in the Trie, board and board index, check all valid neighbor cells
    /// to see if they would continue building a known word from valid words
    /// </summary>
    private void Search(Node node, char[,] board, int x, int y, int width, HashSet<string> words, bool[,] visited)
    {
        // check to make sure we're not out of bounds and we havent been visited before
        if ((x >= 0) && (x < width) && (y >= 0) && (y < width) && (visited[x, y] == false))
        {
            if (node.Edges.TryGetValue(board[x, y], out var edge) == true)
            {
                if (edge.Value is not null)
                {
                    if (edge.Value.Length >= MIN_WORD_LENGTH)
                    {
                        words.Add(edge.Value);
                    }
                }

                visited[x, y] = true;

                for (int i = 0; i < MOVEMENT.Length; ++i)
                {
                    Search(edge, board, x + MOVEMENT[i].x, y + MOVEMENT[i].y, width, words, visited);
                }

                visited[x, y] = false;
            }
        }
    }

    public string[] SolveBoard(int width, int height, string letters)
    {
        char[,] board = new char[width, height];

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                board[x, y] = letters[y * width + x];
            }
        }

        HashSet<string> words = [];

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                bool[,] visited = new bool[width, height];

                if (_rootNode.Edges.ContainsKey(board[x, y]) == true)
                {
                    Search(_rootNode, board, x, y, width, words, visited);
                }
            }
        }

        return [.. words];
    }
}

/// <summary>
/// Class to represent the various chain of characters required to arrive
/// at a valid string provided to the Build method
/// </summary>
public sealed class Node(string? value = null)
{
    public Dictionary<char, Node> Edges { get; } = [];

    public string? Value { get; } = value;

    public Node Build(string[] words)
    {
        for (int i = 0; i < words.Length; ++i)
        {
            var node = this;

            for (int x = 0; x < words[i].Length; ++x)
            {
                if (node.Edges.TryGetValue(words[i][x], out var edge) == false)
                {
                    if (x == words[i].Length - 1)
                    {
                        node.Edges[words[i][x]] = new(words[i]);
                    }
                    else
                    {
                        node.Edges[words[i][x]] = new();
                    }
                }

                node = node.Edges[words[i][x]];
            }
        }

        return this;
    }
}
