using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;
using Projectarium.WebUI.Models.AdminUsersVM;
using Projectarium.WebUI.Models.ProjectManagerVM;
using Projectarium.WebUI.Services;

using Projectarium.WebUI.Models;
using Projectarium.WebUI.Models.HomeVM;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Projectarium.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Project> projects = _context.Projects.ToList();
            return View(projects);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //public async Task<IActionResult> ShowProjectPage(int id)
        //{
        
        //    Project project = _context.Projects
        //        .Include(x => x.Vacancies)
        //        .ThenInclude(vacancy => vacancy.Skills)
        //        .Include(x => x.Links)
        //        .FirstOrDefault(x => x.Id == id);

        //    return View("~/Views/ProjectManager/PreviewProject.cshtml", project);
        //}
        public async Task<IActionResult> CreateRequest(int id)
        {
            ClaimsPrincipal currentUser = this.User;

            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == id);
            Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == vacancy.ProjectId);
            if (project.UserProfileId == userProfile.Id)
            {
                return Ok();
            }
            else
            {
                return PartialView("~/Views/Shared/HomePartialViews/CreateRequestModalView.cshtml", vacancy);
            }


        }
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody]NewRequest newRequest)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));

            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x=>x.Id==newRequest.VacancyId);

            Request request = new Request() { Vacancy = vacancy, UserProfile = userProfile, Motivation = newRequest.Motivation };
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
            return Ok();

        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
