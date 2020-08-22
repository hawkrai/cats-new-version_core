using System.ComponentModel.DataAnnotations;

namespace Application.Core.Data
{
    public abstract class ModelBase : IHasIdentifyKey
    {
        public virtual bool IsNew => Id == 0;

        [Key]
        public int Id { get; set; }
    }
}