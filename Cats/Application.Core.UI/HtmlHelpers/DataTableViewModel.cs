namespace Application.Core.UI.HtmlHelpers
{
	public class DataTableViewModel
	{
		//public DataTableVm DataTable
		//{
		//	get;
		//	set;
		//} todo #

		public DataTableOptions DataTableOptions
		{
			get;
			set;
		}

		public DataTableViewModel()
		{
			DataTableOptions = new DataTableOptions();
		}
	}
}