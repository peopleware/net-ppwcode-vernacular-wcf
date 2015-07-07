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

using System.ServiceModel;

using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

using NUnit.Framework;

using PPWCode.Vernacular.Exceptions.II;
using PPWCode.Vernacular.Wcf.I.ErrorHandlers;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ExceptionMarshallingFixtures : BaseFixtures
    {
        [Test]
        public void Can_Catch_SemanticException()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<ExceptionMarshallingBehaviorAttribute>(),
                    Component.For<IOperations>()
                        .Named("client")
                        .LifeStyle.Transient
                        .AsWcfClient(
                            new DefaultClientModel(
                                WcfEndpoint
                                    .ForContract<IOperations>()
                                    .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                    .At("net.tcp://localhost/Operations"))),
                    Component.For<IOperations>()
                        .Named("server")
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))));

                IOperations client = container.Resolve<IOperations>("client");
                try
                {
                    client.ThrowSemanticException();
                    Assert.Fail("Should have raised an exception");
                }
                catch (SemanticException)
                {
                }
            }
        }

        [Test]
        public void Can_Catch_NonSerializableException()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<ExceptionMarshallingBehaviorAttribute>(),
                    Component.For<IOperations>()
                        .Named("client")
                        .LifeStyle.Transient
                        .AsWcfClient(
                            new DefaultClientModel(
                                WcfEndpoint
                                    .ForContract<IOperations>()
                                    .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                    .At("net.tcp://localhost/Operations"))),
                    Component.For<IOperations>()
                        .Named("server")
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))));

                IOperations client = container.Resolve<IOperations>("client");
                try
                {
                    client.ThrowNonSerializableException();
                    Assert.Fail("Should have raised an exception");
                }
                catch (ProgrammingError e)
                {
                    Assert.IsTrue(e.Message.StartsWith("Exception of type NonSerializableException wasn't serializable, rethrown as plain exception"));
                }
            }
        }
    }
}