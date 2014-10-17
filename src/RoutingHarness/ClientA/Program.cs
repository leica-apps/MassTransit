using System.Configuration;
using Common;

namespace ClientA
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new ClientCode();
			client.RunClientCode(args, ConfigurationManager.AppSettings["RabbitMqRoutingKey"]);
		}
	}
}
