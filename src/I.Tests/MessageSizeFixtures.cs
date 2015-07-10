// Copyright 2014 by PeopleWare n.v..
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

using NUnit.Framework;

using PPWCode.Vernacular.Wcf.I.ErrorHandlers;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class MessageSizeFixtures : BaseFixtures
    {
        [Test]
        public void Client_Can_Recover_From_QuotaExceededException()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<LogErrorHandler>(),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))),
                    Component.For<IOperations>()
                        .Named("client")
                        .LifeStyle.Transient
                        .AsWcfClient(
                            new DefaultClientModel(
                                WcfEndpoint
                                    .ForContract<IOperations>()
                                    .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                    .At("net.tcp://localhost/Operations"))));

                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();
                ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single();
                ServiceEndpoint endpoint = host.Description.Endpoints.First();
                NetTcpBinding binding = endpoint.Binding as NetTcpBinding;
                Assert.IsNotNull(binding);

                long maxMessageSize = binding.MaxReceivedMessageSize;
                IOperations client = container.Resolve<IOperations>("client");

                try
                {
                    client.GetByteArrayInt(maxMessageSize + 1);
                }
                catch (CommunicationException e)
                {
                    if (!(e.InnerException is QuotaExceededException))
                    {
                        throw;
                    }
                }
                int i = client.GetInt();
                Assert.AreEqual(1, i);
            }
        }
    }
}