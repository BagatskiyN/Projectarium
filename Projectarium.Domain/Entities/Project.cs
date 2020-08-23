using System;
using System.Collections.Generic;
using System.Text;

namespace Projectarium.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Likes { get; set; }
        public string AboutProject { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Vacancy> Vacancies { get; set; }
        public List<Link> Links { get; set; }
        public Project()
        {
            Skills = new List<Skill>();
            Vacancies = new List<Vacancy>();
            Links = new List<Link>();
        }

    }
}
