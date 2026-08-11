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
    private readonly ISettingsService settingsService;
    private readonly SettingsModel settingsModel;
    private List<SidebarChange> currentChanges = new List<SidebarChange>();
    private readonly Regex locationNameRules = new Regex(@"^[\p{L}\p{N} ,.'’ʻ()/&–~-]+$");
    private SidebarItemViewModel? selectedSidebarItem;

    public ICommand EnableAddingLocationCommand { get; }
    public ICommand AddLocationCommand { get; }
    public ICommand CancelAddLocationCommand { get; }
    public ICommand EnableEditCommand { get; }
    public ICommand SaveChangesCommand { get; }
    public ICommand DiscardChangesCommand { get; }

    public ObservableCollection<SidebarItemViewModel> SavedLocationViewModels { get; }

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

    public SidebarViewModel(ISettingsService settingsServiceService)
    {
        settingsService = settingsServiceService;
        settingsModel = settingsServiceService.LoadSettings();
        var viewModels = settingsModel.Locations.Select(CreateSidebarItemViewModel);
        SavedLocationViewModels = new ObservableCollection<SidebarItemViewModel>(viewModels);
        SelectSidebarItem(SavedLocationViewModels.First());

        EnableAddingLocationCommand = new RelayAction(ExecuteEnableAddingLocationCommand);
        AddLocationCommand = new RelayAction(ExecuteAddLocationCommand, CanExecuteAddLocationCommand);
        CancelAddLocationCommand = new RelayAction(ExecuteCancelAddLocationCommand);
        EnableEditCommand = new RelayAction(ExecuteEnableEditCommand);
        SaveChangesCommand = new RelayAction(ExecuteSaveChangesCommand);
        DiscardChangesCommand = new RelayAction(ExecuteDiscardChangesCommand);
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
        settingsModel.Locations.Add(AddedLocationName);
        settingsService.SaveSettings(settingsModel);
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
            switch(change.operationType)
            {
                case SidebarChange.OperationType.Move:
                    var location = settingsModel.Locations[change.FromIndex];
                    settingsModel.Locations.RemoveAt(change.FromIndex);
                    settingsModel.Locations.Insert(change.ToIndex, location);
                    break;
                case SidebarChange.OperationType.Remove:
                    settingsModel.Locations.RemoveAt(change.FromIndex);
                    if(change.Item == selectedSidebarItem)
                    {
                        SelectSidebarItem(SavedLocationViewModels.First());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        currentChanges.Clear();
        settingsService.SaveSettings(settingsModel);
        IsSidebarEditActive = false;
    }

    private void ExecuteDiscardChangesCommand(object? parameter)
    {
        for(int i = currentChanges.Count - 1; i >= 0; i--)
        {
            var change = currentChanges[i];
            switch(change.operationType)
            {
                case SidebarChange.OperationType.Move:
                    SavedLocationViewModels.Move(change.ToIndex, change.FromIndex);
                    break;
                case SidebarChange.OperationType.Remove:
                    SavedLocationViewModels.Insert(change.FromIndex, change.Item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        currentChanges.Clear();
        IsSidebarEditActive = false;
    }

    private bool CanSidebarItemMoveUp(SidebarItemViewModel? item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        return index > 0;
    }

    private void MoveSidebarItemUp(SidebarItemViewModel? item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.Move(index, index - 1);

        currentChanges.Add(new SidebarChange
        {
            operationType = SidebarChange.OperationType.Move,
            FromIndex = index,
            ToIndex = index - 1,
            Item = item
        });
    }

    private bool CanSidebarItemMoveDow(SidebarItemViewModel? item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        return index < SavedLocationViewModels.Count - 1;
    }

    private void MoveSidebarItemDown(SidebarItemViewModel? item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.Move(index, index + 1);

        currentChanges.Add(new SidebarChange
        {
            operationType = SidebarChange.OperationType.Move,
            FromIndex = index,
            ToIndex = index + 1,
            Item = item
        });
    }

    private bool CanRemoveSidebarItem(SidebarItemViewModel? item)
    {
        return item.LocationName != SettingsModel.IpBasedLocationName;
    }

    private void RemoveSidebarItem(SidebarItemViewModel? item)
    {
        var index = SavedLocationViewModels.IndexOf(item);
        SavedLocationViewModels.RemoveAt(index);

        currentChanges.Add(new SidebarChange
        {
            operationType = SidebarChange.OperationType.Remove,
            FromIndex = index,
            Item = item
        });
    }

    private bool CanSelectSidebarItem(SidebarItemViewModel? item)
    {
        return !IsSidebarEditActive && item != selectedSidebarItem;
    }

    private void SelectSidebarItem(SidebarItemViewModel? item)
    {
        if(IsAddingLocationActive)
        {
            DisableAddingLocation();
        }
        selectedSidebarItem?.IsSelected = false;
        selectedSidebarItem = item;
        selectedSidebarItem.IsSelected = true;
    }

    private class SidebarChange
    {
        public enum OperationType
        {
            Move,
            Remove
        }

        public OperationType operationType { get; set; }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public SidebarItemViewModel Item { get; set; }
    }
}