using System.Windows.Input;
using WeatherApp.Core;

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

    public bool IsSelected
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            DispatchPropertyChanged();
        }
    }

    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand SelectItemCommand { get; }

    public SidebarItemViewModel(
        string locationName,
        Action<SidebarItemViewModel?> moveUpAction,
        Predicate<SidebarItemViewModel?> canMoveUp,
        Action<SidebarItemViewModel?> moveDownAction,
        Predicate<SidebarItemViewModel?> canMoveDown,
        Action<SidebarItemViewModel?> removeAction,
        Predicate<SidebarItemViewModel?> canRemove,
        Action<SidebarItemViewModel?> selectItemAction,
        Predicate<SidebarItemViewModel?> canSelectItem)
    {
        LocationName = locationName;
        MoveUpCommand = new RelayAction(_ => moveUpAction(this), _ => canMoveUp(this));
        MoveDownCommand = new RelayAction(_ => moveDownAction(this), _ => canMoveDown(this));
        RemoveCommand = new RelayAction(_ => removeAction(this), _ => canRemove(this));
        SelectItemCommand = new RelayAction(_ => selectItemAction(this), _ => canSelectItem(this));
    }
}