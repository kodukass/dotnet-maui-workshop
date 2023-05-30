﻿using MonkeyFinder.Services;
namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    MonkeyService monkeyService;
    public ObservableCollection<Monkey> Monkeys { get; } = new();

    IConnectivity connectivity;
    IGeolocation geolocation;
    public Command GetMonkeysCommand { get; }
    public Command GetClosestMonkeyCommand { get; }
    public Command GoToDetailsCommand { get; }
    public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation, Monkey monkey)
    {
        Title = "Monkey Finder";
        this.monkeyService = monkeyService;
        GetMonkeysCommand = new Command(async () => await GetMonkeysAsync());
        GetClosestMonkeyCommand = new Command(async () => await GetClosestMonkeyAsync());
        this.connectivity = connectivity;
        this.geolocation = geolocation;
        GoToDetailsCommand = new Command(async () => await GoToDetailsAsync(monkey));
    }

    [ObservableProperty]
    bool isRefreshing;

    //[ICommand]
    async Task GetClosestMonkeyAsync()
    {
        if(IsBusy || Monkeys.Count == 0) 
            return;

        try
        {
            //Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>
            var location = await geolocation.GetLastKnownLocationAsync();
            if (location is null)
            {
                location = await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30),
                    });
            }

            if (location is null)
                return;

            var first = Monkeys.OrderBy(m =>
            location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Kilometers)
            ).FirstOrDefault();

            if (first is null)
                return;

            await Shell.Current.DisplayAlert("Closest Monkey",
                $"{first.Name} in {first.Location}", "OK");
        }

        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get closest monkeys: {ex.Message}", "ok");
        }
    }
    //[ICommand]
    async Task GoToDetailsAsync(Monkey monkey)
    {
        if(monkey is null) 
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
            new Dictionary<string, object>
            {
                {"Monkey", monkey}
            });
    }
    //[ICommand]
    async Task GetMonkeysAsync()
    {
        if(IsBusy) 
            return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet issue",
                $"Check your internet", "ok");
                return;
            }
            IsBusy = true;
            var monkeys = await monkeyService.GetMonkeys();

            if(Monkeys.Count != 0)
                Monkeys.Clear();

            foreach(var monkey in monkeys)
                Monkeys.Add(monkey);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!", 
                $"Unable to get monkeys: {ex.Message}", "ok");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
}
