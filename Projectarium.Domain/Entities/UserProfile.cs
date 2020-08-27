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
        public List<SkillUser> SkillUsers { get; set; }
        public UserProfile()
        {
            SkillUsers = new List<SkillUser>();
            Links = new List<Link>();
            Projects = new List<Project>();
        }
    }
}
