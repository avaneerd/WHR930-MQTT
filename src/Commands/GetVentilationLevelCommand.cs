using System.Collections.Generic;

namespace WHR930.Commands
{
    public class GetVentilationLevelCommand : SerialCommand
    {
        public GetVentilationLevelCommand() : base(0x00, 0xCD) {}

        [Sensor("Current ventilation level")]
        public byte CurrentVentilationLevel
        {
            get => DataBytes[8];
        }

        [Sensor("Supply fan active")]
        public string SupplyFanActive
        {
            get => DataBytes[9] switch {
                1 => "Active",
                0 => "Inactive",
                _ => "Unknown"
            };
        }
    }
}