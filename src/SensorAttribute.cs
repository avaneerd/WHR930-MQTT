using System;
using System.Text.RegularExpressions;

namespace WHR930 {
    public class SensorAttribute: Attribute {
        public SensorAttribute(string sensorName)
        {
            SensorName = "WHR930 " + sensorName;
        }

        public string SensorName { get; }
        public string DeviceClass { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Icon { get; set; }
        public string UniqueID {
            get {
                var regex = new Regex("[^a-z0-9]");
                return regex.Replace(SensorName.ToLower(), "-");
            }
        }
    }
}