using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class ApplicationUser:IdentityUser<int>
    {
        public virtual UserProfile UserProfile { get; set; }
    }
    public class ApplicationRole : IdentityRole<int>
    {
    }
}
