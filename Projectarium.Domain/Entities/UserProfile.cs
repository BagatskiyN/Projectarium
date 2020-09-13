using System;
using System.Collections.Generic;
using System.Text;


namespace Projectarium.Domain.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string AboutUser { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public List<Request> Requests { get; set; }
     
        public List<Link> Links { get; set; }
        public List<Project> Projects { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public List<Skill> Skills { get; set; }
        public UserProfile()
        {
            Skills = new List<Skill>();
            Links = new List<Link>();
            Projects = new List<Project>();
        }
    }
}
