using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class Link
    {
        public int Id { get; set; }
        public string Mask { get; set; }
        public string LinkText { get; set; }
        public int? UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
