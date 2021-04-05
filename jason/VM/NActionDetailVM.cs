using Action = lybra.Action;
using jason.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace jason.VM
{
    public class NActionDetailVM : Observable
    {
        private ObservableCollection<Action> actions;
        private Action selectedAction;

        public ObservableCollection<Action> Actions { get => actions; set => SetValue(ref actions, value); }
        public Action SelectedAction { get => selectedAction; set => SetValue(ref selectedAction, value); }

        public ICommand NewAction { get; private set; }

        public NActionDetailVM(List<Action> SelectedNodeActions)
        {
            Actions = new ObservableCollection<Action>(SelectedNodeActions);

            NewAction = new RelayCommand(Exec_NewAction);
        }

        void Exec_NewAction(object parameter)
        { 
        
        }
    }
}
