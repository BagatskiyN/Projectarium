using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "UserId")]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;

        }

        public IActionResult Index(int id)
        {
            List<Request> requests = _context.Requests.Include(x => x.UserProfile).Include(x => x.Vacancy).ToList();
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
    }
}