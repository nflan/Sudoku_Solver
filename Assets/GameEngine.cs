using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    [SerializeField] private List<Square> m_AllSquares = new List<Square>(); // All Squares added manually in Unity Editor

    [SerializeField] private Square[,] m_Grid = new Square[9, 9]; // 9x9 grid of Squares

    void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        foreach (Square square in GameObject.FindObjectsOfType<Square>())
        {
            m_AllSquares.Add(square);
        }

        if (m_AllSquares.Count != 81)
        {
            Debug.LogError("The total number of squares must be exactly 81!");
            return;
        }

        // Sort the squares based on their positions in the scene
        m_AllSquares.Sort((a, b) =>
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;

            // Compare row first (y-axis descending), then column (x-axis ascending)
            if (Mathf.Abs(posA.y - posB.y) > 0.01f) // Allow for small floating-point differences
                return posB.y.CompareTo(posA.y); // Higher y comes first (row order)
            else
                return posA.x.CompareTo(posB.x); // Lower x comes first (column order)
        });

        // Assign squares to the 2D grid
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int index = row * 9 + col;
                m_Grid[row, col] = m_AllSquares[index];
            }
        }
    }

    // Get all squares in a specific row
    public List<Square> GetRow(int rowIndex)
    {
        List<Square> row = new List<Square>();
        for (int col = 0; col < 9; col++)
        {
            row.Add(m_Grid[rowIndex, col]);
        }
        return row;
    }

    // Get all squares in a specific column
    public List<Square> GetColumn(int columnIndex)
    {
        List<Square> column = new List<Square>();
        for (int row = 0; row < 9; row++)
        {
            column.Add(m_Grid[row, columnIndex]);
        }
        return column;
    }

    // Get all squares in a specific block (3x3)
    public List<Square> GetBlock(int blockIndex)
    {
        List<Square> block = new List<Square>();
        int startRow = (blockIndex / 3) * 3;
        int startCol = (blockIndex % 3) * 3;

        for (int row = startRow; row < startRow + 3; row++)
        {
            for (int col = startCol; col < startCol + 3; col++)
            {
                block.Add(m_Grid[row, col]);
            }
        }
        return block;
    }
}
