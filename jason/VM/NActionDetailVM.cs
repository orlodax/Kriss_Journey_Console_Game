using Action = lybra.Action;
using jason.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace jason.VM
{
    public class NActionDetailVM : Observable
    {
        private ObservableCollection<Action> actions;
        private Action selectedAction;

        public ObservableCollection<Action> Actions { get => actions; set => SetValue(ref actions, value); }
        public Action SelectedAction { get => selectedAction; set => SetValue(ref selectedAction, value); }

        public NActionDetailVM(List<Action> SelectedNodeActions)
        {
            Actions = new ObservableCollection<Action>(SelectedNodeActions);
        }
    }
}
