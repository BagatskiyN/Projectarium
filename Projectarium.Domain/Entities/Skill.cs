using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
   public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int? UserProfileId { get; set; }
       public UserProfile UserProfile { get; set; }

        public int? VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
        public Skill()
        {
          
        }
    }
}
