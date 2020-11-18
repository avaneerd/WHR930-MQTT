using System.Threading.Tasks;
using MQTTnet;

namespace WHR930.MessageHandlers {
    public interface IMessageHandler {
        string Topic { get; }
        Task HandleMessage(MqttApplicationMessage message);
    }
}