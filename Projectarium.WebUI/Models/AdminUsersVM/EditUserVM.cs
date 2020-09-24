using Microsoft.AspNetCore.Http;
using Projectarium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Models.AdminUsersVM
{
    public class EditUserVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string AboutUser { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Link> Links { get; set; }
        public List<Project> Projects { get; set; }
        public  byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public IFormFile FormFile { get; set; }

    }
}
