namespace MonkeyFinder.ViewModel;

[QueryProperty("Monkey", "Monkey")]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
    IMap map;
    public Command OpenMapCommand { get; }

    public MonkeyDetailsViewModel() 
    {
        this.map = map;
        OpenMapCommand = new Command(async () => await OpenMapAsync());
    }

    [ObservableProperty]
    Monkey monkey;

   // [ICommand]
    async Task OpenMapAsync()
    {
        try
        {
            await map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
                new MapLaunchOptions
                {
                    Name = Monkey.Name,
                    NavigationMode = NavigationMode.None
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to open map: {ex.Message}", "ok");
        }
    }
    //[ICommand]
    //async Task GoBackAsync()
    //{
    //    await Shell.Current.GoToAsync("..?id=1");
    //}
}
