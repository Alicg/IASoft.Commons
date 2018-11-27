using System.ComponentModel;

namespace IASoft.WPFCommons
{
    public interface IPageViewModel : IBaseViewModel, INotifyPropertyChanged
    {
        string PageName { get; }

        //bool IsSelected { get; set; }
    }
}