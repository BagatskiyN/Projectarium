using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
   public class Vacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Experience { get; set; }   
        public string Awards { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public List<Skill> Skills { get; set; }

        public List<Request> Requests { get; set; }
        public Vacancy()
        {
            Skills = new List<Skill>();
            Requests = new List<Request>();
            
        }
    }
}
