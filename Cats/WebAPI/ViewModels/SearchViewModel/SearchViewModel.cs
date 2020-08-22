using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.ViewModels.SearchViewModel
{
    public class SearchViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Lecturer> Lecturers { get; set; }
    }
}