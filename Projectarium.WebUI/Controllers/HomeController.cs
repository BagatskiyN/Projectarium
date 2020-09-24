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
{    /// <summary>
     ///Контроллер для работы с главной страницей. На главную страницу выводятся все проекты пользователей.
     /// В проекты можно перейти, чтобы ознакомиться с ними подробнее. На странице проекта представлены 
     ///  доступные вакансии на которые можно подать заявку.
     ///</summary> 
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

        /// <summary>
        /// Метод для вывода списка проектов.
        ///</summary>

        public IActionResult Index()
        {
            List<Project> projects = _context.Projects.ToList();
            return View(projects);
        }
        /// <summary>
        ///     Метод CreateRequest используется для передачи в представление частичного представления в котором содержится модальное окно.
        ///   В модальное окно передается вакансия на которую пользователь хотел подать заявку.
        ///</summary> 
        ///  <param name="id">Id вакансии на которую пользователь подает заявку</param>
        public async Task<IActionResult> CreateRequest(int id)
        {

            //Получает из базы данных профиль текущего пользователя
            UserProfile userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.Id == int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == id);
            Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == vacancy.ProjectId);
            //Сравнение Id пользователя который подал заявку на проект и Id создателя проекта. 
            //В случае если пользователи отличаются метод возвращает частичное представление с модальным окном для ввода данных о заявке.
            if (project.UserProfileId == userProfile.Id)
            {
                return Ok();
            }
            else
            {
                return PartialView("~/Views/Shared/HomePartialViews/CreateRequestModalView.cshtml", vacancy);
            }


        }
        /// <summary>
        ///  Метод принимает заявку и сохраняет ее в БД
        ///</summary>
        ///  <param name="id">Объект в котором хранятся Id вакансии и мотивация пользователя который подавал заявку.</param>
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] NewRequest newRequest)
        {
            //Получает из базы данных профиль текущего пользователя
            UserProfile userProfile = await _context.UserProfiles
                                            .FirstOrDefaultAsync(m => m.Id == int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value));

            Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == newRequest.VacancyId);

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
