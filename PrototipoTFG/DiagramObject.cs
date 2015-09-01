using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace PrototipoTFG
{

    /// <summary>
    /// DiagramObject is the basic class that represents a graphical element that might be displayed on the Diagram Area
    /// A DiagramObject has a name, can be a preview element, can be highlighted and also has coordinates.
    /// </summary>
    public abstract class DiagramObject : INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Default empty Constructor of a DiagramObject
        /// </summary>
        public DiagramObject()
        {

        }
        #endregion

        #region Name, isNew, isHighlighted
        private string _name;
        /// <summary>
        /// Gets or Sets the Name of the DiagramObject. Could be used as an ID but it is not restricted in this version.
        /// </summary>
        /// <value>A String</value>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool _isNew;
        /// <summary>
        /// Gets or Sets isNew. This boolean represents if the DiagramObject is recently created and still need a confirmation.
        /// During the New state, objects are just a preview that still need to be confirmed by the user.
        /// </summary>
        /// <value>A boolean value.</value>
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged("IsNew");
            }
        }


        // New, was in node
        private bool _isHighlighted { get; set; }
        /// <summary>
        /// Gets or Sets isHighlighted. This boolean represents if the DiagramObject is being Highlighted on the UI.
        /// </summary>
        /// <value>A boolean value.</value>
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }
        #endregion

        #region Coordinates
        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                //"Grid Snapping"
                //this actually "rounds" the value so that it will always be a multiple of 50.
                _x = (Math.Round(value / 50.0)) * 50;
                OnPropertyChanged("X");
            }
        }
        
        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                //"Grid Snapping"
                //this actually "rounds" the value so that it will always be a multiple of 50.
                _y = (Math.Round(value / 50.0)) * 50;
                OnPropertyChanged("Y");
            }
        }
        #endregion

        #region Event Handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
