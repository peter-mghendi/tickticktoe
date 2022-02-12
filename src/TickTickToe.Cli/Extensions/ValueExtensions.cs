using System.Text;
using TickTickToe.Core;
using static TickTickToe.Core.Value;

namespace TickTickToe.Cli.Extensions;

public static class ValueExtensions
{
    public static char AsText(this Value? value) => value switch
    {
        Cross => 'X',
        Nought => 'O',
        _ => ' '
    };
}