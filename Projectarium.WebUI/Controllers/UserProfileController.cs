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
using Projectarium.WebUI.Services;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "UserId")]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        ILinkMasker _linkMasker;
        public UserProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILinkMasker linkMasker)
        {
            _userManager = userManager;
            _context = context;
            _linkMasker = linkMasker;

        }
        public async Task<IActionResult> Index(int id)
        {
            UserProfile userProfile = await _context.UserProfiles.Include(x=>x.Skills).Include(x=>x.Links).FirstOrDefaultAsync(x => x.Id == id);
            return View(userProfile);
        }
    }
}