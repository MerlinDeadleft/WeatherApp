using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WeatherApp.Core;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public class SidebarViewModel : ViewModelBase
{
    private readonly IWeatherService weatherService;
    private readonly ISettingsService settingsService;

    private List<string> savedLocations;
    private List<SidebarChange> currentChanges = new List<SidebarChange>();
    private readonly Regex locationNameRules = new Regex(@"^[\p{L}\p{N} ,.'’ʻ()/&–~-]+$");
    private SidebarItemViewModel selectedSidebarItem;

    public ICommand EnableAddingLocationCommand { get; }
    public ICommand AddLocationCommand { get; }
    public ICommand CancelAddLocationCommand { get; }
    public ICommand EnableEditCommand { get; }
    public ICommand SaveChangesCommand { get; }
    public ICommand DiscardChangesCommand { get; }

    public ObservableCollection<SidebarItemViewModel> SavedLocationViewModels
    {
        get;
        private set
        {
            if(field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    }

    public bool IsSidebarEditActive
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            foreach(var viewModel in SavedLocationViewModels)
            {
                viewModel.IsSidebarEditActive = value;
            }

            DispatchPropertyChanged();
        }
    }

    public bool IsAddingLocationActive
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    }

    public string AddedLocationName
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    } = "";

    public bool IsInvalidNameEntered
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    }

    public SidebarViewModel(ISettingsService settingsService, IWeatherService weatherService)
    {
        this.weatherService = weatherService;
        this.settingsService = settingsService;
        savedLocations = settingsService.GetSettings().Locations.ToList();
        CreateSavedLocationViewModels();

        EnableAddingLocationCommand = new RelayAction(ExecuteEnableAddingLocationCommand);
        AddLocationCommand = new RelayAction(ExecuteAddLocationCommand, CanExecuteAddLocationCommand);
        CancelAddLocationCommand = new RelayAction(ExecuteCancelAddLocationCommand);
        EnableEditCommand = new RelayAction(ExecuteEnableEditCommand);
        SaveChangesCommand = new RelayAction(ExecuteSaveChangesCommand);
        DiscardChangesCommand = new RelayAction(ExecuteDiscardChangesCommand);
    }

    private void CreateSavedLocationViewModels()
    {
        var viewModels = savedLocations.Select(CreateSidebarItemViewModel);
        SavedLocationViewModels = new ObservableCollection<SidebarItemViewModel>(viewModels);
        SelectSidebarItem(SavedLocationViewModels.First());
    }

    private SidebarItemViewModel CreateSidebarItemViewModel(string locationName)
    {
        return new SidebarItemViewModel(locationName,
            MoveSidebarItemUp, CanSidebarItemMoveUp,
            MoveSidebarItemDown, CanSidebarItemMoveDow,
            RemoveSidebarItem, CanRemoveSidebarItem,
            SelectSidebarItem, CanSelectSidebarItem);
    }

    private void ExecuteEnableAddingLocationCommand(object? parameter)
    {
        IsAddingLocationActive = true;
    }

    private void DisableAddingLocation()
    {
        AddedLocationName = "";
        IsAddingLocationActive = false;
    }

    private bool CanExecuteAddLocationCommand(object? parameter)
    {
        var validName = locationNameRules.IsMatch(AddedLocationName);
        IsInvalidNameEntered = !validName && AddedLocationName.Length > 0;

        return AddedLocationName.Length > 0 && validName;
    }

    private void ExecuteAddLocationCommand(object? parameter)
    {
        SavedLocationViewModels.Add(CreateSidebarItemViewModel(AddedLocationName));
        savedLocations.Add(AddedLocationName);
        settingsService.UpdateSettings(settingsService.GetSettings() with { Locations = savedLocations.ToList() });
        DisableAddingLocation();
    }

    private void ExecuteCancelAddLocationCommand(object? parameter)
    {
        DisableAddingLocation();
    }

    private void ExecuteEnableEditCommand(object? parameter)
    {
        IsSidebarEditActive = true;
    }

    private void ExecuteSaveChangesCommand(object? parameter)
    {
        foreach(var change in currentChanges)
        {
            switch(change.Type)
            {
                case SidebarChange.OperationType.Move:
                    var location = savedLocations[change.FromIndex];
                    savedLocations.RemoveAt(change.FromIndex);
                    savedLocations.Insert(change.ToIndex, location);
                    break;
                case SidebarChange.OperationType.Remove:
                    savedLocations.RemoveAt(change.FromIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        currentChanges.Clear();
        settingsService.UpdateSettings(settingsService.GetSettings() with  { Locations = savedLocations.ToList() });
        IsSidebarEditActive = false;
        
        if(SavedLocationViewModels.Contains(selectedSidebarItem))
        {
            SelectSidebarItem(SavedLocationViewModels.First());
        }
    }

    private void ExecuteDiscardChangesCommand(object? parameter)
    {
        IsSidebarEditActive = false;
        
        if(currentChanges.Count == 0) return;
        currentChanges.Clear();
        SavedLocationViewModels.Clear();
        CreateSavedLocationViewModels();
    }

    private bool CanSidebarItemMoveUp(SidebarItemViewModel item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        return index > 0;
    }

    private void MoveSidebarItemUp(SidebarItemViewModel item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.Move(index, index - 1);

        currentChanges.Add(new SidebarChange
        {
            Type = SidebarChange.OperationType.Move,
            FromIndex = index,
            ToIndex = index - 1,
            Item = item
        });
    }

    private bool CanSidebarItemMoveDow(SidebarItemViewModel item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        return index < SavedLocationViewModels.Count - 1;
    }

    private void MoveSidebarItemDown(SidebarItemViewModel item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.Move(index, index + 1);

        currentChanges.Add(new SidebarChange
        {
            Type = SidebarChange.OperationType.Move,
            FromIndex = index,
            ToIndex = index + 1,
            Item = item
        });
    }

    private bool CanRemoveSidebarItem(SidebarItemViewModel item)
    {
        return item.LocationName != SettingsModel.IpBasedLocationName;
    }

    private void RemoveSidebarItem(SidebarItemViewModel item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.RemoveAt(index);

        currentChanges.Add(new SidebarChange
        {
            Type = SidebarChange.OperationType.Remove,
            FromIndex = index,
            Item = item
        });
    }

    private bool CanSelectSidebarItem(SidebarItemViewModel item)
    {
        return !IsSidebarEditActive && item != selectedSidebarItem && !weatherService.IsFetching;
    }

    private void SelectSidebarItem(SidebarItemViewModel item)
    {
        if(IsAddingLocationActive)
        {
            DisableAddingLocation();
        }

        selectedSidebarItem?.IsSelected = false;
        selectedSidebarItem = item;
        selectedSidebarItem.IsSelected = true;
        weatherService.SelectedLocation = item.LocationName;
    }

    private class SidebarChange
    {
        public enum OperationType
        {
            Move,
            Remove
        }

        public OperationType Type { get; set; }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public SidebarItemViewModel Item { get; set; }
    }
}