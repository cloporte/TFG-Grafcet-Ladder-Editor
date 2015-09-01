using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrototipoTFG
{
    /// <summary>
    /// Simple implementation of ICommand
    /// Serves as an abstraction of Actions performed by the user via interaction with the UI (for instance, Button Click)
    /// </summary>
    public class Command : ICommand
    {
        public Action Action { get; set; }

        /// <summary>
        /// This method checks if the action is ready and initialized to call it.
        /// </summary>
        /// <param name="parameter">Reference of the object</param>
        public void Execute(object parameter)
        {
            if (Action != null)
                Action();
        }

        /// <summary>
        /// This method is used to check if the Command can be executed
        /// </summary>
        /// <param name="parameter">Reference of the object</param>
        /// <returns>Returns the boolean value IsEnabled</returns>
        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        
        private bool _isEnabled;
        /// <summary>
        /// Gets or Sets isEnabled, and Updates CanExecuteChanged
        /// </summary>
        /// <value>True or False. True means the Command can be executed.</value>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Default Constructor of Command
        /// </summary>
        /// <param name="action">Action of the Command</param>
        public Command(Action action)
        {
            Action = action;
        }
        
    }
}
