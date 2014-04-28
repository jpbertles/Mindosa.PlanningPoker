using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Mindosa.PlanningPoker.Infrastructure.Models
{
    public class GroupMember
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string ConnectionId { get; set; }
        public string SelectedCard { get; set; }
        public bool IsObserverOnly { get; set; }
    }
}