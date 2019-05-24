using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    class ActiveCOMPorts : INotifyPropertyChanged
    {
        public ActiveCOMPorts()
        {
            Worker = new BackgroundWorker();
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += OnWorkerDoWork;
            Worker.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnConnectionestablished(object sender, EventArgs e)
        {
            Worker.CancelAsync();
        }

        private void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(1000);
                Ports = SerialPort.GetPortNames();

                if (Worker.CancellationPending)
                    return;
            }
        }

        private string[] ports;

        public string[] Ports
        {
            get { return ports; }
            set {
                ports = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Ports"));
            }
        }


        public BackgroundWorker Worker { get; set; }
    }
}
