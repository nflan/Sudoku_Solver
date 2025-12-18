using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    [Header("GameEngine Information")]
    [SerializeField] private List<Square> allSquares = new List<Square>(); // All Squares added manually in Unity Editor

    [SerializeField] private Square[,] grid = new Square[9, 9]; // 9x9 grid of Squares
    [SerializeField] private bool isOpti = false;

    [Header("~ GameEngine Tools ~")]
    [SerializeField] private GameObject solvingPanel;
    [SerializeField] private TMPro.TextMeshProUGUI solvingText;
    [SerializeField] private GameObject errorText;
    [SerializeField] private GameObject confettiFx;
    public int StepsPerFrame = 500;

    private bool solved = false;
    private bool solving = false;
    private int stepCounter;



    void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        foreach (Square square in GameObject.FindObjectsOfType<Square>())
        {
            this.allSquares.Add(square);
        }

        if (this.allSquares.Count != 81)
        {
            Debug.LogError("The total number of squares must be exactly 81!");
            return;
        }

        // Sort the squares based on their positions in the scene
        this.allSquares.Sort((a, b) =>
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;

            if (Mathf.Abs(posA.y - posB.y) > 0.01f)
                return posB.y.CompareTo(posA.y);
            else
                return posA.x.CompareTo(posB.x);
        });

        // Assign squares to the 2D grid
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int index = row * 9 + col;
                this.allSquares[index].SetIndex(index);
                this.allSquares[index].gameObject.name = "Square_" + index;
                this.grid[row, col] = this.allSquares[index];
            }
        }
    }

    // Get all squares in a specific row
    public List<Square> GetRow(int rowIndex)
    {
        List<Square> row = new List<Square>();
        for (int col = 0; col < 9; col++)
        {
            row.Add(this.grid[rowIndex, col]);
        }
        return row;
    }

    // Get all squares in a specific column
    public List<Square> GetColumn(int columnIndex)
    {
        List<Square> column = new List<Square>();
        for (int row = 0; row < 9; row++)
        {
            column.Add(this.grid[row, columnIndex]);
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
                block.Add(this.grid[row, col]);
            }
        }
        return block;
    }
    
    public int GetRowIndex(int index)
    {
        return index / 9;
    }

    public int GetColumnIndex(int index)
    {
        return index % 9;
    }

    public int GetBlockIndex(int index)
    {
        // Calculate the row and column of the square
        int row = index / 9; // Integer division to get the row
        int col = index % 9; // Modulo to get the column

        // Calculate the block index
        int blockRow = row / 3; // Determine which 3-row group the square is in
        int blockCol = col / 3; // Determine which 3-column group the square is in

        return blockRow * 3 + blockCol; // Combine to get the block index
    }

    public void SetIsOpti(bool isOpti)
    {
        this.isOpti = isOpti;
    }

    private void ShowSolvingUI()
    {
        this.solvingPanel.SetActive(true);
    }

    public void HideErrorPanel()
    {
        this.errorText.SetActive(false);
    }

    public void HideSolvingUI()
    {
        this.solvingPanel.SetActive(false);
    }

    private IEnumerator SolvingAnimation()
    {
        string baseText = "Solving";
        int dots = 0;

        while (this.solving)
        {
            dots = (dots + 1) % 4;
            this.solvingText.text = baseText + new string('.', dots);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void SolveSudoku()
    {
        if (!AllFilled())
        {
            StartCoroutine(SolveWrapper());
        } else
        {
            ShowError("This Sudoku is already done", Color.white);
        }
    }

    private IEnumerator SolveWrapper()
    {
        this.solved = false;
        this.solving = true;
        this.stepCounter = 0;

        if (!PreCheckImpossible())
        {
            ShowError("This Sudoku is impossible (PreCheck)", Color.red);
            yield break;
        }

        ShowSolvingUI();
        Coroutine anim = StartCoroutine(SolvingAnimation());

        if (!this.isOpti)
        {
            yield return StartCoroutine(SolveBasicCoroutine(0));
        }
        else
        {
            yield return StartCoroutine(SolveMRVCoroutine());
        }

        this.solving = false;

        StopCoroutine(anim);
        HideSolvingUI();

        if (!this.solved)
        {
            ShowError("This Sudoku is impossible", Color.red);
            yield break ;
        }

        GameObject particleFx = Instantiate(this.confettiFx, this.transform.parent);
        Destroy(particleFx, 3.5f);
    }


    private void ShowError(string message, Color color)
    {
        if (!this.errorText.activeSelf)
        {
            this.errorText.GetComponent<TMP_Text>().text = message;
            this.errorText.GetComponent<TMP_Text>().color = color;
            this.errorText.SetActive(true);
        }
        this.errorText.GetComponent<FadeInSeconds>().Fade();
    }

    // Fill next cell in order
    private IEnumerator SolveBasicCoroutine(int index)
    {
        this.stepCounter++;
        if (this.stepCounter % this.StepsPerFrame == 0)
        {
            yield return null;
        }

        if (index == this.allSquares.Count)
        {
            this.solved = true;
            yield break;
        }

        if (this.allSquares[index].GetIsManuallySet())
        {
            yield return SolveBasicCoroutine(index + 1);
            yield break;
        }

        for (int num = 1; num <= 9; num++)
        {
            if (IsValid(index, num))
            {
                this.allSquares[index].SetValue(num.ToString());

                yield return SolveBasicCoroutine(index + 1);
                if (solved)
                {
                    yield break;
                }

                this.allSquares[index].ResetSquare();
            }
        }
    }

    private bool PreCheckImpossible()
    {
        return CheckForcedBlockContradictions();
    }

    IEnumerable<int> GetBlockCells(int block)
    {
        int startRow = (block / 3) * 3;
        int startCol = (block % 3) * 3;

        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                yield return r * 9 + c;
            }   
        }
    }


    public bool CheckForcedBlockContradictions()
    {
        for (int block = 0; block < 9; block++)
        {
            // Numbers already present in block
            HashSet<int> present = new();

            foreach (int idx in GetBlockCells(block))
            {
                if (!string.IsNullOrEmpty(this.allSquares[idx].GetValue()))
                {
                    present.Add(int.Parse(this.allSquares[idx].GetValue()));
                }
            }

            // Check remaining numbers
            for (int num = 1; num <= 9; num++)
            {
                if (present.Contains(num))
                {
                    continue;
                }

                int candidateCount = 0;
                int lastCandidate = -1;

                foreach (int idx in GetBlockCells(block))
                {
                    if (!string.IsNullOrEmpty(this.allSquares[idx].GetValue()))
                    {
                        continue;
                    }

                    if (IsValid(idx, num))
                    {
                        candidateCount++;
                        lastCandidate = idx;

                        if (candidateCount > 1)
                        {
                            break; // no forced placement
                        }
                    }
                }

                // No place → impossible
                if (candidateCount == 0)
                {
                    return false;
                }

                // Forced but blocked → impossible
                if (candidateCount == 1)
                {
                    if (!string.IsNullOrEmpty(this.allSquares[lastCandidate].GetValue()))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    private bool AllFilled()
    {
        for (int i = 0; i < this.allSquares.Count; i++)
        {
            if (string.IsNullOrEmpty(this.allSquares[i].GetValue()))
            {
                return false;
            }
        }
        return true;
    }

    //Fill the cell with the fewest valid candidates
    private IEnumerator SolveMRVCoroutine()
    {
        this.stepCounter++;
        if (this.stepCounter % this.StepsPerFrame == 0)
        {
            yield return null;
        }

        int index = FindMRVCell();

        if (index == -1)
        {
            if (AllFilled())
            {
                this.solved = true;
            }
            yield break;
        }

        for (int num = 1; num <= 9; num++)
        {
            if (IsValid(index, num))
            {
                this.allSquares[index].SetValue(num.ToString());

                yield return StartCoroutine(SolveMRVCoroutine());
                if (this.solved)
                {
                    yield break;
                }

                this.allSquares[index].ResetSquare();
            }
        }
    }

    private int FindMRVCell()
    {
        int bestIndex = -1;
        int bestCount = int.MaxValue;

        for (int i = 0; i < this.allSquares.Count; i++)
        {
            if (this.allSquares[i].GetIsManuallySet() || this.allSquares[i].GetValue() != "")
            {
                continue;
            }

            int count = 0;
            for (int num = 1; num <= 9; num++)
            {
                if (IsValid(i, num))
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return i;
            }

            if (count < bestCount)
            {
                bestCount = count;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    private bool IsValid(int index, int num)
    {
        return !CheckRow(index, num)
            && !CheckColumn(index, num)
            && !CheckBlock(index, num);
    }


    public void ResetSudokuGrid()
    {
        foreach (Square square in this.grid)
        {
            square.ResetSquare();
            square.SetIsManuallySet(false);
        }
    }

    private bool CheckBlock(int index, int value)
    {
        foreach (Square block in GetBlock(GetBlockIndex(index)))
        {
            if (value.ToString() == block.GetValue())
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckColumn(int index, int value)
    {
        foreach (Square column in GetColumn(GetColumnIndex(index)))
        {
            if (value.ToString() == column.GetValue())
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckRow(int index, int value)
    {
        foreach (Square row in GetRow(GetRowIndex(index)))
        {
            if (value.ToString() == row.GetValue())
            {
                return true;
            }
        }
        return false;
    }
}
