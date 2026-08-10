using System.Windows.Input;
using WeatherApp.Helpers;

namespace WeatherApp.ViewModels;

public class SidebarItemViewModel : ViewModelBase
{
    public string LocationName { get; }

    public bool IsSidebarEditActive
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    } = false;

    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand RemoveCommand { get; }

    public SidebarItemViewModel(
        string locationName,
        Action<SidebarItemViewModel?> moveUpAction,
        Predicate<SidebarItemViewModel?> canMoveUp,
        Action<SidebarItemViewModel?> moveDownAction,
        Predicate<SidebarItemViewModel?> canMoveDown,
        Action<SidebarItemViewModel?> removeAction,
        Predicate<SidebarItemViewModel?> canRemove)
    {
        LocationName = locationName;
        MoveUpCommand = new RelayAction(_ => moveUpAction(this), _ => canMoveUp(this));
        MoveDownCommand = new RelayAction(_ => moveDownAction(this), _ => canMoveDown(this));
        RemoveCommand = new RelayAction(_ => removeAction(this), _ => canRemove(this));
    }
}