using System;
using System.Threading;
using MassTransit;

namespace Common
{
	public class ClientCode
	{
		public void RunClientCode(string[] args, string client)
		{
			var queueName = client + Guid.NewGuid();
			//Console.WriteLine("G'day mate! The queue name to for the bus is: {0}", queueName);

			var numberOfExpectedMessages = ClientMessage.DefaultQuantity;
			var numberOfActualMessagesReceived = 0;
			if (args.Length > 0)
			{
				var parsed = int.TryParse(args[0], out numberOfExpectedMessages);
				if (!parsed) numberOfExpectedMessages = ClientMessage.DefaultQuantity;
			}

			try
			{
				Bus.Initialize(sbc =>
				{
					sbc.UseRabbitMq();
					sbc.ReceiveFrom(string.Format("rabbitmq://localhost/{0}", queueName));
					sbc.Subscribe(subs => subs.Handler<ClientMessage>(msg =>
					{
						Console.WriteLine(msg.Text);
						numberOfActualMessagesReceived++;
					}));
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.WriteLine("Arrrrgh! You made a doozy!");
			}

			while (numberOfActualMessagesReceived < numberOfExpectedMessages) Thread.Sleep(1);

			Thread.Sleep(1000);

			Bus.Shutdown();
		}
	}
}
