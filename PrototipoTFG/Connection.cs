using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoTFG
{
    /// <summary>
    /// A Connection is a group of Connectors and InterNodes. These represent a connection between two Logic Elements by using multiple lines and intermediate points.
    /// The Start and End of the Connection are references to LogicElement.
    /// The logic implies that the Starting element is followed by the Ending Element, which means the Start considers the End as a Next element, whereas the End considers the Start as a Previous element.
    /// </summary>
    public class Connection
    {
        #region Collections
        private List<Connector> connectors;
        /// <summary>
        /// Gets or Sets the List of the Connection's Connectors
        /// </summary>
        public List<Connector> Connectors
        {
            get { return connectors ?? (connectors = new List<Connector>()); }
            set { }
        }
        private List<InterNode> interNodes;
        /// <summary>
        /// Gets or Sets the List of the Connection's InterNodes.
        /// </summary>
        public List<InterNode> InterNodes
        {
            get { return interNodes ?? (interNodes = new List<InterNode>()); }
            set { }
        }
        #endregion

        #region Start & End
        private DiagramObject start;
        /// <summary>
        /// Gets or Sets the LogicalElement as a Start point
        /// </summary>
        /// <value>DiagramObject reference</value>
        public DiagramObject Start
        {
            get { return start; }
            set { start = value; }
        }

        private DiagramObject end;
        /// <summary>
        /// Gets or Sets the LogicalElement as an End point
        /// </summary>
        /// <value>DiagramObject reference</value>
        public DiagramObject End
        {
            get { return end; }
            set { end = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Empty constructor of a Connection
        /// </summary>
        public Connection()
        {

        }
        #endregion
    }
}
