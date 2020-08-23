using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
   public class Vacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Skill> Skills { get;set; }
        public int Experience { get; set; }   
        public string Awards { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public List<SkillVacancy> SkillVacancies { get; set; }
        public Vacancy()
        {
            SkillVacancies = new List<SkillVacancy>();
            Skills = new List<Skill>();
        }
    }
}
