namespace Application.Core.UI.ViewModels
{
    public class ModelViewModelBase<TModel> : ViewModelBase where TModel : new()
    {
        public ModelViewModelBase()
        {
        }

        public ModelViewModelBase(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; set; }
    }
}