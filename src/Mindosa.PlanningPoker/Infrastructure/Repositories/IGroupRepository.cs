using Mindosa.PlanningPoker.Infrastructure.Models;

namespace Mindosa.PlanningPoker.Infrastructure.Repositories
{
    public interface IGroupRepository
    {
        string CreateGroup();
        Group GetGroup(string groupName);
        string AddUserToGroup(string groupName, string username, string connectionId);
        Group LeaveGroup(string groupName, string connectionId);
        Group LeaveGroup(string connectionId);
        Group UpdateSettings(string groupName, string newUserName, bool isObserverOnly, string connectionId, string[] cards);
        bool SelectCard(string groupName, string connectionId, string card);
    }
}