using System.Collections.Generic;

namespace WHR930.Commands
{
    public class SetVentilationLevelCommand : SerialCommand
    {
        public SetVentilationLevelCommand() : base(0x00, 0xCF, 15, 45, 75, 15, 45, 75, 100, 100, 1, 1) {}

        public byte ExhaustAbsent {
            get => DataBytes[0];
            set => DataBytes[0] = value;
        }

        public byte ExhaustLow {
            get => DataBytes[1];
            set => DataBytes[1] = value;
        }

        public byte ExhaustMedium {
            get => DataBytes[2];
            set => DataBytes[2] = value;
        }

        public byte ExhaustHigh {
            get => DataBytes[6];
            set => DataBytes[6] = value;
        }

        public byte SupplyAbsent {
            get => DataBytes[3];
            set => DataBytes[3] = value;
        }

        public byte SupplyLow {
            get => DataBytes[4];
            set => DataBytes[4] = value;
        }

        public byte SupplyMedium {
            get => DataBytes[5];
            set => DataBytes[5] = value;
        }

        public byte SupplyHigh {
            get => DataBytes[7];
            set => DataBytes[7] = value;
        }
    }
}