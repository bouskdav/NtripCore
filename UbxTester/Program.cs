using Asv.Gnss;
using System.IO.Ports;

namespace UbxTester
{
    internal class Program
    {
        static SerialPort serialPort;

        static async Task Main()
        {
            string portName = "COM8";  // Change this to match your device
            int baudRate = 115200; // Default for ZED-F9P

            var device = new UbxDevice($"serial:{portName}?br={baudRate}");
            await device.SetupByDefault();
            //await device.SetMessageRate<RtcmV3Message1004>();
            await device.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2);
            //device.Connection.Filter<RtcmV3Msm4>().Subscribe(_ => { /* do something with RTCM */ });
            device.Connection.Filter<RtcmV3Msm4>().Subscribe(i =>
            {
                Console.WriteLine(i);
                return;
            });

            while (true)
            {
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    break;
                }
            }

            //serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            //serialPort.DataReceived += SerialPort_DataReceived;
            //serialPort.Open();

            ////byte[] checkTmode3 = new byte[]
            ////{
            ////    0xB5, 0x62,  // UBX Header
            ////    0x06, 0x71,  // Class & ID (CFG-TMODE3)
            ////    0x00, 0x00,  // Payload length (0 bytes)
            ////    0x00, 0x00   // Checksum (will be calculated)
            ////};
            ////AppendUBXChecksum(checkTmode3);
            ////serialPort.Write(checkTmode3, 0, checkTmode3.Length);

            ////// Enable Survey-In Mode (Min time: 60s, Accuracy: 5m)
            ////byte[] surveyInCommand = new byte[]
            ////{
            ////    0xB5, 0x62,  // UBX Header
            ////    0x06, 0x71,  // Class & ID (CFG-TMODE3)
            ////    0x20, 0x00,  // Payload length (32 bytes)
            ////    0x01, 0x00, 0x00, 0x00,  // Mode = 1 (Survey-In)
            ////    0x00, 0x00, 0x00, 0x00,  // Reserved
            ////    0x3C, 0x00, 0x00, 0x00,  // Minimum time (60 sec)
            ////    0x00, 0x00, 0xA0, 0x40,  // Required accuracy (5m = 5.0e6 mm)
            ////    0x00, 0x00, 0x00, 0x00,  // Reserved
            ////    0x00, 0x00, 0x00, 0x00,  // Reserved
            ////    0x00, 0x00, 0x00, 0x00,  // Reserved
            ////    0x00, 0x00  // Checksum (will be calculated)
            ////};

            ////AppendUBXChecksum(surveyInCommand);
            ////serialPort.Write(surveyInCommand, 0, surveyInCommand.Length);

            ////Console.WriteLine("Survey-In mode command sent. Monitoring progress...");

            //Console.WriteLine("Type 'CFG-TMODE3 1' to enable Survey-In mode.");
            //Console.WriteLine("Type 'CFG-TMODE3 0' to disable TMODE3.");
            //Console.WriteLine("Type 'QUERY-TMODE3'.");
            //Console.WriteLine("Type 'exit' to quit.");

            //while (true)
            //{
            //    string input = Console.ReadLine();

            //    if (input.ToUpper() == "CFG-TMODE3 1")
            //    {
            //        SendSurveyInCommand(true);
            //    }
            //    else if (input.ToUpper() == "CFG-TMODE3 0")
            //    {
            //        SendSurveyInCommand(false);
            //    }
            //    else if (input.ToUpper() == "QUERY-TMODE3")
            //    {
            //        SendQueryTmode3Command();
            //    }
            //    else if (input.ToLower() == "exit")
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Invalid command. Use 'CFG-TMODE3 1' or 'CFG-TMODE3 0'.");
            //    }
            //}

            //Console.ReadLine();
            //serialPort.Close();

            ////// Request for NAV-PVT (Position, Velocity, Time)
            ////byte[] navPvtRequest = new byte[]
            ////{
            ////    0xB5, 0x62,  // UBX Header
            ////    0x01, 0x07,  // Class & ID (NAV-PVT)
            ////    0x00, 0x00,  // Payload length (0 bytes)
            ////    0x00, 0x00   // Checksum (will be calculated)
            ////};

            ////AppendUBXChecksum(navPvtRequest);
            ////serialPort.Write(navPvtRequest, 0, navPvtRequest.Length);

            ////Console.WriteLine("Requesting NAV-PVT data...");

            ////// Request for NAV-SAT (Satellite status)
            ////byte[] navSatRequest = new byte[]
            ////{
            ////    0xB5, 0x62,  // UBX Header
            ////    0x01, 0x35,  // Class & ID (NAV-SAT)
            ////    0x00, 0x00,  // Payload length (0 bytes)
            ////    0x00, 0x00   // Checksum (will be calculated)
            ////};

            ////AppendUBXChecksum(navSatRequest);
            ////serialPort.Write(navSatRequest, 0, navSatRequest.Length);

            ////Console.WriteLine("Requesting NAV-SAT data...");

            ////Console.ReadLine();
            ////serialPort.Close();
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            if (buffer.Length < 8 || buffer[0] != 0xB5 || buffer[1] != 0x62)
            {
                //Console.WriteLine("Invalid UBX message received.");
                return;
            }

            int msgClass = buffer[2];
            int msgID = buffer[3];

            //Console.WriteLine($"UBX Message Received: Class 0x{msgClass:X2}, ID 0x{msgID:X2}");

            switch ((msgClass, msgID))
            {
                case (0x01, 0x07): // UBX-NAV-PVT (Position, Velocity, Time)
                    //ParseNavPvt(buffer);
                    break;
                case (0x01, 0x3B): // UBX-NAV-SVIN (Survey-In Status)
                    ParseNavSvin(buffer);
                    break;
                case (0x06, 0x71): // UBX-CFG-TMODE3 (Time Mode)
                    ParseCfgTmode3(buffer);
                    break;
                case (0x05, 0x01): // UBX-ACK-ACK
                    Console.WriteLine("Received ACK for previous command.");
                    break;
                case (0x05, 0x00): // UBX-ACK-NAK
                    Console.WriteLine("Received NAK (Command not accepted).");
                    break;
                default:
                    //Console.WriteLine("Unknown UBX message.");
                    break;
            }
        }

        #region message parsing

        private static void ParseNavPvt(byte[] message)
        {
            if (message.Length < 92) // Ensure message is long enough
            {
                Console.WriteLine("Invalid NAV-PVT message size.");
                return;
            }

            ushort year = BitConverter.ToUInt16(message, 6);
            byte month = message[8];
            byte day = message[9];
            byte hour = message[10];
            byte minute = message[11];
            byte second = message[12];

            byte fixType = message[26]; // GNSS fix type
            double lat = BitConverter.ToInt32(message, 30) / 1e7; // Convert to degrees
            double lon = BitConverter.ToInt32(message, 34) / 1e7;
            double alt = BitConverter.ToInt32(message, 38) / 1e3; // Convert to meters

            Console.WriteLine($"[NAV-PVT] Date: {year}-{month:D2}-{day:D2} {hour:D2}:{minute:D2}:{second:D2}");
            Console.WriteLine($"Fix Type: {fixType} (1=No fix, 2=2D, 3=3D, 4=GNSS+RTK)");
            Console.WriteLine($"Latitude: {lat}°");
            Console.WriteLine($"Longitude: {lon}°");
            Console.WriteLine($"Altitude: {alt}m");
        }

        private static void ParseNavSvin(byte[] message)
        {
            int duration = BitConverter.ToInt32(message, 8);
            int accuracy = BitConverter.ToInt32(message, 12);
            bool valid = message[20] == 1;

            Console.WriteLine($"[NAV-SVIN] Survey Duration: {duration} sec");
            Console.WriteLine($"Survey Accuracy: {accuracy / 1000.0} m");
            Console.WriteLine($"Survey Complete: {(valid ? "Yes" : "No")}");
        }

        private static void ParseCfgTmode3(byte[] message)
        {
            Console.WriteLine("CFG-TMODE3");
            Console.WriteLine(BitConverter.ToString(message));

            int mode = message[8];
            uint minTime = BitConverter.ToUInt32(message, 8);
            uint accuracy = BitConverter.ToUInt32(message, 12);

            Console.WriteLine($"[CFG-TMODE3] Mode: {mode} ({GetModeDescription(mode)})");
            Console.WriteLine($"Minimum Time: {minTime} sec");
            Console.WriteLine($"Required Accuracy: {accuracy / 1000.0} m");
        }

        private static string GetModeDescription(int mode)
        {
            return mode switch
            {
                0 => "Disabled",
                1 => "Survey-In",
                2 => "Fixed Position",
                _ => "Unknown"
            };
        }

        private static void ParseNavSat(byte[] message)
        {
            // Extracting satellite status from NAV-SAT message
            byte numOfSatellites = message[6];
            Console.WriteLine($"Number of satellites visible: {numOfSatellites}");

            // Example: Iterate through satellite data and display satellite ID, signal strength, etc.
            for (int i = 0; i < numOfSatellites; i++)
            {
                byte satelliteID = message[7 + i * 12];
                byte signalStrength = message[8 + i * 12];
                Console.WriteLine($"Satellite {satelliteID}: Signal Strength = {signalStrength}");
            }
        }

        #endregion

        #region send commands

        private static void SendQueryTmode3Command()
        {
            byte[] command = new byte[]
            {
                0xB5, 0x62,  // UBX Header
                0x06, 0x71,  // Class & ID (CFG-TMODE3)
                0x00, 0x00,  // Payload length (0 bytes)
                0x00, 0x00   // Checksum (Calculated manually)
            };
            //byte[] command = new byte[]
            //{
            //    0xB5, 0x62,  // UBX Header
            //    0x01, 0x3B,  // Class & ID (CFG-TMODE3)
            //    0x00, 0x00,  // Payload length (0 bytes)
            //    0x00, 0x00   // Checksum (Calculated manually)
            //};

            AppendUBXChecksum(command);

            Console.WriteLine(BitConverter.ToString(command));

            serialPort.Write(command, 0, command.Length);
        }

        private static void SendSurveyInCommand(bool enable)
        {
            byte[] command;

            if (enable)
            {
                // Enable Survey-In Mode (300 sec, 2m accuracy)
                command = new byte[]
                {
                    0xB5, 0x62,  // UBX Header
                    0x06, 0x71,  // Class & ID (CFG-TMODE3)
                    0x20, 0x00,  // Payload length (32 bytes)
                    0x01, 0x00, 0x00, 0x00,  // Mode = 1 (Survey-In)
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x2C, 0x01, 0x00, 0x00,  // Minimum time (300 sec)
                    0x00, 0x00, 0x20, 0x3E,  // Required accuracy (2m = 2.0e6 mm)
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00  // Checksum (Calculated later)
                };
                Console.WriteLine("Enabling Survey-In mode...");
            }
            else
            {
                // Disable TMODE3 (Set Mode = 0)
                command = new byte[]
                {
                    0xB5, 0x62,  // UBX Header
                    0x06, 0x71,  // Class & ID (CFG-TMODE3)
                    0x20, 0x00,  // Payload length (32 bytes)
                    0x00, 0x00, 0x00, 0x00,  // Mode = 0 (Disabled)
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00, 0x00, 0x00,  // Minimum time (ignored)
                    0x00, 0x00, 0x00, 0x00,  // Required accuracy (ignored)
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00, 0x00, 0x00,  // Reserved
                    0x00, 0x00  // Checksum (Calculated later)
                };
                Console.WriteLine("Disabling TMODE3...");
            }

            AppendUBXChecksum(command);
            serialPort.Write(command, 0, command.Length);
        }

        #endregion

        private static void AppendUBXChecksum(byte[] message)
        {
            int length = message.Length;
            byte ckA = 0, ckB = 0;
            for (int i = 2; i < length - 2; i++)
            {
                ckA += message[i];
                ckB += ckA;
            }
            message[length - 2] = ckA;
            message[length - 1] = ckB;
        }
    }
}
