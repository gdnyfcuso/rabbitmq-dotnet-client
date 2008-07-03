// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007, 2008 LShift Ltd., Cohesive Financial
//   Technologies LLC., and Rabbit Technologies Ltd.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//   The contents of this file are subject to the Mozilla Public License
//   Version 1.1 (the "License"); you may not use this file except in
//   compliance with the License. You may obtain a copy of the License at
//   http://www.rabbitmq.com/mpl.html
//
//   Software distributed under the License is distributed on an "AS IS"
//   basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//   License for the specific language governing rights and limitations
//   under the License.
//
//   The Original Code is The RabbitMQ .NET Client.
//
//   The Initial Developers of the Original Code are LShift Ltd.,
//   Cohesive Financial Technologies LLC., and Rabbit Technologies Ltd.
//
//   Portions created by LShift Ltd., Cohesive Financial Technologies
//   LLC., and Rabbit Technologies Ltd. are Copyright (C) 2007, 2008
//   LShift Ltd., Cohesive Financial Technologies LLC., and Rabbit
//   Technologies Ltd.;
//
//   All Rights Reserved.
//
//   Contributor(s): ______________________________________.
//
//---------------------------------------------------------------------------

namespace RabbitMQ.ServiceModel.Test.OneWayTest
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using RabbitMQ.ServiceModel;
    using System.Threading;

    public class OneWayTest : IServiceTest<ILogServiceContract>
    {
        private ChannelFactory<ILogServiceContract> factory;
        private ServiceHost host;
        private bool serviceStarted;
        private ILogServiceContract client;

        public void BeginRun()
        {
            StartService(Program.GetBinding());

            client = GetClient(Program.GetBinding());
            client.Log(new LogData(LogLevel.High, "Hello Rabbit"));
            client.Log(new LogData(LogLevel.Medium, "Hello Rabbit"));
            client.Log(new LogData(LogLevel.Low, "Hello Rabbit"));
            client.Log(new LogData(LogLevel.Low, "Last Message"));

        }

        public void Run()
        {
            BeginRun();
            System.Threading.Thread.Sleep(500);
            EndRun();
        }

        public void EndRun()
        {
            StopClient(client);
            StopService();
        }

        public void StartService(Binding binding)
        {
            Util.Write(ConsoleColor.Yellow, "  Binding Service...");
            host = new ServiceHost(typeof(LogService), new Uri("soap.amqp:///"));
            ((RabbitMQBinding)binding).OneWayOnly = true;
            //host = new ServiceHost(typeof(LogService), new Uri("http://localhost/"));
            
            host.AddServiceEndpoint(typeof(ILogServiceContract), binding, "LogService");
            host.Open();
            serviceStarted = true;

            Thread.Sleep(500);
            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public void StopService()
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Service...");
            if (serviceStarted)
            {
                host.Close();
                serviceStarted = false;
            }

            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public ILogServiceContract GetClient(Binding binding)
        {
            ((RabbitMQBinding)binding).OneWayOnly = true;
            factory = new ChannelFactory<ILogServiceContract>(binding, "soap.amqp:///LogService");
            //factory = new ChannelFactory<ILogServiceContract>(binding, "http://localhost/LogService");
            factory.Open();
            return factory.CreateChannel();
        }

        public void StopClient(ILogServiceContract client)
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Client...");

            ((IClientChannel)client).Close();
            factory.Close();

            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }
    }
}
