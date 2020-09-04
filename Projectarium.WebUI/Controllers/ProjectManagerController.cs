using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;


namespace Projectarium.WebUI.Controllers
{
    public class ProjectManagerController : Controller
    {
   
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;

        }

        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = this.User;
       
       
            int id = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<Project> projects = _context.Projects
                                             .Where(x => x.UserProfileId ==id)    
                                             .ToList();
            return View(projects);
        }
      public IActionResult CreatePartial()
        {
            return PartialView("~/Views/Shared/ProjectManagerPartialViews/CreateModalView.cshtml");
        }

     [HttpPost]
        public IActionResult Create([FromBody] string ProjectTitle)
        {
            ClaimsPrincipal currentUser = this.User;
     
            UserProfile userProfile=_context.UserProfiles.FirstOrDefault(m=>m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            if(userProfile==null)
            {
                return NotFound();
            }
            Project project = new Project() { Title=ProjectTitle,UserProfile=userProfile};
            _context.Projects.Add(project);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //
        //public IActionResult Create(Project project)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        if (TempData.ContainsKey("Links"))
        //        {
        //            if(TempData["Links"]!=null)
        //            {
        //             project.Links = TempData["Links"] as List<Link>;
        //            }
        //        }
        //        if (TempData.ContainsKey("Vacancies"))
        //        {
        //            if (TempData["Vacancies"] != null)
        //            {
        //                project.Vacancies= TempData["Vacancies"] as List<Vacancy>;
        //            }
        //        }
        //        project.UserProfileId = _userManager.FindByNameAsync(User.Identity.Name).Id;
        //        _context.Projects.Add(project);
        //    }
        //    return RedirectToAction("Index");

        //}

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Project project = _context.Projects.FirstOrDefault(x=>x.Id==id);
            return View(project);
        }

            [HttpPost]
        public IActionResult AddVacancy([FromBody]string title)
        {
            if (title != null)
            {
                List<Vacancy> vacancies1= TempData["Vacancies"] as List<Vacancy>;
                Vacancy vacancy = new Vacancy() { Title = title };
               
                List<Vacancy> vacancies = new List<Vacancy>();
                Vacancy vacancyView = new Vacancy() { Title = title };
                
                vacancies.Add(vacancy);
                TempData["Vacancy"] = vacancies;
                return PartialView("~/Views/Shared/ProjectManagerPartialViews/VacancyView.cshtml",vacancyView);
            }
            return new EmptyResult();


        }

    }
}
