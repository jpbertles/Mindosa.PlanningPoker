using Mindosa.PlanningPoker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindosa.PlanningPoker.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private static readonly Dictionary<string, Group> Groups = new Dictionary<string, Group>();
        private static readonly Random Random = new Random();
        private const int NumberOfDigits = 6;

        public string CreateGroup()
        {
            var groupName = GetRandomHexNumber(NumberOfDigits);
            while (Groups.ContainsKey(groupName))
            {
                groupName = GetRandomHexNumber(NumberOfDigits);
            }

            Groups[groupName] = initGroup(groupName);

            return groupName;
        }

        private Group initGroup(string groupName)
        {
            var group = new Group()
            {
                GroupName = groupName,
                GroupMembers = new List<GroupMember>(),
                Cards = new string[]
                        {
                            "1/2", "1", "2", "4", "6", "8", "10", "12", "16", "20", "24", "32", "40", "?"
                        }
            };

            return group;
        }

        public Group GetGroup(string groupName)
        {
            if (!Groups.ContainsKey(groupName))
            {
                Groups[groupName] = initGroup(groupName);
            }
            return Groups[groupName];
        }

        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            Random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + Random.Next(16).ToString("X");
        }

        public string AddUserToGroup(string groupName, string username, string connectionId)
        {
            var group = Groups[groupName];
            if (string.IsNullOrWhiteSpace(username))
            {
                int i = 1;
                while (group.GroupMembers.Any(x => x.Name == "User " + i))
                {
                    i++;
                }
                username = "User " + i;
            }
            group.GroupMembers.Add(new GroupMember() { ConnectionId = connectionId, Name = username });

            return username;
        }
        
        public Group LeaveGroup(string groupName, string connectionId)
        {
            var group = GetGroup(groupName);
            group.GroupMembers.RemoveAll(x => x.ConnectionId == connectionId);

            return group;
        }

        public Group LeaveGroup(string connectionId)
        {
            var group = Groups.Values.FirstOrDefault(x => x.GroupMembers.Any(y => y.ConnectionId == connectionId));

            group.GroupMembers.RemoveAll(x => x.ConnectionId == connectionId);

            return group;
        }

        public Group UpdateSettings(string groupName, string newUserName, bool isObserverOnly, string connectionId, string[] cards)
        {
            var group = GetGroup(groupName);

            if (!group.Cards.SequenceEqual(cards))
            {
                group.GroupMembers.ForEach(x => x.SelectedCard = null);
            }

            group.Cards = cards;

            var user = group.GroupMembers.Single(x => x.ConnectionId == connectionId);
            user.Name = newUserName;
            user.IsObserverOnly = isObserverOnly;
            if (user.IsObserverOnly)
            {
                user.SelectedCard = null;
            }

            return group;
        }

        public bool SelectCard(string groupName, string connectionId, string card)
        {
            var group = GetGroup(groupName);
            if (group.Cards.Contains(card))
            {
                var user = group.GroupMembers.FirstOrDefault(x => x.ConnectionId == connectionId);
                if (user != null)
                {
                    user.SelectedCard = card;
                    return true;
                }
            }

            return false;
        }
    }
}