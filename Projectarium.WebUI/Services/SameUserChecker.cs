using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Projectarium.Domain.Concrete;
using Projectarium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Services
{
    public interface ISameUserCheckerService
    {
 
        public Task<bool> SameUserCheckerByVacation(ClaimsPrincipal currentUser,int VacancyId);

    }
    public class SameUserCheckerService: ISameUserCheckerService
    {
        public ApplicationDbContext _context;
        public SameUserCheckerService(ApplicationDbContext context)
        {

            _context = context;
        }
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
}
