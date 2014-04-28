using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindosa.PlanningPoker.Infrastructure.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<GroupMember> GroupMembers { get; set; }
        public string[] Cards { get; set; }
    }
}