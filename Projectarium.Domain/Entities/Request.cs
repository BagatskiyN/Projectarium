using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public string Motivation { get; set; }
        public int? UserProfileId { get; set; } 
        public UserProfile UserProfile { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

    }
}
