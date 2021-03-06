﻿
namespace PublisherA
{
	using System;
	using Common;
	using MassTransit;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("This is the Publisher A");

			var numberOfMessages = ClientMessage.DefaultQuantity; 

			if (args.Length > 0)
			{
				var parsed = int.TryParse(args[0], out numberOfMessages);
				if (!parsed) numberOfMessages = ClientMessage.DefaultQuantity; 
			}

			Bus.Initialize(sbc =>
			{
				sbc.UseRabbitMq();
				sbc.ReceiveFrom("rabbitmq://localhost/publisher_a");
			});

			for (var i = 0; i < numberOfMessages; i++)
			{
				Bus.Instance.Publish(new ClientMessage { Text = "ClientA." + Guid.NewGuid() });
			}

			Bus.Shutdown();
		}
	}
}
