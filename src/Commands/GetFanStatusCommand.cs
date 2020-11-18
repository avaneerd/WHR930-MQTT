namespace WHR930.Commands
{
    public class GetFanStatusCommand : SerialCommand
    {
        public GetFanStatusCommand() : base(0x00, 0x0B) {}

        [Sensor("Supply fan speed", UnitOfMeasurement = "%")]
        public byte SupplyFanSpeed
        {
            get => DataBytes[0];
        }

        [Sensor("Exhaust fan speed", UnitOfMeasurement = "%")]
        public byte ExhaustFanSpeed
        {
            get => DataBytes[1];
        }

        [Sensor("Supply fan RPM", UnitOfMeasurement = "RPM")]
        public int SupplyFanRPM
        {
            get => 1875000 / (DataBytes[2] * 256 + DataBytes[3]);
        }

        [Sensor("Exhaust fan RPM", UnitOfMeasurement = "RPM")]
        public int ExhaustFanRPM
        {
            get => 1875000 / (DataBytes[4] * 256 + DataBytes[5]);
        }
    }

}