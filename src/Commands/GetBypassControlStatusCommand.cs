namespace WHR930.Commands
{
    public class GetBypassControlStatusCommand : SerialCommand
    {
        public GetBypassControlStatusCommand() : base(0x00, 0xDF) {}

        [Sensor("Bypass factor")]
        public byte BypassFactor
        {
            get => DataBytes[2];
        }

        [Sensor("Bypass stage")]
        public byte BypassStage
        {
            get => DataBytes[3];
        }

        [Sensor("Bypass correction")]
        public byte BypassCorrection
        {
            get => DataBytes[4];
        }

        [Sensor("Summer mode")]
        public string SummerMode
        {
            get => DataBytes[6] switch
            {
                0 => "No",
                1 => "Yes",
                _ => "Unknown"
            };
        }
    }
}