using Message.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace Message
{
    public partial class Service1 : ServiceBase
    {
        private IConnection _connection;
        private IModel _channel;
        private string _connectionString = "Server=.;Database=DtechLogger;Trusted_Connection=True;TrustServerCertificate=True"; // Replace with your actual connection string

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "Inbound",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    Request myRequest = JsonConvert.DeserializeObject<Request>(message);

                    // Create a file with the name specified in myRequest.FileName
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, myRequest.FileName);
                    File.WriteAllLines(filePath, myRequest.FileContentLines);

                    // Insert data into MyLogger table
                    InsertLogEntry(myRequest);
                }
                catch (Exception ex)
                {
                    // Log the exception to the Event Log
                    EventLog.WriteEntry("Message Service", $"Error: {ex.Message}", EventLogEntryType.Error);
                }
            };

            _channel.BasicConsume(queue: "Inbound",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void InsertLogEntry(Request myRequest)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO MyLogger (Originator, FileName, LogDate, Status) VALUES (@Originator, @FileName, @LogDate, @Status)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Originator", myRequest.Originator);
                        command.Parameters.AddWithValue("@FileName", myRequest.FileName);
                        command.Parameters.AddWithValue("@LogDate", DateTime.Now);
                        command.Parameters.AddWithValue("@Status", "Sent");

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception to the Event Log
                EventLog.WriteEntry("Message Service", $"Error inserting log entry: {ex.Message}", EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
