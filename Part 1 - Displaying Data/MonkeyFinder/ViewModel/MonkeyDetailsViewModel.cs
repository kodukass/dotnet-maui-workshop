namespace MonkeyFinder.ViewModel;

[QueryProperty("Monkey", "Monkey")]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
    public MonkeyDetailsViewModel() 
    { 
        
    }

    [ObservableProperty]
    Monkey monkey;

    //[ICommand]
    //async Task GoBackAsync()
    //{
    //    await Shell.Current.GoToAsync("..?id=1");
    //}
}
