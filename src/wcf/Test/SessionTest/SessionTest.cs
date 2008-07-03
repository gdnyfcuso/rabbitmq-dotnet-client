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

namespace RabbitMQ.ServiceModel.Test.SessionTest
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;

    public class SessionTest : IServiceTest<ICart>
    {
        ServiceHost service;
        ChannelFactory<ICart> factory;

        public void StartService(Binding binding)
        {
            Util.Write(ConsoleColor.Yellow, "  Binding Service...");
            service = new ServiceHost(typeof(Cart), new Uri("soap.amqp:///"));
            service.AddServiceEndpoint(typeof(ICart), binding, "Cart");
            service.Open();

            Thread.Sleep(500);
            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public void StopService()
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Service...");
            service.Close();
            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public ICart GetClient(Binding binding)
        {
            factory = new ChannelFactory<ICart>(binding, "soap.amqp:///Cart");
            factory.Open();
            return factory.CreateChannel();
        }

        public void StopClient(ICart client)
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Client...");
            ((IChannel)client).Close();
            factory.Close();
            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        private void AddToCart(ICart cart, string name, double price) {
            CartItem item = new CartItem();
            item.Name = name;
            item.Price = price;
            Util.WriteLine(ConsoleColor.Magenta, "  Adding {0} to cart", name);
            cart.Add(item);
        }

        public void Run()
        {
            StartService(Program.GetBinding());

            ICart cart = GetClient(Program.GetBinding());

            AddToCart(cart, "Beans", 0.49);
            AddToCart(cart, "Bread", 0.89);
            AddToCart(cart, "Toaster", 4.99);

            double total = cart.GetTotal();
            if (total != (0.49 + 0.89 + 4.99))
                throw new Exception("Incorrect Total");

            Util.WriteLine(ConsoleColor.Magenta, "  Total: {0}", total);
            
            try
            {
                StopClient(cart);
                StopService();
            }
            catch (Exception)
            {
                Util.WriteLine(ConsoleColor.Red, "  Failed to Close Gracefully.");
            }
        }
    }
}
