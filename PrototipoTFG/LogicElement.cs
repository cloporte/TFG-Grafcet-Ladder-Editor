using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrototipoTFG
{
    /// <summary>
    /// A LogicElement is a DiagramObject with logic references as Previous and Next.
    /// </summary>
    public class LogicElement : DiagramObject
    {
        #region Constructor
        /// <summary>
        /// Default Empty Constructor of a LogicElement
        /// </summary>
        public LogicElement()
        {

        }
        #endregion

        #region Collections : Previous & Next
        private ObservableCollection<LogicElement> previous;
        /// <summary>
        /// Collection of LogicElement that represents in a logic sense the Previous Elements connected by Connections to this LogicElement.
        /// </summary>
        /// <value>LogicElement references</value>
        [XmlIgnore]
        public ObservableCollection<LogicElement> Previous
        {
            get
            {
                if (previous != null) return previous;
                previous = new ObservableCollection<LogicElement>();
                previous.CollectionChanged += Previous_CollectionChanged;
                return previous;
            }

            set { }
        }

        /// <summary>
        /// Handler that notifies a change into the Previous Collection to use Data-binding on the WPF xaml.
        /// </summary>
        /// <param name="sender">Object reference to the sender of the event</param>
        /// <param name="e">Arguments for the Event</param>
        private void Previous_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged("Previous");
        }


        private ObservableCollection<LogicElement> next;
        /// <summary>
        /// Collection of LogicElement that represents in a logic sense the Next Elements connected by Connections to this LogicElement.
        /// </summary>
        /// <value>LogicElement references</value>
        [XmlIgnore]
        public ObservableCollection<LogicElement> Next
        {
            get
            {
                if (next != null) return next;
                next = new ObservableCollection<LogicElement>();
                next.CollectionChanged += Next_CollectionChanged;
                return next;
            }

            set { }
        }

        /// <summary>
        /// Handler that notifies a change into the Next Collection to use Data-binding on the WPF xaml.
        /// </summary>
        /// <param name="sender">Object reference to the sender of the event</param>
        /// <param name="e">Arguments for the Event</param>
        private void Next_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged("Next");
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler CollectionChanged;
        
        private void OnCollectionChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = CollectionChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}

