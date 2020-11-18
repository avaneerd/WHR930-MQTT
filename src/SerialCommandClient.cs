using System;
using System.Linq;
using System.IO.Ports;
using WHR930.Commands;
using System.Threading;
using System.Collections.Generic;

namespace WHR930
{
    public class SerialCommandClient : IDisposable
    {
        object lockObj = new object();
        readonly SerialPort serialPort;

        public SerialCommandClient()
        {
            serialPort = new SerialPort("/dev/serial0", 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
        }

        public void SendCommandWithResponse(SerialCommand command)
        {
            if (!serialPort.IsOpen)
                throw new InvalidOperationException("Serial port is not open");

            var bytesToWrite = command.GetAllBytes();

            var responseBuffer = WriteBytesAndGetResponse(bytesToWrite);

            command.ApplyResponseAndVerify(responseBuffer);
        }

        public void SendCommandWithAck(SerialCommand command)
        {
            if (!serialPort.IsOpen)
                throw new InvalidOperationException("Serial port is not open");

            var bytesToWrite = command.GetAllBytes();

            var response = WriteBytesAndGetResponse(bytesToWrite);

            if (response[0] != 0x07 || response[1] != 0xF3)
                throw new InvalidOperationException("Expected ACK response, got something else");
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }

        private byte[] WriteBytesAndGetResponse(byte[] bytesToWrite) {
            lock (lockObj) {
                LogBytes("Writing: ", bytesToWrite);

                serialPort.DiscardInBuffer();

                serialPort.Write(bytesToWrite, 0, bytesToWrite.Length);

                Thread.Sleep(50);

                var response = new List<byte>();

                while (serialPort.BytesToRead > 0) {
                    response.Add((byte)serialPort.ReadByte());
                }

                LogBytes("Read: ", response);

                return response.ToArray();
            }
        }

        private void LogBytes(string prefix, IEnumerable<byte> bytesToLog)
        {
            var hexString = string.Join(' ', bytesToLog.Select(b => b.ToString("x2")));

            //Console.WriteLine($"{prefix}{hexString}");
        }
    }
}