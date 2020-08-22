using LMP.Models;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesBaseViewModel
    {
        public ModulesBaseViewModel(int subjectId, Module module)
        {
            SubjectId = subjectId;
            Module = module;
        }

        public int SubjectId { get; set; }

        public Module Module { get; set; }
    }
}