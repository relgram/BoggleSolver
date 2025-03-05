namespace BoggleSolver;

public sealed class BoggleSolver
{
    /// <summary>
    /// Minimum word length per the rules of Boggle (https://en.wikipedia.org/wiki/Boggle)
    /// </summary>
    private static readonly int MIN_WORD_LENGTH = 3;

    /// <summary>
    /// Dictionary of board sizes with subdictionary of board index and valid neighbor cells
    /// Since this data never changes, we can optimize via compiler
    /// </summary>
    private static readonly Dictionary<int, Dictionary<int, int[]>> NEIGHBORS = new()
    {
        {
            3 * 3, new()
            {
                { 0, [1, 3, 4] },
                { 1, [0, 2, 3, 4, 5] },
                { 2, [1, 4, 5] },
                { 3, [0, 1, 4, 6, 7] },
                { 4, [0, 1, 2, 3, 5, 6, 7, 8] },
                { 5, [1, 2, 4, 7, 8] },
                { 6, [3, 4, 7] },
                { 7, [3, 4, 5, 6, 8] },
                { 8, [4, 5, 7] },
            }
        },
        {
            4 * 4, new()
            {
                { 0, [1, 4, 5] },
                { 1, [0, 2, 4, 5, 6] },
                { 2, [1, 3, 5, 6, 7] },
                { 3, [2, 6, 7] },
                { 4, [0, 1, 5, 9, 8] },
                { 5, [0, 1, 2, 4, 6, 8, 9, 10] },
                { 6, [1, 2, 3, 5, 7, 9, 10, 11] },
                { 7, [2, 3, 6, 10, 11] },
                { 8, [4, 5, 9, 12, 13] },
                { 9, [4, 5, 6, 8, 10, 12, 13, 14] },
                { 10, [5, 6, 7, 9, 11, 13, 14, 15] },
                { 11, [6, 7, 10, 14, 15] },
                { 12, [8, 9, 13] },
                { 13, [8, 9, 10, 12, 14] },
                { 14, [9, 10, 11, 13, 15] },
                { 15, [10, 11, 14] },
            }
        },
        {
            5 * 5, new()
            {
                { 0, [1, 5, 6] },
                { 1, [0, 2, 5, 6, 7] },
                { 2, [1, 3, 6, 7, 8] },
                { 3, [2, 4, 7, 8, 9] },
                { 4, [3, 8, 9] },
                { 5, [0, 1, 6, 10, 11] },
                { 6, [0, 1, 2, 5, 7, 10, 11, 12] },
                { 7, [1, 2, 3, 6, 8, 11, 12, 13] },
                { 8, [2, 3, 4, 7, 9, 12, 13, 14] },
                { 9, [3, 4, 8, 13, 14] },
                { 10, [5, 6, 11, 15, 16] },
                { 11, [5, 6, 7, 10, 12, 15, 16, 17] },
                { 12, [6, 7, 8, 11, 13, 16, 17, 18] },
                { 13, [7, 8, 9, 12, 14, 17, 18, 19] },
                { 14, [8, 9, 13, 18, 19] },
                { 15, [10, 11, 16, 20, 21] },
                { 16, [10, 11, 12, 15, 17, 20, 21, 22] },
                { 17, [11, 12, 13, 16, 18, 21, 22, 23] },
                { 18, [12, 13, 14, 17, 19, 22, 23, 24] },
                { 19, [13, 14, 18, 23, 24] },
                { 20, [15, 16, 21] },
                { 21, [15, 16, 17, 20, 22] },
                { 22, [16, 17, 18, 21, 23] },
                { 23, [17, 18, 19, 22, 24] },
                { 24, [18, 19, 23] },
            }
        },
    };

    private readonly Node _rootNode;

    public BoggleSolver(string[] validWords)
    {
        _rootNode = new Node().Build(validWords);
    }

    /// <summary>
    /// Given a current node in the Trie, board and board index, check all valid neighbor cells
    /// to see if they would continue building a known word from valid words
    /// </summary>
    private void Search(Node node, char[] board, int index, HashSet<string> words, HashSet<int> visited)
    {
        // look at each valid neighbor according to current board index
        for (int i = 0; i < NEIGHBORS[board.Length][index].Length; ++i)
        {
            var idx = NEIGHBORS[board.Length][index][i];

            // ensure we're not looking at an index we've visited
            if (visited.Contains(idx) == false)
            {
                // check if the letter we're looking at is in the 'path' of a word
                if (node.Edges.TryGetValue(board[idx], out var edge) == true)
                {
                    if (edge.Value is not null)
                    {
                        if (edge.Value.Length >= MIN_WORD_LENGTH)
                        {
                            words.Add(edge.Value);
                        }
                    }

                    // recursively search this neighbor index with new set of visited for this path
                    Search(edge, board, idx, words, [.. visited, idx]);
                }
            }
        }
    }

    public string[] SolveBoard(int width, int height, string letters)
    {
        if (width != height)
        {
            throw new InvalidOperationException("Invalid board size");
        }

        if (width * height != letters.Length)
        {
            throw new InvalidOperationException("Insufficient letters provided");
        }

        HashSet<string> words = [];

        var board = letters.ToArray();

        for (int i = 0; i < letters.Length; ++i)
        {
            if (_rootNode.Edges.ContainsKey(letters[i]) == true)
            {
                Search(_rootNode.Edges[letters[i]], board, i, words, [i]);
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
