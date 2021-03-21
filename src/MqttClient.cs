using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WHR930.MessageHandlers;

namespace WHR930
{
    public class MqttClient: IDisposable
    {
        private readonly string clientName;
        private readonly string host;
        private readonly int port;
        private IMqttClient client;
        private List<IMessageHandler> messageHandlers = new List<IMessageHandler>();
        private List<SensorAttribute> sensors = new List<SensorAttribute>();

        public MqttClient(string clientName, string host, int port)
        {
            this.clientName = clientName;
            this.host = host;
            this.port = port;
        }

        public async Task ConnectClientAsync()
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();

            client.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("MQTT: Disconnected");
                await Task.Delay(TimeSpan.FromSeconds(5));

                await ConnectMqttAsync();
            });

            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine($"MQTT connected");

                foreach(var mh in messageHandlers) {
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(mh.Topic).Build());
                    Console.WriteLine($"MQTT subscribed to: {mh.Topic}");
                }

                foreach (var sensor in sensors) {
                    await PublishSensorConfigAsync(sensor);
                    Console.WriteLine($"MQTT Published sensor config for: {sensor.SensorName}");
                }
            });

            client.UseApplicationMessageReceivedHandler(async (message) => {
                Console.WriteLine($"MQTT Received message on: {message.ApplicationMessage.Topic}");
                Console.WriteLine($"MQTT payload: {Encoding.UTF8.GetString(message.ApplicationMessage.Payload)}");

                var handler = messageHandlers.FirstOrDefault(mh => mh.Topic == message.ApplicationMessage.Topic);

                if (handler != null) {
                    Console.WriteLine($"MQTT send message to handler: {handler.GetType().Name}");

                    await handler.HandleMessage(message.ApplicationMessage);

                    Console.WriteLine($"MQTT message handled by: {handler.GetType().Name}");
                }
            });

            await ConnectMqttAsync();
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        public void RegisterMessageHandler(IMessageHandler messageHandler) => messageHandlers.Add(messageHandler);
        public void RegisterSensor(SensorAttribute sensor) => sensors.Add(sensor);
        public async Task PublishSensorStateAsync(string sensorState) {
            var message = new MqttApplicationMessageBuilder()
                .WithPayload(sensorState)
                .WithTopic($"homeassistant/sensor/whr930/state")
                .WithRetainFlag(true)
                .Build();

            await client.PublishAsync(message);
        }

        private async Task ConnectMqttAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(clientName)
                .WithTcpServer(host, port)
                .WithCleanSession()
                .Build();

            await client.ConnectAsync(options, CancellationToken.None);
        }

        private async Task PublishSensorConfigAsync(SensorAttribute sensor)
        {
            var json = new JObject();

            var device = new JObject();
            device.Add("name", new JValue("Zehnder WHR 930"));
            json.Add("device", device);

            json.Add("name", new JValue(sensor.SensorName));
            json.Add("unique_id", new JValue(sensor.UniqueID));
            json.Add("expire_after ", new JValue(21_600));
            json.Add("state_topic", new JValue("homeassistant/sensor/whr930/state"));
            json.Add("value_template", new JValue("{{ value_json['" + sensor.UniqueID + "'] }}"));

            if (!string.IsNullOrEmpty(sensor.DeviceClass))
                json.Add("device_class", new JValue(sensor.DeviceClass));

            if (!string.IsNullOrEmpty(sensor.Icon))
                json.Add("icon", new JValue(sensor.Icon));

            if (!string.IsNullOrEmpty(sensor.UnitOfMeasurement))
                json.Add("unit_of_measurement", new JValue(sensor.UnitOfMeasurement));

            var message = new MqttApplicationMessageBuilder()
                .WithPayload(json.ToString())
                .WithTopic($"homeassistant/sensor/whr930-{sensor.UniqueID}/config")
                .WithRetainFlag(true)
                .Build();

            await client.PublishAsync(message);
        }
    }
}