using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ball_in_a_maze
{
    public class MotionData
    {
        // ------------------------------------------------------------------
        //                          EVENTS
        // ------------------------------------------------------------------

        // Event that is called if new Data is available
        public event EventHandler NewDataAvailable;
        public event EventHandler StartUpFailed;
        public event EventHandler StartUpSuccessfull;
        public event EventHandler DataError;

        // ------------------------------------------------------------------
        //                          PROPERTIES
        // ------------------------------------------------------------------

        // Motion Data: 3 axis: X, Y and Z
        public int Axis_X { get; private set; }
        public int Axis_Y { get; private set; }
        public int Axis_Z { get; private set; }

        // ------------------------------------------------------------------
        //                      PRIVATE MEMBERS
        // ------------------------------------------------------------------

        // Serial Port
        private SerialPort mPort;

        // Private Property is used everytime the SerialPort is accessed
        // This prevents Race Condition from accessing it asynchronously via Events
        private SerialPort Port {
            get
            {
                // to prevent Race Condition with Asynchronous Events from ViewModel

                // TODO: instead of LOCKING --> when any EVENT called "ClosePort" --> no further access to mPort
                lock(PortLock)
                {
                    return mPort;
                }
            }
            set
            {
                mPort = value;
            }
        }


        // Connected FLAG
        private bool IsConnected = false;

        // Flag that is used by TIMER to ensure that board is still sending data
        private bool NewData = false;

        // Timer that is used to ensure that board is still connected and sending data
        private Timer NewDataTimer = null;

        // Start Condition that is sent to Board at startup
        private readonly byte[] StartCondition = new byte[2] { 0xAF, 0xFE };

        // Path to LOG file for ERROR messages
        private readonly string LogFile_Path = System.AppDomain.CurrentDomain.BaseDirectory + "BallInAMaze.log";

        // Mutex used to lock access to SerialPort to prevent Race Conditions
        private readonly object PortLock = new object();

        // ------------------------------------------------------------------
        //                      PUBLIC METHODS
        // ------------------------------------------------------------------

        /// <summary>
        /// Method to send message to board to RESET it and CLOSE PORT
        /// </summary>
        public void ClosePort(bool SenStopCMD = true)
        {
            try
            {
                // Send command to board to RESET it IF requested
                if (SenStopCMD)
                {
                    byte[] Stop = new byte[1] { 0x01 };
                    Port.Write(Stop, 0, 1);
                }
                // RESET flag
                IsConnected = false;

                // Close PORT --> dispose all ressources
                Port.Close();
            }
            catch (Exception)
            {
                // OK
            }
        }

        /// <summary>
        /// Method can be called to establish a Connection to motion board
        /// </summary>
        /// <param name="PortNum">Index of COM Port in "GetPortNames()" array</param>
        public void TryConnect(int PortNum)
        {
            // Check given Port Number
            if ((PortNum < 0) || (PortNum > SerialPort.GetPortNames().Length - 1))
            {
                _ERROR(StartUpFailed, "ERROR: Wrong port number in MotionData.cs:TryConnect!");
                return;
            }

            // Create NEW SERIAL PORT
            mPort = new SerialPort(SerialPort.GetPortNames()[PortNum], 115200, Parity.None, 8, StopBits.One);

            // Attach a method to be called when there
            // is data waiting in the port's buffer
            Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived_Handler);
            Port.ReadTimeout = 10000;  // Exception if no data is received after 10 sec
            Port.WriteTimeout = 10;

            try
            {
                // OPEN PORT
                Port.Open();

                // CLEAR IN/OUT Buffer
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
            }
            catch (Exception)
            {
                _ERROR(StartUpFailed, "ERROR: could not open PORT!");
                return;
            }

            // Firstly: send port START condition
            try
            {
                Port.Write(StartCondition, 0, 2);
            }
            catch (Exception)
            {
                _ERROR(StartUpFailed, "ERROR: could not send START condition!");
                return;
            }
        }

        // ------------------------------------------------------------------
        //                              CTOR
        // ------------------------------------------------------------------

        public MotionData()
        {
            // Delete old LOG files
            // Catch Exceptions from this Method, but we dont care!
            try
            {
                System.IO.File.Delete(LogFile_Path);
            }
            catch (Exception)
            {
                // Do nothing!
            }
        }

        // ------------------------------------------------------------------
        //                        PRIVATE METHODS
        // ------------------------------------------------------------------

        /// <summary>
        /// THIS method can be exchanged if we are not longer using a CONSOLE application BUT a real UI application
        /// </summary>
        /// <param name="info">String that is printed into LOG file</param>
        private void PrintInformation(string info)
        {
            // Write Message to LOG FILE
            System.IO.File.AppendAllText(LogFile_Path, info + '\n');
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
                HandleACK();  

            // Read out RAW motion Data
            var data = new byte[12];
            BlockingRead(data, 0, 12);

            // Convert to actual values
            Axis_X = BitConverter.ToInt32(data, 0);
            Axis_Y = BitConverter.ToInt32(data, 4);
            Axis_Z = BitConverter.ToInt32(data, 8);

            // Inform User that Data is now available
            NewDataAvailable?.Invoke(this, EventArgs.Empty);

            // Ensure that flag is set to let Timer know that there is new data
            NewData = true;
        }

        /// <summary>
        /// Method to handle STARTUP phase of Motion Board
        /// </summary>
        private void HandleACK()
        {
            var ack = new byte[2];
            UInt16 ack_check = BitConverter.ToUInt16(StartCondition, 0);

            // We know that we have already received data --> READ it out
            var read_ret = Port.Read(ack, 0, 2);

            // ACK must contain 2 bytes!
            if (read_ret != 2)
            {
                _ERROR(StartUpFailed, "ERROR: Did not receive correct amount of bytes for ACK!");
                return;
            }

            // Check if we received the CORRECT ACK bytes
            if (BitConverter.ToUInt16(ack, 0) != ack_check)
            {
                _ERROR(StartUpFailed, "ERROR: Did not receive ACK from Board!");
                return;
            }

            // SET GLOBAL FLAG
            IsConnected = true;

            // Enable timer to ensure that there is new data at least every 1 sec
            NewDataTimer            = new Timer();
            NewDataTimer.Elapsed   += new ElapsedEventHandler(OnTimedEvent);
            NewDataTimer.Interval   = 1000; // 1 sec
            NewDataTimer.Enabled    = true;
            NewDataTimer.AutoReset  = true;

            // Inform ViewModel, that StartUp was successful
            StartUpSuccessfull?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method that prints message to LOG file, calls 'ClosePort()' and triggers ERROR event
        /// </summary>
        /// <param name="Error_Event">Event that is triggered</param>
        /// <param name="log">Message that is printed to LOG file</param>
        private void _ERROR(EventHandler Error_Event, string log, bool SendStopCMD = true)
        {
            // Print INFO to LOG File
            PrintInformation(log);

            // Close Connection (Port) to Board
            ClosePort(SendStopCMD);

            // Trigger Event to ViewModel
            Error_Event?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// TIMER event handler method --> to check if board is still sending data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!NewData)
            {
                // Disable Timer
                NewDataTimer.Enabled = false;
                NewDataTimer.AutoReset = false;
                NewDataTimer = null;

                // Trigger ERROR
                _ERROR(DataError, "ERRO: now data since 1 sec!", false);
            }

            // Reset Flag
            NewData = false;
        }

        /// <summary>
        /// Method that is used to READ out 'count' bytes from Serial Port
        /// </summary>
        /// <param name="buffer">Buffer to read bytes into</param>
        /// <param name="offset">Offset in 'buffer'</param>
        /// <param name="count">How many bytes to be read?</param>
        private void BlockingRead(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                // SerialPort.Read() blocks until at least one byte has been read, or SerialPort.ReadTimeout milliseconds
                // have elapsed. If a timeout occurs a TimeoutException will be thrown.
                // Because SerialPort.Read() blocks until some data is available this is not a busy loop,
                // and we do NOT need to issue any calls to Thread.Sleep().
                try
                {
                    int bytesRead = Port.Read(buffer, offset, count);
                    offset += bytesRead;
                    count -= bytesRead;
                }
                catch (Exception e)
                {
                    _ERROR(DataError, "ERROR: reading UART: " + e.Message);
                    return;
                }
            }
        }
    }
}
