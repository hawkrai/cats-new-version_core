using System.ComponentModel;

namespace WebAPI.ViewModels.SubjectViewModels
{
    public class SubjectListViewModel
    {
        [DisplayName("Название предмета")]
        public string Name { get; set; }

        [DisplayName("Аббревиатура")]
        public string ShortName { get; set; }

        [DisplayName("Действие")]
        public string Action { get; set; }
    }
}