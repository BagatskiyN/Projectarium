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
using Projectarium.WebUI.Services;
using Projectarium.WebUI.Models;
using Projectarium.WebUI.Models.HomeVM;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Projectarium.WebUI.Models.UserProfileVM;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "User")]
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
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;

            UserProfile userProfile = await _context.UserProfiles
                .Include(x => x.Links)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));

            return View(userProfile);
        }
        public async Task<IActionResult> IndexById(int id)
        { 
            UserProfile userProfile = await _context.UserProfiles
                .Include(x => x.Links)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(userProfile);
        }
        public async Task<IActionResult> Edit()
        {
            ClaimsPrincipal currentUser = this.User;

            UserProfile userProfile = await _context.UserProfiles
                .Include(x => x.Links)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));

            return View(userProfile);
        }
        [HttpPost]
        public async Task Edit(string UserName,string AboutUser,IFormFile Image)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            userProfile.Name = UserName;
            userProfile.AboutUser = AboutUser;
            using (var binaryReader = new BinaryReader(Image.OpenReadStream()))
            {
                userProfile.ImageData = binaryReader.ReadBytes((int)Image.Length);
                userProfile.ImageMimeType = Image.ContentType;
            }
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();

        }

        public async Task<FileContentResult> GetUserProfileImage(int id)
        {
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);
            if (userProfile == null)
            {
                return null;
            }
            if (userProfile.ImageData == null || userProfile.ImageMimeType == null)
            {

                return null;
            }
            else
            {
                return File(userProfile.ImageData, userProfile.ImageMimeType);
            }
        }
    }
}