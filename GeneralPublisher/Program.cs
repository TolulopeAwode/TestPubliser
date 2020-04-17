using CollegeBox.Contract.Commands;
using CollegeBox.Contract.Config;
using CollegeBox.Contract.Queries;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("CollegeBox Event Sourcing>>>>>>");
            while (true)
            {
                Console.Write("Award Description: ");
                var awrdName = Console.ReadLine();
                Console.Write("Acronyms: ");
                var acronym = Console.ReadLine();

                var bus = BusConfigurator.ConfigureBus();
                Console.Title = "Publisher...";
                await bus.StartAsync();
                //await bus.Publish<AwardCommand>(new
                //{
                //    Description = awrdName,
                //    Acronym=acronym,
                //    InstitutionId=1
                //});

                var serviceAddress = new Uri($"{RabbitMqConstants.RabbitMqUri}/{RabbitMqConstants.UpdateAward}");
                var requestTimeout = TimeSpan.FromSeconds(60);
                var client = bus.CreateRequestClient<EditAwardCommand>(serviceAddress, requestTimeout);
                var responseContext = await client.GetResponse<GenericResponse<bool>>(new { Id = 1, Description=awrdName, Acronym=acronym, InstitutionId=1 }, CancellationToken.None);
                if (responseContext.Message.Response)
                    Console.WriteLine($"Success:>> {responseContext.Message.Message}");
                else
                    Console.WriteLine($"Error:>>{responseContext.Message.Message}");



                Console.WriteLine("Press X to terminate");
                if (Console.ReadLine().ToLower() == "x") break;
                await bus.StopAsync();
                Console.Clear();
            }
        }
    }
}
