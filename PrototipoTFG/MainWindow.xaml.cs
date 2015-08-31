using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PrototipoTFG;

namespace PrototipoTFG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }

        private void Thumb_Drag(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null) return;

            var node = thumb.DataContext as Node;
            if (node == null) return;

            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
        }

        private void Thumb_DragTransitions(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null) return;

            var transition = thumb.DataContext as Transition;
            if (transition == null) return;

            transition.X += e.HorizontalChange;
            transition.Y += e.VerticalChange;
        }

        private void Thumb_DragInterNodes(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var interNode = thumb.DataContext as InterNode;
            if (interNode == null)
                return;

            interNode.X += e.HorizontalChange;
            interNode.Y += e.VerticalChange;
        }

        private void Thumb_DragInputs(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var input = thumb.DataContext as InputLadder;
            if (input == null)
                return;

            input.X += e.HorizontalChange;
            input.Y += e.VerticalChange;
        }

        private void Thumb_DragOutputs(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var output = thumb.DataContext as OutputLadder;
            if (output == null)
                return;

            output.X += e.HorizontalChange;
            output.Y += e.VerticalChange;
        }


        private void Thumb_DragNotInputs(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var input = thumb.DataContext as NotInputLadder;
            if (input == null)
                return;

            input.X += e.HorizontalChange;
            input.Y += e.VerticalChange;
        }

        private void Thumb_DragNotOutputs(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var output = thumb.DataContext as NotOutputLadder;
            if (output == null)
                return;

            output.X += e.HorizontalChange;
            output.Y += e.VerticalChange;
        }




        private void ListBox_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var listbox = sender as ListBox;

            if (listbox == null)
                return;

            var vm = listbox.DataContext as MainViewModel;

            if (vm == null)
                return;

            if (vm.SelectedObject != null && (vm.SelectedObject is Node || vm.SelectedObject is Transition || vm.SelectedObject is InterNode ||
                vm.SelectedObject is InputLadder || vm.SelectedObject is OutputLadder || vm.SelectedObject is NotInputLadder || vm.SelectedObject is NotOutputLadder) && vm.SelectedObject.IsNew)
            {
                vm.SelectedObject.X = e.GetPosition(listbox).X;
                vm.SelectedObject.Y = e.GetPosition(listbox).Y;

                if (vm.SelectedObject is InterNode)
                {
                    var diagramObject = GetDiagramObjectUnderMouse();
                    if (diagramObject != null && vm.interConnector != null)
                    {
                        vm.interConnector.End = diagramObject;
                    }
                    else
                    {
                        if (vm.interConnector != null) vm.interConnector.End = vm.SelectedObject;
                    }
                }
            }
            else if (vm.SelectedObject != null && vm.SelectedObject is Connector && vm.SelectedObject.IsNew)
            {
                var diagramObject = GetDiagramObjectUnderMouse();
                if (diagramObject == null)
                    return;

                var connector = vm.SelectedObject as Connector;

                if (connector.Start != null && diagramObject != connector.Start)
                    connector.End = diagramObject;
            }
        }

        private void ListBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                // When creating a InterNode for a Connection
                if (vm.CreatingNewInterNode)
                {
                    var diagramObject = vm.GetDiagramObject(vm.SelectedObject) as DiagramObject;
                    var logicElement = vm.GetDiagramObject(vm.SelectedObject) as LogicElement;
                    var connector = vm.interConnector;
                    var interNode = vm.SelectedObject as InterNode;



                    // When we finish our connnection clicking on an interNode
                    if (diagramObject != null && !diagramObject.IsNew && diagramObject is InterNode)
                    {
                        //We can do that since a connection can't be connected to another connection
                        vm.RemoveConnection(vm.newConnection);
                        vm.CreatingNewInterNode = false;
                        e.Handled = true;
                        return;
                    }

                    // When we finish our connection clicking on a Logic Element
                    if (diagramObject != null && !diagramObject.IsNew && (diagramObject is Node || diagramObject is Transition
                        || diagramObject is InputLadder || diagramObject is OutputLadder || diagramObject is NotInputLadder || diagramObject is NotOutputLadder) && connector != null && connector.Start != null)                                      
                    {
                        vm.InterNodes.Remove(interNode);
                        if (vm.newConnection.Start.X == diagramObject.X && vm.newConnection.Start.Y == diagramObject.Y)
                        { // If attempting to make a connection on itself
                            vm.RemoveConnection(vm.newConnection);
                            vm.CreatingNewInterNode = false;
                            e.Handled = true;
                            return;
                        }

                        connector.End = diagramObject;
                        connector.IsNew = false;
                        vm.newConnection.Connectors.Add(connector);
                        // Making the logic connection                            
                        vm.newConnection.End = logicElement;
                        (vm.newConnection.Start as LogicElement).Next.Add(logicElement);
                        (vm.newConnection.End as LogicElement).Previous.Add((vm.newConnection.Start as LogicElement));

                        vm.CreatingNewInterNode = false;
                        // Remove failed connections
                        vm.Connections.Where(x => x.Start == null || x.End == null).ToList().ForEach(x => vm.RemoveConnection(x));
                        e.Handled = true;
                        return;

                    }
                    else
                    {
                        connector.IsNew = false;
                        vm.newConnection.Connectors.Add(connector);
                        vm.newConnection.InterNodes.Add(interNode);

                    }
                }


                if (vm.SelectedObject != null)vm.SelectedObject.IsNew = false;
                if (vm.CreatingNewInterNode) vm.interConnector.IsNew = false;
                // We continue creating interNodes
                if (!vm.CreatingNewNode && !vm.CreatingNewTransition && !vm.CreatingNewInput && !vm.CreatingNewOutput && !vm.CreatingNewNotInput && !vm.CreatingNewNotOutput && vm.CreatingNewInterNode)
                {
                    vm.CreatingNewInterNode = false;
                    vm.CreatingNewInterNode = true;
                }
                else
                {
                    vm.CreatingNewNode = false;
                    vm.CreatingNewTransition = false;
                    vm.CreatingNewInterNode = false;
                    vm.CreatingNewInput = false;
                    vm.CreatingNewOutput = false;
                    vm.CreatingNewNotInput = false;
                    vm.CreatingNewNotOutput = false;

                }
            }
        }

        private DiagramObject GetDiagramObjectUnderMouse()
        {
            var item = Mouse.DirectlyOver as ContentPresenter;
            if (item == null)
                return null;

            return item.DataContext as DiagramObject;
        }

    }
}
