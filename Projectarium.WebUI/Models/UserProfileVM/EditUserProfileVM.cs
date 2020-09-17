using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Models.UserProfileVM
{
    public class EditUserProfileVM
    {
        public string Name { get; set; }
        public string AboutUser { get; set; }

        public IFormFile Image { get; set; }

        public EditUserProfileVM()
        {

        }

    }
}
