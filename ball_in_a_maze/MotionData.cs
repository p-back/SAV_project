using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ball_in_a_maze
{
    public class MotionData
    {
        // Event that is called if new Data is available
        public event EventHandler NewDataAvailable;
        public event EventHandler StartUpFailed;
        public event EventHandler DataError;

        // Motion Data: 3 axis: X, Y and Z
        public int Axis_X { get; private set; }
        public int Axis_Y { get; private set; }
        public int Axis_Z { get; private set; }

        // ------------------------------------------------------------------
        //      PRIVATE MEMBERS
        // ------------------------------------------------------------------

        // Serial Port
        private SerialPort mPort;

        // Connected FLAG
        private bool IsConnected = false;

        // Start Condition that is sent to Board at startup
        private readonly byte[] StartCondition = new byte[2] { 0xAF, 0xFE };

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="newDataEvent"></param>
        public MotionData(int PortNumber)
        {
            // TODO: remove this --> the correct port number is given in as parameter

            // Open Serial Port to read from Board
            Console.WriteLine("Available COM Ports: {0}", SerialPort.GetPortNames().Length);
            int i = 1;
            foreach (var item in SerialPort.GetPortNames())
            {
                Console.WriteLine("[{0}] Port Name: {1}", i++, item);
            }

            // Ask user to SELECT correct PORT
            Console.WriteLine("Select Port number and press ENTER!");
            var input = Console.ReadLine();
            int portnum = 0;
            if (!int.TryParse(input, out portnum) || (portnum > SerialPort.GetPortNames().Length))
            {
                Console.WriteLine("ERROR: wrong PORT number!");
                return;
            }

            // Create NEW SERIAL PORT
            mPort = new SerialPort(SerialPort.GetPortNames()[portnum - 1], 115200, Parity.None, 8, StopBits.One);
            //mPort = new SerialPort(SerialPort.GetPortNames()[PortNumber - 1], 115200, Parity.None, 8, StopBits.One);

            // Attach a method to be called when there
            // is data waiting in the port's buffer
            mPort.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived_Handler);
            mPort.ReadTimeout = 10000;  // Exception if now data is received after 10 sec

            // Begin communications
            try {
                mPort.Open();
            }
            catch (Exception)
            {
                PrintInformation("ERROR: could not open PORT!");
                StartUpFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Check if port is really OPEN
            if (!mPort.IsOpen)
            {
                PrintInformation("ERROR: could not open PORT!");
                StartUpFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Firstly: send port START condition
            try {
                mPort.Write(StartCondition, 0, 2);
            }
            catch (Exception)
            {
                PrintInformation("ERROR: could not send START condition!");
                StartUpFailed?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        // THIS method can be exchanged if we are not longer
        // using a CONSOLE application BUT a real UI application
        public void PrintInformation(string info, params object[] obj)
        {
            Console.WriteLine(info, obj);
        }

        /// <summary>
        /// Handler that is called everytime UART (Serial) data is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived_Handler(object sender, SerialDataReceivedEventArgs e)
        {
            // If we did not receive the ACK bytes YET --> check if got them now
            if (!IsConnected)
            {
                HandleACK();  
            }

            // Read out RAW motion Data
            var data = new byte[12];
            BlockingRead(data, 0, 12);

            // Convert to actual values
            Axis_X = BitConverter.ToInt32(data, 0);
            Axis_Y = BitConverter.ToInt32(data, 4);
            Axis_Z = BitConverter.ToInt32(data, 8);

            // Inform User that Data is now available
            NewDataAvailable?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method to handle STARTUP phase of Motion Board
        /// </summary>
        private void HandleACK()
        {
            var ack = new byte[2];
            UInt16 ack_check = BitConverter.ToUInt16(StartCondition, 0);

            // We know that we have already received data --> READ it out
            var read_ret = mPort.Read(ack, 0, 2);

            // ACK must contain 2 bytes!
            if (read_ret != 2)
            {
                PrintInformation("ERROR: Did not receive correct amount of bytes for ACK!");
                StartUpFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Check if we received the CORRECT ACK bytes
            if (BitConverter.ToUInt16(ack, 0) != ack_check)
            {
                PrintInformation("ERROR: Did not receive ACK from Board!");
                StartUpFailed?.Invoke(this, EventArgs.Empty);
                return;
            }

            // SET GLOBAL FLAG
            IsConnected = true;
            return;
        }

        public void BlockingRead(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                // SerialPort.Read() blocks until at least one byte has been read, or SerialPort.ReadTimeout milliseconds
                // have elapsed. If a timeout occurs a TimeoutException will be thrown.
                // Because SerialPort.Read() blocks until some data is available this is not a busy loop,
                // and we do NOT need to issue any calls to Thread.Sleep().
                try
                {
                    int bytesRead = mPort.Read(buffer, offset, count);
                    offset += bytesRead;
                    count -= bytesRead;
                }
                catch (Exception e)
                {
                    PrintInformation("ERROR: reading UART: {0}", e.Message);
                    DataError?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
    }
}
