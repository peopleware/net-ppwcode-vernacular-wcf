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

using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

using NUnit.Framework;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class EndPointNotFoundFixtures : BaseFixtures
    {
        [Test, ExpectedException(typeof(EndpointNotFoundException))]
        public void Can_Call_From_Client_To_None_Existing_EndPoint()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<IOperations>()
                        .Named("client")
                        .LifeStyle.Transient
                        .AsWcfClient(
                            new DefaultClientModel(
                                WcfEndpoint
                                    .ForContract<IOperations>()
                                    .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                    .At("net.tcp://localhost/Operations"))));

                IOperations client = container.Resolve<IOperations>("client");
                client.GetInt();
            }
        }

        [Test, ExpectedException(typeof(EndpointNotFoundException))]
        public void Can_Call_From_Client_To_Closed_Service_()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
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

                IOperations client = container.Resolve<IOperations>("client");
                client.GetInt();

                // Shutdown the server
                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();
                ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single();
                host.Close();

                client.GetInt();
            }
        }
    }
}