using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Core.UI.HtmlHelpers
{
    public class BaseNumberedGridItem
    {
        [Display(Order = 1)]
        [DisplayName("№")]
        public virtual int Number { get; set; }
    }
}