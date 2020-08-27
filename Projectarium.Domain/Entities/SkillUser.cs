using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
   public class SkillUser
    {
        public int? UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
