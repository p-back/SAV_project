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
    class ActiveCOMPorts
    {
        public ActiveCOMPorts()
        {
            Worker = new BackgroundWorker();
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += OnWorkerDoWork;
            Worker.RunWorkerAsync();
        }

        public event EventHandler PortsUpdated;

        internal void OnConnectionEstablished(object sender, EventArgs e)
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
                if(!Equals(ports, value))
                {
                    ports = value;
                    PortsUpdated(this, EventArgs.Empty);
                }
            }
        }


        public BackgroundWorker Worker { get; set; }
    }
}
