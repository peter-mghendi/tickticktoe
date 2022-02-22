using MudBlazor;
using TickTickToe.Core;
using static TickTickToe.Core.CellValue;

namespace TickTickToe.Web.Client.Extensions;

public static class CellValueExtensions
{
    public static string AsIcon(this CellValue value) => value switch
    {
        Cross => Icons.Outlined.Clear,
        Nought => Icons.Outlined.Circle,
        Empty or _ => string.Empty
    };
}