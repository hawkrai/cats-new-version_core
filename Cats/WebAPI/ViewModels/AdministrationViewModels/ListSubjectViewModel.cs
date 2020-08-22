using System.Collections.Generic;
using Application.Core.UI.HtmlHelpers;
using LMP.Models;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class ListSubjectViewModel : BaseNumberedGridItem
    {
        public List<Subject> Subjects { get; set; }

        public string Name { get; set; }

        public static ListSubjectViewModel FormSubjects(List<Subject> subjects, string name)
        {
            return new ListSubjectViewModel
            {
                Subjects = subjects,
                Name = name
            };
        }
    }
}