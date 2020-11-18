using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using Newtonsoft.Json;
using WHR930.Commands;

namespace WHR930.MessageHandlers {
    public class SetConfortTemperatureMessageHandler : IMessageHandler
    {
        private readonly SerialCommandClient commandClient;

        public string Topic => "whr930/comfort-temperature/set";

        public SetConfortTemperatureMessageHandler(SerialCommandClient commandClient)
        {
            this.commandClient = commandClient;
        }

        public Task HandleMessage(MqttApplicationMessage message)
        {
            try
            {
                var serialCommand = new SetComfortTemperatureCommand();
                serialCommand.ComfortTemperature = double.Parse(Encoding.UTF8.GetString(message.Payload));

                commandClient.SendCommandWithAck(serialCommand);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                throw;
            }
        }
    }
}