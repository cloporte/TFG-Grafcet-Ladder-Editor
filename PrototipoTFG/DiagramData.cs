using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrototipoTFG
{
    /// <summary>
    /// DiagramData is a group of ObservableCollections of all the Elements a Diagram has : Nodes, Transitions, InterNodes, Inputs, Outputs, NotInputs, NotOuputs, Connectors and Connections.
    /// It is all the data needed to save or load a Diagram.
    /// </summary>
    public class DiagramData
    {
        #region Collections
        private ObservableCollection<Node> nodes;
        /// <summary>
        /// Gets or Sets the Nodes of the Diagram
        /// </summary>
        /// <value>Node references</value>
        public ObservableCollection<Node> Nodes
        {
            get { return nodes ?? (nodes = new ObservableCollection<Node>());}
            set { }
        }


        private ObservableCollection<Transition> transitions;
        /// <summary>
        /// Gets or Sets the Transitions of the Diagram
        /// </summary>
        /// <value>Transition references</value>
        public ObservableCollection<Transition> Transitions
        {
            get { return transitions ?? (transitions = new ObservableCollection<Transition>()); }
            set { }
        }

        private ObservableCollection<InterNode> interNodes;
        /// <summary>
        /// Gets or Sets the InterNodes of the Diagram
        /// </summary>
        /// <value>InterNodes references</value>
        public ObservableCollection<InterNode> InterNodes
        {
            get { return interNodes ?? (interNodes = new ObservableCollection<InterNode>()); }
            set { }
        }

        private ObservableCollection<InputLadder> inputs;
        /// <summary>
        /// Gets or Sets the Inputs of the Diagram
        /// </summary>
        /// <value>Inputs references</value>
        public ObservableCollection<InputLadder> Inputs
        {
            get { return inputs ?? (inputs = new ObservableCollection<InputLadder>()); }
            set { }
        }

        private ObservableCollection<OutputLadder> outputs;
        /// <summary>
        /// Gets or Sets the Outputs of the Diagram
        /// </summary>
        /// <value>Output references</value>
        public ObservableCollection<OutputLadder> Outputs
        {
            get { return outputs ?? (outputs = new ObservableCollection<OutputLadder>()); }
            set { }
        }

        private ObservableCollection<NotInputLadder> notInputs;
        /// <summary>
        /// Gets or Sets the NotInputs of the Diagram
        /// </summary>
        /// <value>NotInput references</value>
        public ObservableCollection<NotInputLadder> NotInputs
        {
            get { return notInputs ?? (notInputs = new ObservableCollection<NotInputLadder>()); }
            set { }
        }

        private ObservableCollection<NotOutputLadder> notOutputs;
        /// <summary>
        /// Gets or Sets the NotOutputs of the Diagram
        /// </summary>
        /// <value>NotOutput references</value>
        public ObservableCollection<NotOutputLadder> NotOutputs
        {
            get { return notOutputs ?? (notOutputs = new ObservableCollection<NotOutputLadder>()); }
            set { }
        }

        private ObservableCollection<Connector> connectors;
        /// <summary>
        /// Gets or Sets the Connectors of the Diagram
        /// </summary>
        /// <value>Connector references</value>
        [XmlIgnore]
        public ObservableCollection<Connector> Connectors
        {
            get { return connectors ?? (connectors = new ObservableCollection<Connector>()); }
            set { }
        }

        private Collection<Connection> connections;
        /// <summary>
        /// Gets or Sets the Connections of the Diagram
        /// </summary>
        /// <value>Connection references</value>
        public Collection<Connection> Connections
        {
            get { return connections ?? (connections = new Collection<Connection>()); }
            set { }
        }
        #endregion

        #region Method
        /// <summary>
        /// This method checks if there are elements initialized in the Diagram
        /// </summary>
        /// <returns>A boolean value</returns>
        public bool diagramDataHasElements() {
            return (nodes.Count + transitions.Count + interNodes.Count + inputs.Count + outputs.Count + notInputs.Count + notOutputs.Count + Connectors.Count) > 0;
        }
        #endregion
    }
}
