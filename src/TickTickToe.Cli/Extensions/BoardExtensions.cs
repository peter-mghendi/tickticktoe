using System.Text;
using TickTickToe.Core;

namespace TickTickToe.Cli.Extensions;

public static class BoardExtensions
{
    private const string HorizontalDividers = "     |     |      \n";
    private const string VerticalDividers = "_____|_____|_____ \n";

    public static string Render(this Board board)
    {
        var boardBuilder = new StringBuilder();

        var values = new char[9];
        var current = 0;

        foreach(var row in Enum.GetValues<Row>())
        {
            foreach(var column in Enum.GetValues<Column>())
            {
                values[current++] = board.Get(row, column).AsText();
            }
        }

        boardBuilder.Append(HorizontalDividers);
        boardBuilder.Append(BuildRow(values[0..3]));
        boardBuilder.Append(VerticalDividers);
        boardBuilder.Append(HorizontalDividers);
        boardBuilder.Append(BuildRow(values[3..6]));
        boardBuilder.Append(VerticalDividers);
        boardBuilder.Append(HorizontalDividers);
        boardBuilder.Append(BuildRow(values[6..9]));
        boardBuilder.Append(HorizontalDividers);

        return boardBuilder.ToString();
    }

    private static string BuildRow(char[] values) => $"  {values[0]}  |  {values[1]}  |  {values[2]}   \n";   
}