using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
   public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<SkillVacancy> SkillVacancies { get; set; }
        public List<SkillUser> SkillUsers { get; set; }
        public Skill()
        {
            SkillUsers = new List<SkillUser>();
            SkillVacancies = new List<SkillVacancy>();
        }
    }
}
