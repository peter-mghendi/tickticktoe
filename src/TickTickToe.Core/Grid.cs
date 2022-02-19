using System.Linq;
using static TickTickToe.Core.Row;
using static TickTickToe.Core.Column;
using static TickTickToe.Core.Value;

namespace TickTickToe.Core;

public record class Grid
{
    private const int Size = 3;
    private readonly Value?[,] _values = new Value?[Size, Size];

    public Grid() { }

    public int CountBlanks()
    {
        var blanks = 0;

        foreach (var value in _values)
        {
            if (value is null) ++blanks;
        }

        return blanks;
    }

    public bool IsFull()
    {
        foreach (var value in _values)
        {
            if (value is null) return false;
        }

        return true;
    }

    public bool IsWon()
    {
        #region Values
        var topStart     = _values[(int)Top, (int)Start];     // 0
        var topCenter    = _values[(int)Top, (int)Centre];    // 1
        var topEnd       = _values[(int)Top, (int)End];       // 2
        var middleStart  = _values[(int)Middle, (int)Start];  // 3
        var middleCenter = _values[(int)Middle, (int)Centre]; // 4
        var middleEnd    = _values[(int)Middle, (int)End];    // 5
        var bottomStart  = _values[(int)Bottom, (int)Start];  // 6
        var bottomCenter = _values[(int)Bottom, (int)Centre]; // 7
        var bottomEnd    = _values[(int)Bottom, (int)End];    // 8
        #endregion

        #region Horizontal Winning Condtions
        if (EqualAndNotNull(topStart, topCenter, topEnd)) return true;          // Top row
        if (EqualAndNotNull(middleStart, middleCenter, middleEnd)) return true; // Middle row
        if (EqualAndNotNull(bottomStart, bottomCenter, bottomEnd)) return true; // Bottom row
        #endregion

        #region Vertical Winning Condtions
        if (EqualAndNotNull(topStart, middleStart, bottomStart)) return true;    // Start column
        if (EqualAndNotNull(topCenter, middleCenter, bottomCenter)) return true; // Centre column
        if (EqualAndNotNull(topEnd, middleEnd, bottomEnd)) return true;          // End column
        #endregion

        #region Diagonal Winning Conditions
        if (EqualAndNotNull(topStart, middleCenter, bottomEnd)) return true; // LTR diagonal
        if (EqualAndNotNull(topEnd, middleCenter, bottomStart)) return true; // RTL diagonal
        #endregion

        return false;
    }

    public void Reset() => Array.Clear(_values);

    public Value? Get(Row row, Column column) => _values[(int)row, (int)column];
    
    public void Set(Row row, Column column, Value value)
    {
        if (_values[(int)row, (int)column] is not null)
        {
            throw new InvalidOperationException("You cannot overwrite a previously set value.");
        }
        _values[(int)row, (int)column] = value;
    }

    // TODO: Redo
    private static bool EqualAndNotNull(params Value?[] values)
    {
        // Early exit
        if (values.Length <= 1) return true;

        // Iterate
        for (int i = 1; i < values.Length; i++) {
            if (values[i] is null) return false;
            if (values[i - 1] is null) return false;
            if (values[i] != values[i - 1]) return false;
        }
            
        // No luck
        return true;
    }
}
