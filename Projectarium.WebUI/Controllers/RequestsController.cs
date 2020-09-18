using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "User")]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;

        }
        [HttpGet("Requests/Index/id")]
        public IActionResult Index(int id)
        {

            List<Vacancy> vacancies = _context.Vacancies.Include(x => x.Requests).Where(x => x.ProjectId == id).ToList();
            List<Request> requests = new List<Request>();
            foreach (var item in vacancies)
            {
                requests.AddRange(item.Requests);
            }
            return View(requests);
        }
        public async Task<IActionResult> Details(int id)
        {
            Request request = await _context.Requests
                .Include(x => x.Vacancy).ThenInclude(vacancy => vacancy.Project)
                                      .Include(x => x.UserProfile).ThenInclude(user => user.Skills)
                                      .Include(x => x.UserProfile).ThenInclude(user => user.Links)
                .FirstOrDefaultAsync(x => x.Id == id);
            return View(request);

        }

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            List<Request> requests = _context.Projects.Include(x => x.UserProfile)
                .Include(x => x.Vacancies)
                        .SelectMany(x => x.Vacancies
                                .SelectMany(x => x.Requests))
                                    .Include(x=>x.Vacancy)
                                       .Where(x => x.UserProfileId == userProfile.Id)
                .ToList();

            return View(requests);
        }

    }
}