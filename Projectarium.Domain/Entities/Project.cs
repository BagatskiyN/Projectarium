using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name = "Название проекта")]
        public string Title { get; set; }
      
        public string AboutProject { get; set; }
     
        public List<Vacancy> Vacancies { get; set; }
        public List<Link> Links { get; set; }
    
        public int? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public Project()
        {
          
            Vacancies = new List<Vacancy>();
            Links = new List<Link>();
      
        }

    }
}
