using static TickTickToe.Core.Row;
using static TickTickToe.Core.Column;
using static TickTickToe.Core.CellValue;

namespace TickTickToe.Core;

public record class Grid
{
    private const int Size = 3;
    private readonly CellValue[,] _cells = new CellValue[Size, Size];

    #region Manually setting values

    public CellValue TopStart
    {
        get => _cells[(int) Top, (int) Start];
        set => _cells[(int) Top, (int) Start] = value;
    }

    public CellValue TopCenter
    {
        get => _cells[(int) Top, (int) Center];
        set => _cells[(int) Top, (int) Center] = value;
    }

    public CellValue TopEnd
    {
        get => _cells[(int) Top, (int) End];
        set => _cells[(int) Top, (int) End] = value;
    }

    public CellValue MiddleStart
    {
        get => _cells[(int) Middle, (int) Start];
        set => _cells[(int) Middle, (int) Start] = value;
    }

    public CellValue MiddleCenter
    {
        get => _cells[(int) Middle, (int) Center];
        set => _cells[(int) Middle, (int) Center] = value;
    }

    public CellValue MiddleEnd
    {
        get => _cells[(int) Middle, (int) End];
        set => _cells[(int) Middle, (int) End] = value;
    }

    public CellValue BottomStart
    {
        get => _cells[(int) Bottom, (int) Start];
        set => _cells[(int) Bottom, (int) Start] = value;
    }

    public CellValue BottomCenter
    {
        get => _cells[(int) Bottom, (int) Center];
        set => _cells[(int) Bottom, (int) Center] = value;
    }

    public CellValue BottomEnd
    {
        get => _cells[(int) Bottom, (int) End];
        set => _cells[(int) Bottom, (int) End] = value;
    }

    #endregion


    public Grid()
    {
    }

    public int CountBlanks()
    {
        var blanks = 0;

        foreach (var cell in _cells)
        {
            if (cell is Empty) ++blanks;
        }

        return blanks;
    }

    public bool IsFull()
    {
        foreach (var cell in _cells)
        {
            if (cell is Empty) return false;
        }

        return true;
    }

    public bool IsWon()
    {
        #region Horizontal Winning Condtions

        if (EqualAndNotNull(TopStart, TopCenter, TopEnd)) return true; // Top row
        if (EqualAndNotNull(MiddleStart, MiddleCenter, MiddleEnd)) return true; // Middle row
        if (EqualAndNotNull(BottomStart, BottomCenter, BottomEnd)) return true; // Bottom row

        #endregion

        #region Vertical Winning Condtions

        if (EqualAndNotNull(TopStart, MiddleStart, BottomStart)) return true; // Start column
        if (EqualAndNotNull(TopCenter, MiddleCenter, BottomCenter)) return true; // Centre column
        if (EqualAndNotNull(TopEnd, MiddleEnd, BottomEnd)) return true; // End column

        #endregion

        #region Diagonal Winning Conditions

        if (EqualAndNotNull(TopStart, MiddleCenter, BottomEnd)) return true; // LTR diagonal
        if (EqualAndNotNull(TopEnd, MiddleCenter, BottomStart)) return true; // RTL diagonal

        #endregion

        return false;
    }

    public void Reset() => Array.Clear(_cells);

    public CellValue Get(Row row, Column column) => _cells[(int) row, (int) column];

    public void Set(Row row, Column column, CellValue cellValue)
    {
        if (_cells[(int) row, (int) column] is not Empty)
        {
            throw new InvalidOperationException("You cannot overwrite a previously set value.");
        }

        _cells[(int) row, (int) column] = cellValue;
    }

    // TODO: Redo
    private static bool EqualAndNotNull(params CellValue[] values)
    {
        // Early exit
        if (values.Length <= 1) return true;

        // Iterate
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] is Empty) return false;
            if (values[i - 1] is Empty) return false;
            if (values[i] != values[i - 1]) return false;
        }

        // No luck
        return true;
    }
}