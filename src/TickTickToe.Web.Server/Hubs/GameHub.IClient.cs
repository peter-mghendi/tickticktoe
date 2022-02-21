using TickTickToe.Core;
using TickTickToe.Core.Models;
using TickTickToe.Web.Server.Models;

namespace TickTickToe.Web.Server.Hubs;

public partial class GameHub
{
    public interface IGameClient
    {
        public Task AddAsPlayerOne(Guid gameId);
        public Task AddAsPlayerTwo(string playerOneId, Guid gameId);
        public Task AddPlayerTwo(string userId);
        public Task ReceiveSystemMessage(SystemMessage message);
        public Task SetGridValue(Row row, Column column, CellValue value);
    }
}