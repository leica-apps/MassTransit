// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.RabbitMq.Tests
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using Magnum.TestFramework;
	using NUnit.Framework;

	[Scenario]
	public class Given_routing_is_enabled
	{
		private string _publisherAOutput;
		private string _publisherBOutput;
		private string _clientAOutput;
		private string _clientBOutput;
		private string _clientCOutput;

		const int _publisherANumberOfMessages = 10;
		const int _publisherBNumberOfMessages = 25;

		private Process _publisherA;
		public Process PublisherA
		{
			get
			{
				return _publisherA ?? (_publisherA =
					GetProcess(
						@"RoutingHarness\PublisherA.exe",
						_publisherANumberOfMessages.ToString(CultureInfo.InvariantCulture)));
			}
		}

		private Process _publisherB;
		public Process PublisherB
		{
			get
			{
				return _publisherB ?? (_publisherB =
					GetProcess(
						@"RoutingHarness\PublisherB.exe",
						_publisherBNumberOfMessages.ToString(CultureInfo.InvariantCulture)));
			}
		}

		private Process _clientA;
		public Process ClientA
		{
			get
			{
				return _clientA ?? (_clientA =
					GetProcess(
						@"RoutingHarness\ClientA.exe",
						_publisherANumberOfMessages.ToString(CultureInfo.InvariantCulture)));
			}
		}


		private Process _clientB;
		public Process ClientB
		{
			get
			{
				return _clientB ?? (_clientB =
					GetProcess(
						@"RoutingHarness\ClientB.exe",
						_publisherBNumberOfMessages.ToString(CultureInfo.InvariantCulture)));
			}
		}

		private Process _clientC;
		public Process ClientC
		{
			get
			{
				return _clientC ?? (_clientC =
					GetProcess(
						@"RoutingHarness\ClientC.exe",
						(_publisherANumberOfMessages + _publisherBNumberOfMessages).ToString(CultureInfo.InvariantCulture)));
			}
		}

		[When]
		public void Routing_harness_is_executed()
		{
			PublisherA.Start();
			PublisherB.Start();
			ClientA.Start();
			ClientB.Start();
			ClientC.Start();

			PublisherA.WaitForExit();
			PublisherB.WaitForExit();
			ClientA.WaitForExit();
			ClientB.WaitForExit();
			ClientC.WaitForExit();

			_publisherAOutput = PublisherA.StandardOutput.ReadToEnd();
			_publisherBOutput = PublisherB.StandardOutput.ReadToEnd();
			_clientAOutput = ClientA.StandardOutput.ReadToEnd();
			_clientBOutput = ClientB.StandardOutput.ReadToEnd();
			_clientCOutput = ClientC.StandardOutput.ReadToEnd();
		}

		[Then]
		public void client_a_captures_the_correct_number_of_messages()
		{
			var messages = _clientAOutput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Assert.That(_clientAOutput, Is.Not.Null);
			Assert.That(messages, Is.Not.Null);
			Assert.That(messages.Count(), Is.EqualTo(_publisherANumberOfMessages));
			Assert.That(messages.All(msg => msg.StartsWith("ClientA")));
		}

		[Then]
		public void client_b_captures_the_correct_number_of_messages()
		{
			var messages = _clientBOutput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Assert.That(_clientBOutput, Is.Not.Null);
			Assert.That(messages, Is.Not.Null);
			Assert.That(messages.Count(), Is.EqualTo(_publisherBNumberOfMessages));
			Assert.That(messages.All(msg => msg.StartsWith("ClientB")));
		}

		[Then]
		public void client_c_captures_the_correct_number_of_messages()
		{
			var messages = _clientCOutput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Assert.That(_clientCOutput, Is.Not.Empty);
			Assert.That(messages, Is.Not.Null);
			Assert.That(messages.Count(), Is.EqualTo(_publisherANumberOfMessages + _publisherBNumberOfMessages));
			Assert.That(messages.Count(msg => msg.StartsWith("ClientA")), Is.EqualTo(_publisherANumberOfMessages));
			Assert.That(messages.Count(msg => msg.StartsWith("ClientB")), Is.EqualTo(_publisherBNumberOfMessages));
		}

		private Process GetProcess(string fileName, string argument)
		{
			return new Process
			{
				StartInfo =
				{
					FileName = fileName,
					Arguments = argument,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
				}
			};
		}
	}
}