using Common;

namespace ClientC
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new ClientCode();
			client.RunClientCode(args, "ClientC.CloudCustomer");
		}
	}
}
