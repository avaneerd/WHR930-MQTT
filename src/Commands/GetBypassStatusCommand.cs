namespace WHR930.Commands
{
    public class GetBypassStatusCommand : SerialCommand
    {
        public GetBypassStatusCommand() : base(0x00, 0x0D) {}

        [Sensor("Bypass percentage", UnitOfMeasurement = "%")]
        public byte BypassPercentage
        {
            get => DataBytes[0];
        }

        [Sensor("Preheating status")]
        public string PreheatingStatus
        {
            get => DataBytes[1] switch
            {
                0 => "Closed",
                1 => "Open",
                _ => "Unknown"
            };
        }

        [Sensor("Bypass motor current")]
        public byte BypassMotorCurrent
        {
            get => DataBytes[2];
        }

        [Sensor("Preheating motor current")]
        public byte PreheatingMotorCurrent
        {
            get => DataBytes[3];
        }
    }

}