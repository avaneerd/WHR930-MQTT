using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WHR930.Commands;
using WHR930.MessageHandlers;

namespace WHR930
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            using var serialClient = new SerialCommandClient();
            using var mqttClient = new MqttClient(
                Environment.GetEnvironmentVariable("MQTT_CLIENT_ID"),
                Environment.GetEnvironmentVariable("MQTT_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("MQTT_PORT")));

            RegisterSensors(mqttClient);

            mqttClient.RegisterMessageHandler(new SetVentilationLevelMessageHandler(serialClient));
            mqttClient.RegisterMessageHandler(new SetConfortTemperatureMessageHandler(serialClient));

            await mqttClient.ConnectClientAsync();

            while (true)
            {
                try {
                    var sensorState = new JObject();

                    foreach (var cmd in getCommands())
                    {
                        serialClient.SendCommandWithResponse(cmd);

                        foreach (var prop in cmd.GetType().GetProperties())
                        {
                            if (Attribute.IsDefined(prop, typeof(SensorAttribute)))
                            {
                                var sensor = Attribute.GetCustomAttribute(prop, typeof(SensorAttribute)) as SensorAttribute;

                                sensorState.Add(sensor.UniqueID, new JValue(prop.GetValue(cmd)));
                            }
                        }
                    }

                    await mqttClient.PublishSensorStateAsync(sensorState.ToString());
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync(ex.ToString());
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        private static void RegisterSensors(MqttClient mqttClient)
        {
            foreach (var cmd in getCommands())
            {
                foreach (var prop in cmd.GetType().GetProperties())
                {
                    if (Attribute.IsDefined(prop, typeof(SensorAttribute)))
                    {
                        var sensor = Attribute.GetCustomAttribute(prop, typeof(SensorAttribute)) as SensorAttribute;

                        mqttClient.RegisterSensor(sensor);
                    }
                }
            }
        }

        private static SerialCommand[] getCommands() => new SerialCommand[] {
                new GetBypassStatusCommand(),
                new GetBypassControlStatusCommand(),
                new GetFanStatusCommand(),
                new GetTemperatureCommand(),
                new GetVentilationLevelCommand()
            };
    }
}
