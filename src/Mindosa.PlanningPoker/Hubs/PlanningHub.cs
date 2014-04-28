using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Mindosa.PlanningPoker.Infrastructure.Models;
using Mindosa.PlanningPoker.Infrastructure.Repositories;

namespace Mindosa.PlanningPoker.Hubs
{
    public class PlanningHub : Hub
    {
        private IGroupRepository _groupRepository;

        public PlanningHub(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<string> JoinGroup(string groupName, string username)
        {
            var group = _groupRepository.GetGroup(groupName);

            var user = group.GroupMembers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (user == null)
            {
                username = _groupRepository.AddUserToGroup(groupName, username, Context.ConnectionId);

                await Groups.Add(Context.ConnectionId, groupName);
                Clients.Group(groupName).reset(group);

                return username;
            }
            else
            {
                return user.Name;
            }
        }

        public async Task UpdateSettings(string groupName, string username, bool isObserverOnly, string[] cards)
        {
            var group = _groupRepository.UpdateSettings(groupName, username, isObserverOnly, Context.ConnectionId, cards);

            Clients.Group(groupName).reset(group);
        }
        
        public async Task LeaveGroup(string groupName)
        {
            var group = _groupRepository.LeaveGroup(groupName, Context.ConnectionId);

            await Groups.Remove(Context.ConnectionId, groupName);
            Clients.Group(groupName).reset(group);
        }

        public bool SelectCard(string groupName, string username, string card)
        {
            var success = _groupRepository.SelectCard(groupName, Context.ConnectionId, card);

            if (success)
            {
                Clients.Group(groupName).cardSelected(username, card);
            }
            return success;
        }

        public void Clear(string groupName)
        {
            Clients.Group(groupName).clear();
        }

        //public void UpdateCards(string groupName, string[] cards)
        //{
        //    _groupRepository.UpdateCards(groupName, cards);
        //    Clients.Group(groupName).updateCards(cards);
        //}

        public override Task OnDisconnected()
        {
            var group = _groupRepository.LeaveGroup(Context.ConnectionId);

            Clients.Group(group.GroupName).reset(group);        
            Groups.Remove(Context.ConnectionId, group.GroupName);
            

            return base.OnDisconnected();
        }
    }
}