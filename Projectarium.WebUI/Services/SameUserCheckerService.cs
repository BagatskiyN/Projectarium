using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Services
{ ///<summary>
  /// Интерфейс сервиса для сравнения текущего пользователя и передаваемого.
  ///</summary>
    public interface ISameUserCheckerService
    {
 
        public Task<bool> SameUserCheckerByVacation(ClaimsPrincipal currentUser,int VacancyId);

    }
    ///<summary>
    /// Класс сервиса для сравнения текущего пользователя и передаваемого. 
    ///</summary>
    public class SameUserCheckerService: ISameUserCheckerService
    {
        public ApplicationDbContext _context;
        public SameUserCheckerService(ApplicationDbContext context)
        {

            _context = context;
        }
        ///<summary>
        ///Метод для сравнения текущего пользователя и пользователя который
        ///создал вакансию на которую текущий пользователь хочет подать заявку. 
        ///</summary>
        public async Task<bool> SameUserCheckerByVacation(ClaimsPrincipal currentUser, int VacancyId)
        {
         

       
            Vacancy vacancy =await _context.Vacancies
                                    .Include(x => x.Project)
                                    .ThenInclude(x => x.UserProfile)
                                    .FirstOrDefaultAsync(x => x.Id == VacancyId);
           

            if(vacancy.Project.UserProfileId == int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
      
    }
    ///<summary>
    /// Метод расширения для сервиса SameUserCheckerService
    ///</summary>
    public static class SameUserCheckerServiceExtention
        {
            public static IServiceCollection SameUserChecker(this IServiceCollection services)
                => services.AddTransient<ISameUserCheckerService,SameUserCheckerService>();
        }

}
