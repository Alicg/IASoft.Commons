namespace SVA.Infrastructure.Dictionaries
{
    using System.ComponentModel;

    public interface IViewModelDictionary : INotifyPropertyChanged
    {
        string DictionaryName { get; }
    }
}
