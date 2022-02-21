using System.Text;
using TickTickToe.Core;
using static TickTickToe.Core.CellValue;

namespace TickTickToe.Cli.Extensions;

public static class CellValueExtensions
{
    public static char AsText(this CellValue value) => value switch
    {
        Cross => 'X',
        Nought => 'O',
        Empty or _ => ' '
    };
}