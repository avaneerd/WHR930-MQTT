using System;
using System.Linq;

namespace WHR930.Commands
{
    public abstract class SerialCommand
    {
        public byte[] AckBytes { private set; get; }
        public byte[] StartBytes { private set; get; } = new byte[2] { 0x07, 0xF0 };
        public byte[] CommandBytes { private set; get; } = new byte[2];
        public byte DataLength { private set; get; } = 0x00;
        public byte[] DataBytes { private set; get; } = new byte[0];
        public byte Checksum { private set; get; }
        public byte[] EndBytes { private set; get; } = new byte[2] { 0x07, 0x0F };

        public SerialCommand(params byte[] bytes)
        {
            CommandBytes = new[] { bytes[0], bytes[1] };

            if (bytes.Length > 2)
            {
                DataLength = (byte)(bytes.Length - 2);
                DataBytes = bytes.Skip(2).ToArray();
            }
        }

        public void ApplyResponseAndVerify(byte[] bytes)
        {
            AckBytes = new[] { bytes[0], bytes[1] };
            StartBytes = new[] { bytes[2], bytes[3] };
            CommandBytes = new[] { bytes[4], bytes[5] };
            DataLength = bytes[6];

            if (bytes.Length > 10)
            {
                DataBytes = bytes.Skip(7).Reverse().Skip(3).Reverse().ToArray();
            }

            Checksum = bytes.Reverse().Skip(2).First();
            EndBytes = bytes.Reverse().Take(2).Reverse().ToArray();

            VerifyResponse();
        }

        private void VerifyResponse()
        {
            if (AckBytes[0] != 0x07 || AckBytes[1] != 0xF3)
                throw new InvalidOperationException("Did not receive ACK");

            if (StartBytes[0] != 0x07 || StartBytes[1] != 0xF0)
                throw new InvalidOperationException("Response did not start with 07F0");

            if (EndBytes[0] != 0x07 || EndBytes[1] != 0x0F)
                throw new InvalidOperationException("Response did not end with 070F");

            var calculatedChecksum = CalculateChecksum();

            if (Checksum != calculatedChecksum)
                throw new InvalidOperationException($"Checksum {Checksum} and calculated checksum {calculatedChecksum} did not match");
        }

        private byte CalculateChecksum()
        {
            const int checksumValue = 173;
            var filteredDataBytes = DataBytes;

            if (filteredDataBytes.Any(b => b == 0x07))
            { // For checksum calculation only one 0x07 byte can be included
                filteredDataBytes = filteredDataBytes.Where(b => b != 0x07)
                    .Append<byte>(0x07)
                    .ToArray();
            }

            var allBytes = CommandBytes.Append(DataLength).Concat(filteredDataBytes);

            var checksum = allBytes.Aggregate(checksumValue, (acc, val) => acc + val);

            return (byte)(checksum & 0xFF);
        }

        public byte[] GetAllBytes()
        {
            return StartBytes
                .Concat(CommandBytes)
                .Append(DataLength)
                .Concat(DataBytes)
                .Append(CalculateChecksum())
                .Concat(EndBytes)
                .ToArray();
        }
    }
}