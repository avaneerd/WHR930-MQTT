using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using Newtonsoft.Json;
using WHR930.Commands;

namespace WHR930.MessageHandlers
{
    public class SetVentilationLevelMessageHandler : IMessageHandler
    {
        private readonly SerialCommandClient commandClient;

        public string Topic => "whr930/ventilation-level/set";

        public SetVentilationLevelMessageHandler(SerialCommandClient commandClient)
        {
            this.commandClient = commandClient;
        }

        public Task HandleMessage(MqttApplicationMessage message)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(message.Payload);

                var payloadObject = JsonConvert.DeserializeObject<SetVentilationLevel>(payload);

                byte highest(byte left, int right) => left > right ? left : (byte)right;

                var serialCommand = new SetVentilationLevelCommand();
                serialCommand.ExhaustAbsent = highest(serialCommand.ExhaustAbsent, payloadObject.DesiredVentilationOut);
                serialCommand.ExhaustLow = highest(serialCommand.ExhaustLow, payloadObject.DesiredVentilationOut);
                serialCommand.ExhaustMedium = highest(serialCommand.ExhaustMedium, payloadObject.DesiredVentilationOut);
                serialCommand.ExhaustHigh = highest(serialCommand.ExhaustHigh, payloadObject.DesiredVentilationOut);

                serialCommand.SupplyAbsent = highest(serialCommand.SupplyAbsent, payloadObject.DesiredVentilationIn);
                serialCommand.SupplyLow = highest(serialCommand.SupplyLow, payloadObject.DesiredVentilationIn);
                serialCommand.SupplyMedium = highest(serialCommand.SupplyMedium, payloadObject.DesiredVentilationIn);
                serialCommand.SupplyHigh = highest(serialCommand.SupplyHigh, payloadObject.DesiredVentilationIn);

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

    public class SetVentilationLevel
    {
        public int DesiredVentilationIn { get; set; }
        public int DesiredVentilationOut { get; set; }
    }
}