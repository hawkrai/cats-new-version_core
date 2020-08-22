using System.Collections.Generic;
using LMP.Models;

namespace Application.Infrastructure.SubjectManagement
{
    public interface IModulesManagementService
    {
        ICollection<Module> GetModules();
        IEnumerable<Module> GetModules(int subjectId);
    }
}