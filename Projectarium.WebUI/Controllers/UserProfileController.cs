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

        [HttpGet("UserProfile/Index/id")]
        public async Task<IActionResult> Index(int id)
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
        public async Task Edit(string UserName, string AboutUser, IFormFile Image)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (UserName != null)
            {
                userProfile.Name = UserName;
            }

            if (AboutUser != null)
            {

                userProfile.AboutUser = AboutUser;
            }
            if (Image != null)
            {
                using (var binaryReader = new BinaryReader(Image.OpenReadStream()))
                {
                    userProfile.ImageData = binaryReader.ReadBytes((int)Image.Length);
                    userProfile.ImageMimeType = Image.ContentType;
                }
            }

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();

        }
        [HttpPost]
        public async Task<IActionResult> AddUserLink([FromBody] string link)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (userProfile == null)
            {
                return NotFound();
            }
            else
            {
                Uri uri = new Uri(link);
                Link link1 = new Link()
                {
                    LinkText = link,
                    Mask = _linkMasker.MaskLink(uri),
                    UserProfile = userProfile

                };
                await _context.Links.AddAsync(link1);
                await _context.SaveChangesAsync();
                return PartialView("~/Views/Shared/ProjectManagerPartialViews/EditPartialViews/EditLinkView.cshtml", link1);
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddUserSkill([FromBody] string Title)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (userProfile == null)
            {
                return NotFound();
            }
            else
            {
                Skill skill = new Skill()
                {
                    Title = Title,
                    UserProfile = userProfile
                };
                await _context.Skills.AddAsync(skill);
                await _context.SaveChangesAsync();
                return PartialView("~/Views/Shared/ProjectManagerPartialViews/EditPartialViews/EditSkillView.cshtml", skill);
            }


        }

        [HttpPost]
        public async Task<IActionResult> CheckUser([FromBody] int? Id)
        {
            ClaimsPrincipal currentUser = this.User;
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));

            if (Id == userProfile.Id)
            {

                return PartialView("~/Views/Shared/UserProfilePartialViews/EditProfileHeaderView.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/UserProfilePartialViews/ViewProfileHeaderView.cshtml");
            }


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