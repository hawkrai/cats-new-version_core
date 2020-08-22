using System.Collections.Generic;
using LMP.Models;

namespace Application.Infrastructure.FoldersManagement
{
    public interface IFoldersManagementService
    {
        List<Folders> GetAllFolders();

        Folders FolderRootBySubjectModuleId(int SubjectModulesId);
    }
}