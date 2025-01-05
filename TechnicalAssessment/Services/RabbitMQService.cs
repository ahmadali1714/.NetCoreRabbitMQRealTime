using RabbitMQ.Client;
using System.Text;

public class RabbitMQService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "Inbound",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "",
                             routingKey: "Inbound",
                             basicProperties: null,
                             body: body);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
