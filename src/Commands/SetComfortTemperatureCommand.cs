using System.Collections.Generic;
using System;

namespace WHR930.Commands
{
    public class SetComfortTemperatureCommand : SerialCommand
    {
        public SetComfortTemperatureCommand() : base(0x00, 0xD3, 80) {}

        public double ComfortTemperature {
            get => DataBytes[0] / 2d - 20;
            set => DataBytes[0] = (byte)((value + 20) * 2);
        }
    }
}