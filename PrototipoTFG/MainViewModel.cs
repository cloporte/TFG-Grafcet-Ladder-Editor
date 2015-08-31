using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;




namespace PrototipoTFG
{
    /// <summary>
    /// MainViewModel is the main Business Logic Layer
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {

        #region Collections
        /// <summary>
        /// Gets the Nodes observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<Node> Nodes
        {
            get { return dd.Nodes ?? (dd.Nodes = new ObservableCollection<Node>()); }
        }

        /// <summary>
        /// Gets the Transitions observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<Transition> Transitions
        {
            get { return dd.Transitions ?? (dd.Transitions = new ObservableCollection<Transition>()); }
        }

        /// <summary>
        /// Gets the Nodes observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<InterNode> InterNodes
        {
            get { return dd.InterNodes ?? (dd.InterNodes = new ObservableCollection<InterNode>()); }
        }

        /// <summary>
        /// Gets the Inputs observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<InputLadder> Inputs
        {
            get { return dd.Inputs ?? (dd.Inputs = new ObservableCollection<InputLadder>()); }
        }

        /// <summary>
        /// Gets the Outputs observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<OutputLadder> Outputs
        {
            get { return dd.Outputs ?? (dd.Outputs = new ObservableCollection<OutputLadder>()); }
        }

        /// <summary>
        /// Gets the NotInputs observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<NotInputLadder> NotInputs
        {
            get { return dd.NotInputs ?? (dd.NotInputs = new ObservableCollection<NotInputLadder>()); }
        }

        /// <summary>
        /// Gets the NotOutputs observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<NotOutputLadder> NotOutputs
        {
            get { return dd.NotOutputs ?? (dd.NotOutputs = new ObservableCollection<NotOutputLadder>()); }
        }

        /// <summary>
        /// Gets the Connectors observable collection from the DiagramData.
        /// </summary>
        public ObservableCollection<Connector> Connectors
        {
            get { return dd.Connectors ?? (dd.Connectors = new ObservableCollection<Connector>()); }
        }

        /// <summary>
        /// Gets the Connections observable collection from the DiagramData.
        /// </summary>
        public Collection<Connection> Connections
        {
            get { return dd.Connections ?? (dd.Connections = new Collection<Connection>()); }
        }

        #endregion

        #region Current Selection
        public DiagramObject PreviousObject;
        private DiagramObject _selectedObject;        
        /// <summary>
        /// Gets or Sets current Selected Object by the User.
        /// Setting a new Selected Object, updates the PreviousObject and resets the Highlighted diagram objects.
        /// </summary>
        public DiagramObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                
                Nodes.ToList().ForEach(x => x.IsHighlighted = false);
                Transitions.ToList().ForEach(x => x.IsHighlighted = false);
                InterNodes.ToList().ForEach(x => x.IsHighlighted = false);
                Inputs.ToList().ForEach(x => x.IsHighlighted = false);
                Outputs.ToList().ForEach(x => x.IsHighlighted = false);
                NotInputs.ToList().ForEach(x => x.IsHighlighted = false);
                NotOutputs.ToList().ForEach(x => x.IsHighlighted = false);


                if (_selectedObject != null && !_selectedObject.IsNew && (_selectedObject is Node || _selectedObject is Transition
                    || _selectedObject is InterNode || _selectedObject is OutputLadder || _selectedObject is InputLadder
                    || _selectedObject is NotOutputLadder || _selectedObject is NotInputLadder))
                {
                    PreviousObject = _selectedObject; // Save the previous SelectedObject if it is not new
                }
                _selectedObject = value;
                OnPropertyChanged("SelectedObject");
                // Enable Delete and InterNode Command only when we are selecting an object
                DeleteCommand.IsEnabled = value != null; 
                InterNodeEnabled = value != null; 

                // If we are selectiing a Connector, we highlight its start and end
                var connector = value as Connector;
                if (connector != null)
                {
                    if (connector.Start != null) connector.Start.IsHighlighted = true;
                    if (connector.End != null) connector.End.IsHighlighted = true;
                }

            }
        }

        #endregion

        #region Bool (Visibility) Options

        private bool _showNames;
        public bool ShowNames
        {
            get { return _showNames; }
            set
            {
                _showNames = value;
                OnPropertyChanged("ShowNames");
            }
        }

        private bool _showCurrentCoordinates;
        public bool ShowCurrentCoordinates
        {
            get { return _showCurrentCoordinates; }
            set
            {
                _showCurrentCoordinates = value;
                OnPropertyChanged("ShowCurrentCoordinates");
            }
        }

        private bool _showAllCoordinates;
        public bool ShowAllCoordinates
        {
            get { return _showAllCoordinates; }
            set
            {
                _showAllCoordinates = value;
                OnPropertyChanged("ShowAllCoordinates");
            }
        }

        #endregion

        #region Constructor

        private int nodeNumber;
        private int transitionNumber;
        private int inputNumber;
        private int outputNumber;
        private int notInputNumber;
        private int notOutputNumber;
        /// <summary>
        /// The reference of the DiagramData that contains all the elements of each Observable Collection in the MainViewModel
        /// </summary>
        public DiagramData dd { get; set; }
        /// <summary>
        /// Auxliary DiagramData used for the XML Serializer
        /// </summary>
        public DiagramData auxDD { get; set; }
        public MainWindow MainWindowReference;
        
        /// <summary>
        /// Constructor of the MainViewModel, initializes all needed properties
        /// </summary>
        /// <param name="mw">The MainWindow</param>
        public MainViewModel(MainWindow mw)
        {
            nodeNumber = 0;
            transitionNumber = 0;
            inputNumber = 0;
            outputNumber = 0;
            notInputNumber = 0;
            notOutputNumber = 0;
            MainWindowReference = mw;
            NewProjectCommand.IsEnabled = true;
            SaveProjectCommand.IsEnabled = true;
            OpenProjectCommand.IsEnabled = true;
            AboutCommand.IsEnabled = true;
            
            dd = new DiagramData();
            

            ShowAllCoordinates = false;
            ShowNames = true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Creating New Objects


        private bool _creatingNewNode;
        public bool CreatingNewNode
        {
            get { return _creatingNewNode; }
            set
            {
                _creatingNewNode = value;
                OnPropertyChanged("CreatingNewNode");

                if (value) CreateNewNode();
                else RemoveNewObjects();
            }
        }

        public void CreateNewNode()
        {
            var newnode = new Node()
            {
                Name = "E" + nodeNumber,
                IsNew = true
            };

            Nodes.Add(newnode);
            nodeNumber++;
            SelectedObject = newnode;
        }

        private bool _creatingNewTransition;
        public bool CreatingNewTransition
        {
            get { return _creatingNewTransition; }
            set
            {
                _creatingNewTransition = value;
                OnPropertyChanged("CreatingNewTransition");

                if (value)
                    CreateNewTransition();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewTransition()
        {
            var newTransition = new Transition()
            {
                Name = "Transition" + transitionNumber,
                IsNew = true
            };

            Transitions.Add(newTransition);
            transitionNumber++;
            SelectedObject = newTransition;
        }

        private bool _creatingNewInput;
        public bool CreatingNewInput
        {
            get { return _creatingNewInput; }
            set
            {
                _creatingNewInput = value;
                OnPropertyChanged("CreatingNewInput");

                if (value)
                    CreateNewInput();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewInput()
        {
            var newInput = new InputLadder()
            {
                Name = "I" + inputNumber,
                IsNew = true
            };            
            Inputs.Add(newInput);
            inputNumber++;
            SelectedObject = newInput;
        }

        private bool _creatingNewOutput;
        public bool CreatingNewOutput
        {
            get { return _creatingNewOutput; }
            set
            {
                _creatingNewOutput = value;
                OnPropertyChanged("CreatingNewOutput");

                if (value)
                    CreateNewOutput();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewOutput()
        {
            var newOutput = new OutputLadder()
            {
                Name = "I" + outputNumber,
                IsNew = true
            };

            Outputs.Add(newOutput);
            outputNumber++;
            SelectedObject = newOutput;
        }

        private bool _creatingNewNotInput;
        public bool CreatingNewNotInput
        {
            get { return _creatingNewNotInput; }
            set
            {
                _creatingNewNotInput = value;
                OnPropertyChanged("CreatingNewNotInput");

                if (value)
                    CreateNewNotInput();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewNotInput()
        {
            var newNotInput = new NotInputLadder()
            {
                Name = "I" + notInputNumber,
                IsNew = true
            };
            NotInputs.Add(newNotInput);
            notInputNumber++;
            SelectedObject = newNotInput;
        }

        private bool _creatingNewNotOutput;
        public bool CreatingNewNotOutput
        {
            get { return _creatingNewNotOutput; }
            set
            {
                _creatingNewNotOutput = value;
                OnPropertyChanged("CreatingNewNotOutput");

                if (value)
                    CreateNewNotOutput();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewNotOutput()
        {
            var newNotOutput = new NotOutputLadder()
            {
                Name = "I" + notOutputNumber,
                IsNew = true
            };

            NotOutputs.Add(newNotOutput);
            notOutputNumber++;
            SelectedObject = newNotOutput;
        }

        private bool _interNodeEnabled;
        /// <summary>
        /// Indicates if we're able to use the insertConnection button
        /// </summary>
        public bool InterNodeEnabled
        {
            get { return _interNodeEnabled; }
            set
            {
                _interNodeEnabled = value;
                OnPropertyChanged("InterNodeEnabled");
            }
        }

        /// <summary>
        /// Current connection being created
        /// </summary>
        public Connection newConnection;
        /// <summary>
        /// Current connector added to the Connection
        /// </summary>
        public Connector interConnector;
        private bool _creatingInterNode;
        public bool CreatingNewInterNode
        {
            get { return _creatingInterNode; }
            set
            {
                _creatingInterNode = value;
                OnPropertyChanged("CreatingNewInterNode");

                if (value)
                    CreateNewConnection();
                else
                    RemoveNewObjects();
            }
        }

        public void CreateNewConnection()
        {
            // Create a new interNode that will be previewed as the SelectedObject
            var newInterNode = new InterNode()
            {
                Name = "InterNode" + (InterNodes.Count + 1),
                IsNew = true
            };

            InterNodes.Add(newInterNode);
            SelectedObject = newInterNode;

            // Create a connector that will be referenced as interConnector and connect the PreviousObject with the new Internode
            var connector = new Connector()
            {
                Name = "Connector" + (Connectors.Count + 1),
                IsNew = true,
            };

            Connectors.Add(connector);
            interConnector = connector;
            interConnector.Start = PreviousObject;
            // We only create a logic connection if it is starting from a Logic Element
            // Because if the PreviousObject is not a logic element, that means it is an interNode
            // and we're still building a connection.
            if (PreviousObject is LogicElement)
            {
                newConnection = new Connection();
                Connections.Add(newConnection);
                newConnection.Start = PreviousObject as LogicElement;
                newConnection.Connectors.Add(connector);
            }
        }

        /// <summary>
        /// Removes the Objects that are still in preview mode
        /// </summary>
        public void RemoveNewObjects()
        {
            Connectors.Where(x => (x.Start != null && x.Start.IsNew) || (x.End != null && x.End.IsNew)).ToList().ForEach(x => Connectors.Remove(x));
            Nodes.Where(x => x.IsNew).ToList().ForEach(x => Nodes.Remove(x));
            Transitions.Where(x => x.IsNew).ToList().ForEach(x => Transitions.Remove(x));
            InterNodes.Where(x => x.IsNew).ToList().ForEach(x => InterNodes.Remove(x));
            Connectors.Where(x => x.IsNew).ToList().ForEach(x => Connectors.Remove(x));
            Inputs.Where(x => x.IsNew).ToList().ForEach(x => Inputs.Remove(x));
            NotInputs.Where(x => x.IsNew).ToList().ForEach(x => NotInputs.Remove(x));
            Outputs.Where(x => x.IsNew).ToList().ForEach(x => Outputs.Remove(x));
            NotOutputs.Where(x => x.IsNew).ToList().ForEach(x => NotOutputs.Remove(x));

        }

        #endregion

        #region NewProject Command

        private Command _newProjectCommand;
        public Command NewProjectCommand
        {
            get { return _newProjectCommand ?? (_newProjectCommand = new Command(NewProject)); }
        }
        
        private void NewProject()
        {
            if (dd.diagramDataHasElements()) {
                // Show dialog confirm
                MessageBoxResult result = MessageBox.Show("Would you like to save your current Project ?", "Current Project", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();
                    
                }
                ClearData();
                 
            }


        }

        /// <summary>
        /// Clear all observable collections
        /// </summary>
        private void ClearData()
        {
            InterNodes.Clear();
            Connections.Clear();
            Connectors.Clear();
            Nodes.Clear();
            Transitions.Clear();
            Inputs.Clear();
            Outputs.Clear();
            NotInputs.Clear();
            NotOutputs.Clear();
        }

        #endregion

        #region SaveProject Command

        private Command _saveProjectCommand;
        public Command SaveProjectCommand
        {
            get { return _saveProjectCommand ?? (_saveProjectCommand = new Command(SaveProject)); }
        }

        private void SaveProject()
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save XML File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                new XmlSerializer(typeof(DiagramData)).Serialize(fs, dd);
                fs.Close();
            }
        }

        #endregion

        #region OpenProject Command

        private Command _openProjectCommand;
        public Command OpenProjectCommand
        {
            get { return _openProjectCommand ?? (_openProjectCommand = new Command(OpenProject)); }
        }

        private void OpenProject()
        {
            if (dd.diagramDataHasElements())
            {
                // Show dialog confirm
                MessageBoxResult result = MessageBox.Show("Would you like to save your current Project ?", "Current Project", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();

                }
            }
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML File|*.xml";
            openFileDialog1.Title = "Open Project";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                FileStream fs = (FileStream)openFileDialog1.OpenFile();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DiagramData));
                auxDD = (DiagramData)xmlSerializer.Deserialize(fs);
                fs.Close();
                FillData(auxDD);
            }
            
        }

        private void FillData(DiagramData auxDD)
        {
            ClearData();
            // Logic Elements
            foreach (Node n in auxDD.Nodes) { dd.Nodes.Add(n); }
            foreach (Transition n in auxDD.Transitions) { dd.Transitions.Add(n); }            
            foreach (InputLadder n in auxDD.Inputs) { dd.Inputs.Add(n); }
            foreach (NotInputLadder n in auxDD.NotInputs) { dd.NotInputs.Add(n); }
            foreach (OutputLadder n in auxDD.Outputs) { dd.Outputs.Add(n); }
            foreach (NotOutputLadder n in auxDD.NotOutputs) { dd.NotOutputs.Add(n); }
            foreach (InterNode n in auxDD.InterNodes) { dd.InterNodes.Add(n); }

            // Rebuilding connections
            foreach (Connection n in auxDD.Connections)
            { 
                dd.Connections.Add(n);
                n.Start = GetDiagramObject(n.Start);
                n.End = GetDiagramObject(n.End);
                Collection<InterNode> auxCollection = new Collection<InterNode>();
                foreach (InterNode inter in n.InterNodes)
                {
                    // For each internode, we need to make sure it is one of those already initiliazed and present in dd.InterNodes
                    auxCollection.Add(GetDiagramObject(inter) as InterNode);
                }
                foreach (InterNode inter in auxCollection)
                {
                    n.InterNodes.Add(inter);
                }
                foreach (Connector c in n.Connectors)
                {
                    dd.Connectors.Add(c);
                    c.Start = GetDiagramObject(c.Start);
                    c.End = GetDiagramObject(c.End);
                }

                

                
            }

            LogicElement auxLogicElement;
            foreach (Connection n in dd.Connections)
            {
                // We need to fill the Next and Previous lists of each logicElement that is connected to a valid connection
                if (n.Start != null && n.End != null)
                {
                    auxLogicElement = n.Start as LogicElement;
                    auxLogicElement.Next.Add(n.End as LogicElement);
                    auxLogicElement = n.End as LogicElement;
                    auxLogicElement.Previous.Add(n.Start as LogicElement);

                    
                }
            }


        }

        #endregion

        #region Delete Command

        private Command _deleteCommand;
        public Command DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new Command(Delete)); }
        }
        /// <summary>
        /// Deletes an element and its connections
        /// </summary>
        private void Delete()
        {
            
            if (SelectedObject is Connector) RemoveConnectionByConnector(SelectedObject as Connector);
            if (SelectedObject is Node)
            {
                var node = SelectedObject as Node;
                Connectors.Where(x => x.Start == node || x.End == node).ToList().ForEach(x => RemoveConnectionByConnector(x));
                Nodes.Remove(node);
            }
            if (SelectedObject is Transition)
            {
                var transition = SelectedObject as Transition;
                Connectors.Where(x => x.Start == transition || x.End == transition).ToList().ForEach(x => RemoveConnectionByConnector(x));
                Transitions.Remove(transition);
            }
            if (SelectedObject is InputLadder)
            {
                var input = SelectedObject as InputLadder;
                Connectors.Where(x => x.Start == input || x.End == input).ToList().ForEach(x => RemoveConnectionByConnector(x));
                Inputs.Remove(input);
            }
            if (SelectedObject is OutputLadder)
            {
                var output = SelectedObject as OutputLadder;
                Connectors.Where(x => x.Start == output || x.End == output).ToList().ForEach(x => RemoveConnectionByConnector(x));
                Outputs.Remove(output);
            }

            if (SelectedObject is NotInputLadder)
            {
                var notInput = SelectedObject as NotInputLadder;
                Connectors.Where(x => x.Start == notInput || x.End == notInput).ToList().ForEach(x => RemoveConnectionByConnector(x));
                NotInputs.Remove(notInput);
            }

            if (SelectedObject is NotOutputLadder)
            {
                var notOutput = SelectedObject as NotOutputLadder;
                Connectors.Where(x => x.Start == notOutput || x.End == notOutput).ToList().ForEach(x => RemoveConnectionByConnector(x));
                NotOutputs.Remove(notOutput);
            }


            if (SelectedObject is InterNode)
            {
                var interNode = SelectedObject as InterNode;
                if (Connections.First(x => InterNodes.Contains(interNode)) != null) RemoveConnectionByConnector(Connectors.First(x => x.Start == interNode || x.End == interNode));
                else
                {
                    Connectors.Where(x => x.Start == interNode || x.End == interNode).ToList().ForEach(x => Connectors.Remove(x));
                    InterNodes.Remove(interNode);
                }
            }


        }

        #endregion

        #region About Command

        private Command _aboutCommand;
        public Command AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new Command(AboutProject)); }
        }
        /// <summary>
        /// Launches the About Window
        /// </summary>
        private void AboutProject()
        {
            About w = new About();
            w.Show();

        }


        #endregion

        #region Scrolling support

        private double _areaHeight = 500;
        /// <summary>
        /// Gets or Sets the Height of the Area for scrolling
        /// </summary>
        public double AreaHeight
        {
            get { return _areaHeight; }
            set
            {
                _areaHeight = value;
                OnPropertyChanged("AreaHeight");
            }
        }

        private double _areaWidth = 500;
        /// <summary>
        /// Gets or Sets the Height of the Area for scrolling
        /// </summary>
        public double AreaWidth
        {
            get { return _areaWidth; }
            set
            {
                _areaWidth = value;
                OnPropertyChanged("AreaWidth");
            }
        }

        #endregion

        #region Localization

        /// <summary>
        /// Searches for a DiagramObject in the observable collections of the DiagramData with the sames coordinates as the DiagramObject parameter
        /// This only makes sense if the user doesn't places two DiagramObject on the same coordinates of the Grid
        /// </summary>
        /// <param name="diagramObject">Diagram object parameter</param>
        /// <returns>Null or the DiagramObject</returns>
        internal DiagramObject GetDiagramObject(DiagramObject diagramObject)
        {
            
            if (diagramObject == null) return null;
            DiagramObject diag;
            diag = Nodes.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = InterNodes.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = Transitions.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = Inputs.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = Outputs.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = NotInputs.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);
            if (diag != null) return diag;
            diag = NotOutputs.ToList().Find(e => e != null && !e.IsNew && e.X == diagramObject.X && e.Y == diagramObject.Y);

            
            return diag;

        }

        #endregion

        #region Removing Connections

        /// <summary>
        /// Removes a Connection that contains the connector parameter
        /// </summary>
        /// <param name="connector">The connector parameter</param>
        internal void RemoveConnectionByConnector(Connector connector)
        {
            if (Connections.Count != 0)
            {
                Connection connectionToRemove = Connections.First(x => x.Connectors.Contains(connector));
                if (connectionToRemove != null)
                {
                    RemoveConnection(connectionToRemove);
                }
            }
        }

        /// <summary>
        /// Removes a Connection that contains the interNode parameter
        /// </summary>
        /// <param name="interNode">The interNode parameter</param>
        private void RemoveConnectionByInterNode(InterNode interNode)
        {
            if (Connections.Count != 0)
            {
                Connection connectionToRemove = Connections.First(x => x.InterNodes.Contains(interNode));
                if (connectionToRemove != null)
                {
                    RemoveConnection(connectionToRemove);
                }
            }
        }
        /// <summary>
        /// Deletes the connection by deleting all its Connectors, InterNodes
        /// and also removing the Logic References (Previous/Next) from the respective connection's LogicElements : Start and End
        /// </summary>
        /// <param name="connection"></param>
        internal void RemoveConnection(Connection connection)
        {
            if (connection != null)
            {
                connection.Connectors.ForEach(x => this.Connectors.Remove(x));
                connection.InterNodes.ForEach(x => this.InterNodes.Remove(x));
                if(connection.Start != null)(connection.Start as LogicElement).Next.Remove(connection.End as LogicElement);
                if(connection.End != null)(connection.End as LogicElement).Previous.Remove(connection.Start as LogicElement);
                if (Connections.Contains(connection)) Connections.Remove(connection);
            }
        }
        #endregion
    }
}
