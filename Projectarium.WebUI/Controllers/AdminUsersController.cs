using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;
using Projectarium.WebUI.Models.AdminUsersVM;

namespace Projectarium.WebUI.Controllers
{
    [Authorize(Policy = "AdminId")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public static List<Skill> Skills { get; set; } = new List<Skill>();
   
        public static List<NewSkill> NewSkills { get; set; } = new List<NewSkill>();
        public AdminUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        
        }

        // GET: AdminUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserProfiles.Include(u => u.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // GET: AdminUsers/Create
        public IActionResult Create()
        {
          
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM createUserVM, IFormFile imageInp)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = createUserVM.Email, Email = createUserVM.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, "123456");

                if (result.Succeeded)
                {
                    ApplicationUser applicationUser = await _userManager.FindByEmailAsync(createUserVM.Email);

                    UserProfile userProfile = new UserProfile()
                    {   
                        Id=applicationUser.Id,
                        AboutUser = createUserVM.AboutUser
                    };
                    if(createUserVM.FormFile!=null)
                    {
                        byte[] imageData = null;
                        string imageType = null;
                        using (var binaryReader = new BinaryReader(createUserVM.FormFile.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)createUserVM.FormFile.Length);
                            imageType = createUserVM.FormFile.ContentType;
                        }
                        userProfile.ImageData = imageData;
                        userProfile.ImageMimeType = imageType;

                    }
                  
                    _context.UserProfiles.Add(userProfile);

                    await _context.SaveChangesAsync();

                    await _userManager.AddClaimAsync(user, new Claim("UserId", applicationUser.Id.ToString()));

                    return RedirectToAction(nameof(Index));
                }
            }
     
            return RedirectToAction("AdminUsers","Index");
        }

        // GET: AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
         
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser= await _userManager.FindByIdAsync(id.ToString());
            var userProfile = await _context.UserProfiles.FindAsync(id);
            EditUserVM editUserVM = new EditUserVM()
            {
                Id=userProfile.Id,
                Email = applicationUser.Email,
                AboutUser = userProfile.AboutUser,
               
                Links = userProfile.Links,
                //Projects = userProfile.Projects,
                ImageData=userProfile.ImageData,
                ImageType=userProfile.ImageMimeType
            };
            Skills = _context.UserProfiles
                .Include(u => u.Skills)
                .FirstOrDefault(x => x.Id ==id)
                .Skills
                .ToList();
            editUserVM.Skills = Skills;
            if (applicationUser== null)
            {
                return NotFound();
            }
            if (userProfile == null)
            {
                return NotFound();
            }
            return View(editUserVM);
        }

        // POST: AdminUsers/Edit/5
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AboutUser")] EditUserVM editUserVM)
        {
            if (id !=editUserVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    UserProfile userProfile = await _context.UserProfiles.FindAsync(id);
                    userProfile.AboutUser = editUserVM.AboutUser;
                   List<Skill>  FormerSkillsList =_context.UserProfiles
                        .Include(u => u.Skills)
                        .FirstOrDefault(x => x.Id == id)
                        .Skills
                       .ToList();
                    List<Skill> DeletedSkills = FormerSkillsList.Except(Skills).ToList();

                    foreach (var skill in DeletedSkills)
                    {
                       _context.Skills.Remove(skill);
                    }
                   foreach(var item in NewSkills)
                    {
                        Skill skill = new Skill()
                        {
                            Title = item.text,
                          UserProfile=userProfile
                        };
                        await _context.Skills.AddAsync(skill);
                        await _context.SaveChangesAsync();
                   
                    }
                
                    //foreach (var item in NewSkills)
                    //{
                     
                    //     //_context.SkillUsers.

                    //}
                    _context.UserProfiles.Update(userProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(editUserVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }    

            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public ActionResult DeleteSkill(int? id)
        //{
        //    if(id==null)
        //    {
        //        return NotFound();
        //    }
          
        //   if(Skills.Select(x=>x.Id==id)!=null)
        //    {
        //        Skill skill = Skills.Where(x => x.Id == id).FirstOrDefault();
        //        Skills.Remove(skill);
        //    }
        //    return PartialView("~/Shared/AdminUsersPartialViews/SkillsList.cshtml", Skills);
        //}

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteSkill([FromBody]string text)
        {
            Skill skill = Skills.Where(x => x.Title == text).FirstOrDefault();
            Skills.Remove(skill);
            
            NewSkill newSkill = NewSkills.Where(x => x.text == text).FirstOrDefault();
    
            NewSkills.Remove(newSkill);
            return PartialView("~/Views/Shared/AdminUsersPartialViews/SkillsList.cshtml", Skills);

        }
   


        [HttpPost]
        public IActionResult AddSkill([FromBody] NewSkill newSkill)
        {
            if (newSkill.text != null)
            {
                UserProfile userProfile = _context.UserProfiles.FirstOrDefault(x => x.Id == newSkill.id);
                Skill skill = new Skill()
                {
                    Title = newSkill.text,
                    UserProfile = userProfile
                };
                _context.Skills.Add(skill);
                _context.SaveChanges();
            }
            return PartialView("~/Views/Shared/AdminUsersPartialViews/SkillsList.cshtml", _context.Skills.Where(x=>x.Id==newSkill.id).ToList());
        }


        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }
    }
}
