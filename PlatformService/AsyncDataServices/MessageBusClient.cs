using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration configuration;
    private readonly IConnection connection;
    private readonly IModel channel;

    public MessageBusClient(IConfiguration configuration)
    {
        this.configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQHost"],
            Port = int.Parse(configuration["RabbitMQPort"])
        };

        try
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange:"trigger",type:ExchangeType.Fanout);

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

            Console.WriteLine("--> Connected to RabbitMQ Messagebus... ");
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection shutdown");
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message  = JsonSerializer.Serialize(platformPublishedDto);

        if(connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connected , sending message");
            SendMessage(message);
        }
        else{
            Console.WriteLine("--> RabbitMQ NOT Connected ");
        }

    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
                            exchange:"trigger",
                            routingKey:"",
                            basicProperties:null,
                            body:body
                            );
        
        Console.WriteLine("--> RabbitMQ Message sent :" + message);

    }

    public void Dispose()
    {
        if(channel.IsOpen)
        {
            channel.Close();
            connection.Close();
            Console.WriteLine("--> RabbitMQ MessageBus Disposed");
        }
    }
}
