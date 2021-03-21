using System.Collections.Generic;

namespace WHR930.Commands
{
    public class GetTemperatureCommand : SerialCommand
    {
        public GetTemperatureCommand() : base(0x00, 0xD1) {}

        [Sensor("Comfort temperature", DeviceClass = "temperature", UnitOfMeasurement = "°C")]
        public double ComfortTemperature
        {
            get => DataBytes[0] / 2d - 20;
        }

        [Sensor("Outside air temperature", DeviceClass = "temperature", UnitOfMeasurement = "°C")]
        public double OutsideAirTemperature
        {
            get => DataBytes[1] / 2d - 20;
        }

        [Sensor("Supply air temperature", DeviceClass = "temperature", UnitOfMeasurement = "°C")]
        public double SupplyAirTemperature
        {
            get => DataBytes[2] / 2d - 20;
        }

        [Sensor("Return air temperature", DeviceClass = "temperature", UnitOfMeasurement = "°C")]
        public double ReturnAirTemperature
        {
            get => DataBytes[3] / 2d - 20;
        }

        [Sensor("Exhaust air temperature", DeviceClass = "temperature", UnitOfMeasurement = "°C")]
        public double ExhaustAirTemperature
        {
            get => DataBytes[4] / 2d - 20;
        }

        [Sensor("Present Sensors")]
        public string PresentSensors
        {
            get {
                var presentSensors = new List<string>();

                if ((DataBytes[5] & 0x01) == 0x01)
                    presentSensors.Add("T1 / outside air");

                if ((DataBytes[5] & 0x02) == 0x02)
                    presentSensors.Add("T2 / supply air");

                if ((DataBytes[5] & 0x04) == 0x04)
                    presentSensors.Add("T3 / return air");

                if ((DataBytes[5] & 0x08) == 0x08)
                    presentSensors.Add("T4 / exhaust air");

                if ((DataBytes[5] & 0x010) == 0x10)
                    presentSensors.Add("EWT");

                if ((DataBytes[5] & 0x20) == 0x20)
                    presentSensors.Add("Post heating");

                if ((DataBytes[5] & 0x40) == 0x40)
                    presentSensors.Add("Kitchen hood");

                return string.Join(", ", presentSensors);
            }
        }
    }
}