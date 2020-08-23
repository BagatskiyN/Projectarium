using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class SkillVacancy
    {
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}
