using System.ComponentModel;

namespace WebAPI.ViewModels.ParentalViewModels
{
    public class DataTableStat
    {
        public DataTableStat(string stat)
        {
            Stat = stat;
        }

        [DisplayName("Сообщения")]
        public string Stat { get; set; }
    }
}