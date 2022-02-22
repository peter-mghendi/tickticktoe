using static TickTickToe.Core.Row;
using static TickTickToe.Core.Column;
using static TickTickToe.Core.CellValue;

namespace TickTickToe.Core;

public record class Grid
{
    public const int Size = 3;

    public CellValue[][] Cells { get; init; } = DefaultCells;

    public static CellValue[][] DefaultCells =
    {
        new[] {Empty, Empty, Empty},
        new[] {Empty, Empty, Empty},
        new[] {Empty, Empty, Empty},
    };

    public Grid()
    {
    }

    public int CountBlanks()
    {
        var blanks = 0;

        foreach (var row in Cells)
        {
            foreach (var cell in row)
            {
                if (cell is Empty) ++blanks;
            }
        }

        return blanks;
    }

    public bool IsFull()
    {
        foreach (var row in Cells)
        {
            foreach (var cell in row)
            {
                if (cell is Empty) return false;
            }
        }

        return true;
    }

    public bool IsWon()
    {
        #region Values

        var topStart = Cells[(int) Top][(int) Start]; // 0
        var topCenter = Cells[(int) Top][(int) Center]; // 1
        var topEnd = Cells[(int) Top][(int) End]; // 2
        var middleStart = Cells[(int) Middle][(int) Start]; // 3
        var middleCenter = Cells[(int) Middle][(int) Center]; // 4
        var middleEnd = Cells[(int) Middle][(int) End]; // 5
        var bottomStart = Cells[(int) Bottom][(int) Start]; // 6
        var bottomCenter = Cells[(int) Bottom][(int) Center]; // 7
        var bottomEnd = Cells[(int) Bottom][(int) End]; // 8

        #endregion

        #region Horizontal Winning Condtions

        if (EqualAndNotEmpty(topStart, topCenter, topEnd)) return true; // Top row
        if (EqualAndNotEmpty(middleStart, middleCenter, middleEnd)) return true; // Middle row
        if (EqualAndNotEmpty(bottomStart, bottomCenter, bottomEnd)) return true; // Bottom row

        #endregion

        #region Vertical Winning Condtions

        if (EqualAndNotEmpty(topStart, middleStart, bottomStart)) return true; // Start column
        if (EqualAndNotEmpty(topCenter, middleCenter, bottomCenter)) return true; // Centre column
        if (EqualAndNotEmpty(topEnd, middleEnd, bottomEnd)) return true; // End column

        #endregion

        #region Diagonal Winning Conditions

        if (EqualAndNotEmpty(topStart, middleCenter, bottomEnd)) return true; // LTR diagonal
        if (EqualAndNotEmpty(topEnd, middleCenter, bottomStart)) return true; // RTL diagonal

        #endregion

        return false;
    }

    public void Reset() => Array.Clear(Cells);

    public CellValue Get(Row row, Column column) => Cells[(int) row][(int) column];

    public void Set(Row row, Column column, CellValue cellValue)
    {
        if (Cells[(int) row][(int) column] is not Empty)
        {
            throw new InvalidOperationException("You cannot overwrite a previously set value.");
        }

        Cells[(int) row][(int) column] = cellValue;
    }

    private static bool EqualAndNotEmpty(params CellValue[] values)
    {
        // Iterate
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] is Empty) return false;
            if (values[i] != values[i - 1]) return false;
        }

        // We have a winner
        return true;
    }
}