using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Models.AdminUsersVM
{
    public class CreateUserVM
    {
        public string Email { get; set; }
        public string AboutUser { get; set; }
        public IFormFile FormFile { get; set; }
      
    }
}
