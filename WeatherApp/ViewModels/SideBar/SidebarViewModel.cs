using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WeatherApp.Helpers;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public class SidebarViewModel : ViewModelBase
{
    private readonly ISettingsService settingsService;
    private readonly SettingsModel settingsModel;
    private List<SidebarChange> currentChanges = new List<SidebarChange>();

    public ICommand AddLocationCommand { get; }
    public ICommand EnableEditCommand { get; }
    public ICommand SaveChangesCommand { get; }
    public ICommand DiscardChangesCommand { get; }
    
    public ObservableCollection<SidebarItemViewModel> SavedLocationViewModels { get; private set; }

    public bool IsSidebarEditActive
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            foreach (var viewModel in SavedLocationViewModels)
            {
                viewModel.IsSidebarEditActive = value;
            }
            DispatchPropertyChanged();
        }
    }

    public SidebarViewModel(ISettingsService settingsServiceService)
    {
        settingsService = settingsServiceService;
        settingsModel = settingsServiceService.LoadSettings();
        SetupSavedLocationViewModelsFromSettings();

        AddLocationCommand = new RelayAction(ExecuteAddLocationCommand);
        EnableEditCommand = new RelayAction(ExecuteEnableEditCommand);
        SaveChangesCommand = new RelayAction(ExecuteSaveChangesCommand);
        DiscardChangesCommand = new RelayAction(ExecuteDiscardChangesCommand);
    }

    private void SetupSavedLocationViewModelsFromSettings()
    {
        var viewModels = settingsModel.Locations.Select(locationName =>
            new SidebarItemViewModel(locationName,
                MoveSidebarItemUp, CanSidebarItemMoveUp,
                MoveSidebarItemDown, CanSidebarItemMoveDow,
                RemoveSidebarItem, CanRemoveSidebarItem)
        );
        SavedLocationViewModels = new ObservableCollection<SidebarItemViewModel>(viewModels);
    }

    private void ExecuteAddLocationCommand(object? parameter)
    {
        MessageBox.Show("This function is not yet implemented!", "Not Yet Implemented", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    
    private void ExecuteEnableEditCommand(object? parameter)
    {
        IsSidebarEditActive = true;
    }

    private void ExecuteSaveChangesCommand(object? parameter)
    {
        foreach (var change in currentChanges)
        {
            switch (change.operationType)
            {
                case SidebarChange.OperationType.Move:
                    var location = settingsModel.Locations[change.FromIndex];
                    settingsModel.Locations.RemoveAt(change.FromIndex);
                    settingsModel.Locations.Insert(change.ToIndex, location);
                    break;
                case SidebarChange.OperationType.Remove:
                    settingsModel.Locations.RemoveAt(change.FromIndex);
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
        for (int i = currentChanges.Count - 1; i >= 0; i--)
        {
            var change = currentChanges[i];
            switch (change.operationType)
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

    private class SidebarChange
    {
        public enum OperationType {Move, Remove}
        public OperationType operationType { get; set; }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public SidebarItemViewModel Item { get; set; }
    }
}