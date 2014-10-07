using System;
using MassTransit;
using MassTransit.Transports.RabbitMq;
using MassTransit.Transports.RabbitMq.Configuration.Configurators;

namespace ClientC
{
	class Program
	{
		static void Main(string[] args)
		{
			var queueName = "ClientC." + Guid.NewGuid();
			Console.WriteLine("G'day mate! The queue name to for the bus is: {0}", queueName);

			try
			{
				Bus.Initialize(
					sbc =>
					{
						sbc.UseRabbitMq();
						sbc.ReceiveFrom(string.Format("rabbitmq://localhost/{0}", queueName));
						sbc.Subscribe(subs => subs.Handler<Common.ClientMessage>(msg => Console.WriteLine(msg.Text)));

					});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.WriteLine("Arrrrgh! You made a doozy!");
			}

			Console.Read();

			Bus.Shutdown();
		}
	}
}
