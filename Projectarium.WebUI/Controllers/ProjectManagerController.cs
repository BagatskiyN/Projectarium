using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;
using Projectarium.WebUI.Models.AdminUsersVM;
using Projectarium.WebUI.Models.ProjectManagerVM;
using Projectarium.WebUI.Services;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "User")]
    public class ProjectManagerController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        ILinkMasker _linkMasker;
        public ProjectManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILinkMasker linkMasker)
        {
            _userManager = userManager;
            _context = context;
            _linkMasker = linkMasker;

        }

        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = this.User;


            int id = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<Project> projects = _context.Projects
                                             .Where(x => x.UserProfileId == id)
                                             .ToList();
            return View(projects);
        }
        public IActionResult CreatePartial()
        {
            return PartialView("~/Views/Shared/ProjectManagerPartialViews/CreateModalView.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string ProjectTitle)
        {
            ClaimsPrincipal currentUser = this.User;

            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (userProfile == null)
            {
                return NotFound();
            }
            Project project = new Project() { Title = ProjectTitle, UserProfile = userProfile };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        public async Task SaveProjectDescription([FromBody] SaveDescription saveDescription)
        {

            Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == saveDescription.Id);
            project.AboutProject = saveDescription.Description;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

        }
        [HttpPost]
        public async Task SaveVacancyDescription([FromBody] SaveDescription saveDescription)
        {
            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == saveDescription.Id);

            vacancy.Awards = saveDescription.Description;
            _context.Vacancies.Update(vacancy);
            await _context.SaveChangesAsync();

        }
        [HttpPost]
        public async Task<IActionResult> DeleteVacancy([FromBody] int id)
        {
            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }
            _context.Vacancies.Remove(vacancy);
            _context.SaveChanges();
            return Ok();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Project project = _context.Projects
                .Include(x => x.Vacancies)
                .ThenInclude(vacancy => vacancy.Skills)
                .Include(x=>x.Links)
                .FirstOrDefault(x => x.Id == id);

            return View(project);
        }

        [HttpPost]
        public IActionResult AddVacancy([FromBody]NewVacancy newVacancy)
        {
            if (newVacancy.VacancyTitle != null)
            {
                Project project = _context.Projects.FirstOrDefault(x => x.Id == newVacancy.ProjectId);
                if (project == null)
                {
                    return NotFound();
                }
                Vacancy vacancy = new Vacancy() { Title = newVacancy.VacancyTitle, Project = project };


                _context.Vacancies.Add(vacancy);
                _context.SaveChanges();
                return PartialView("~/Views/Shared/ProjectManagerPartialViews/EditPartialViews/EditVacancyView.cshtml", vacancy);
            }
            return new EmptyResult();


        }
        [HttpPost]
        public async Task<IActionResult> AddVacancySkill([FromBody] NewSkill newSkill)
        {
            Vacancy vacancy = _context.Vacancies.FirstOrDefault(x => x.Id == newSkill.id);
            if (vacancy == null)
            {
                return NotFound();
            }
            Skill skill = new Skill() { Title = newSkill.text, Vacancy = vacancy };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return PartialView("~/Views/Shared/ProjectManagerPartialViews/EditPartialViews/EditSkillView.cshtml", skill);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSkill([FromBody] int id)
        {
            Skill skill = _context.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return Ok();

        }


        [HttpPost]
        public async Task<IActionResult> AddLink([FromBody]NewLink IncomingLink)
        {
            Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == IncomingLink.ProjectId);
            if (IncomingLink == null || IncomingLink.Link == null || project == null)
            {
                return NotFound();
            }
            Uri uri = new Uri(IncomingLink.Link);
            string LinkMask = _linkMasker.MaskLink(uri);
            Link link = new Link() { Mask = LinkMask, LinkText = IncomingLink.Link, Project = project };
            await _context.Links.AddAsync(link);
            await _context.SaveChangesAsync();
            return PartialView("~/Views/Shared/ProjectManagerPartialViews/EditPartialViews/EditLinkView.cshtml", link);
        }

        [HttpPost]

        public async Task<IActionResult> DeleteLink([FromBody] int id)
        {
            Link link = await _context.Links.FirstOrDefaultAsync(x => x.Id == id);
            if(link==null)
            {
                return NotFound();
            }
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();
            return Ok();

        }
        public IActionResult PreviewProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Project project = _context.Projects
                .Include(x => x.Vacancies)
                .ThenInclude(vacancy => vacancy.Skills)
                .Include(x => x.Links)
                .FirstOrDefault(x => x.Id == id);

            return View(project);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: ProjectManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }

}

