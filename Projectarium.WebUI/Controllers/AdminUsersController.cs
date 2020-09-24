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
    /// <summary>
    ///Контроллер для работы со списком пользользователей. 
    ///Контроллер позволяет просматривать некоторые данные о пользователях, создавать,редактировать и удалять пользователей.
    ///</summary>
    [Authorize(Policy = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        
        }

        /// <summary>
        ///Метод передает в представление список пользователей. 
        ///</summary>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserProfiles.Include(u => u.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }
        /// <summary>
       ///Метод передает в представление данные о пользователе. 
        ///</summary>
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

        /// <summary>
        ///Метод вызывает страницу создания пользователя. 
        ///</summary>
        public IActionResult Create()
        {
          
            return View();
        }


        /// <summary>
        ///Метод сохраняет нового пользователя. 
        ///</summary>
        ///<param name="createUserVM">Объект в котором хранятся данные нового пользователя.</param>
        ///<param name="imageInp">Файл - картинка пользовательского профиля .</param>
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
                        ApplicationUser=applicationUser,
                        Name=createUserVM.Name,
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
     
            return RedirectToAction("Index","AdminUsers");
        }

        /// <summary>
        ///Метод передает в представление данные пользователя для редактирования. 
        ///</summary>
        ///<param name="id">Id пользователя для редактирования.</param>
        public async Task<IActionResult> Edit(int? id)
        {
         
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser= await _userManager.FindByIdAsync(id.ToString());
            var userProfile = await _context.UserProfiles.Include(x=>x.Projects).FirstOrDefaultAsync(x=>x.Id==id);
            EditUserVM editUserVM = new EditUserVM()
            {
                Id = userProfile.Id,
                Name=userProfile.Name,
                Email = applicationUser.Email,
                AboutUser = userProfile.AboutUser,
                Projects = userProfile.Projects,
                Links = userProfile.Links,
                ImageData=userProfile.ImageData,
                ImageType=userProfile.ImageMimeType
            };
           
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


        /// <summary>
        ///Метод сохраняет данные измененного пользователя. 
        ///</summary>
        ///<param name="id">Id пользователя для редактирования.</param>
        ///<param name="editUserVM">Объект хранящий измененные данные пользователя.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AboutUser")] EditUserVM editUserVM)
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
                    userProfile.Name = editUserVM.Name;
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

        /// <summary>
        ///Метод выводит представление для подтверждения удаления пользователя. 
        ///</summary>
        ///<param name="id">Id пользователя для удаления.</param>
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

        /// <summary>
        ///Метод  удаляет пользователя. 
        ///</summary>
        ///<param name="id">Id пользователя для удаления.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfile = await _context.UserProfiles.Include(x=>x.Projects).FirstOrDefaultAsync(x=>x.Id==id);
            ApplicationUser applicationUser = await _context.ApplicationUsers.FindAsync(id);
            foreach(var project in userProfile.Projects)
            {
                _context.Projects.Remove(project);
            }
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        ///Метод  добавления умения пользователя. 
        ///</summary>
        ///<param name="newSkill">Объект хранящий Id пользователя и название умения .</param>
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
