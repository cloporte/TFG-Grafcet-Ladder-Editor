using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoTFG
{
    /// <summary>
    /// Each connector represents each line drawn between Diagram Objects.
    /// The Connector's X and Y properties are always 0 because the line coordinates are actually determined,
    /// by the Start.X, Start.Y and End.X, End.Y Nodes' properties.   
    /// </summary>
    public class Connector : DiagramObject
    {

        #region Start & End
        private DiagramObject _start;
        /// <summary>
        /// Gets or Sets the DiagramObject at the Start of the Connector
        /// </summary>
        /// <value>A DiagramObject Reference</value>
        public DiagramObject Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        
        private DiagramObject _end;
        /// <summary>
        /// Gets or Sets the DiagramObject at the End of the Connector
        /// </summary>
        /// <value>A DiagramObject Reference</value>
        public DiagramObject End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnPropertyChanged("End");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Empty constructor of a Conector
        /// </summary>
        public Connector() { }
        #endregion
    }

}
