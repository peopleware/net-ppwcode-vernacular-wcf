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

using PPWCode.Vernacular.Wcf.I.Behaviors;
using PPWCode.Vernacular.Wcf.I.Config;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ServiceBehaviorFixtures : BaseFixtures
    {
        [Test]
        public void Can_Apply_Throttling()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<ServiceThrottling>()
                        .DependsOn(
                            new
                            {
                                maxConcurrentCalls = 1,
                                maxConcurrentSessions = 2,
                                maxConcurrentInstances = 3
                            })
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Services),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))));
                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();
                ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single();
                ServiceThrottlingBehavior servicebehavior = host.Description.Behaviors.OfType<ServiceThrottlingBehavior>().SingleOrDefault();
                Assert.IsNotNull(servicebehavior);
                Assert.AreEqual(1, servicebehavior.MaxConcurrentCalls);
                Assert.AreEqual(2, servicebehavior.MaxConcurrentSessions);
                Assert.AreEqual(3, servicebehavior.MaxConcurrentInstances);
            }
        }

        [Test]
        public void Can_Apply_PrincipalPermissionModeAuthorization()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<PrincipalPermissionModeAuthorization>()
                        .DependsOn(new { principalPermissionMode = PrincipalPermissionMode.Custom })
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Services),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))));
                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();
                ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single();
                ServiceAuthorizationBehavior serviceAuthorizationBehavior = host.Description.Behaviors.OfType<ServiceAuthorizationBehavior>().SingleOrDefault();
                Assert.IsNotNull(serviceAuthorizationBehavior);
                Assert.AreEqual(PrincipalPermissionMode.Custom, serviceAuthorizationBehavior.PrincipalPermissionMode);
            }
        }

        [Test]
        public void Can_Apply_Specific_ServiceBehavior()
        {
            using (IWindsorContainer container = Container)
            {
                container.Register(
                    Component.For<ServiceBehavior4IOperations2>()
                        .Named("serviceBehaviorIOperations2")
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Explicit),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))),
                    Component.For<IOperations2>()
                        .ImplementedBy<Operations2>()
                        .LifeStyle.Transient
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations2"))
                                .AddExtensions("serviceBehaviorIOperations2")));

                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();

                // IOperations is PerSession
                // Check on behavior + check nr. instances created
                ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single(h => h.Description.Endpoints.Any(e => e.Contract.ContractType == typeof(IOperations)));
                ServiceBehaviorAttribute serviceBehavior = host.Description.Behaviors.OfType<ServiceBehaviorAttribute>().SingleOrDefault();
                Assert.IsNotNull(serviceBehavior);
                Assert.AreEqual(InstanceContextMode.PerSession, serviceBehavior.InstanceContextMode);

                IOperations client = GetClient();
                Operations.s_GetIntResults.Clear();

                client.GetInt();
                client.GetInt();
                Assert.AreEqual(2, Operations.s_GetIntResults.Count);
                Assert.AreEqual(1, Operations.s_GetIntResults[0]);
                Assert.AreEqual(2, Operations.s_GetIntResults[1]);

                // IOperations2 is PerCall
                // Check on behavior + check nr. instances created
                ServiceHost host2 = wcfFacility.Services.ManagedServiceHosts.Single(h => h.Description.Endpoints.Any(e => e.Contract.ContractType == typeof(IOperations2)));
                ServiceBehaviorAttribute serviceBehavior2 = host2.Description.Behaviors.OfType<ServiceBehaviorAttribute>().SingleOrDefault();
                Assert.IsNotNull(serviceBehavior2);
                Assert.AreEqual(InstanceContextMode.PerCall, serviceBehavior2.InstanceContextMode);

                IOperations2 client2 = GetClient2();
                Operations2.s_GetIntResults.Clear();

                client2.GetInt();
                client2.GetInt();
                Assert.AreEqual(2, Operations2.s_GetIntResults.Count);
                Assert.IsTrue(Operations2.s_GetIntResults.All(r => r == 1));
            }
        }

        private class ServiceBehavior4IOperations2 : ServiceBehaviorBase
        {
            protected override void Configure(ServiceBehaviorAttribute serviceBehavior)
            {
                serviceBehavior.InstanceContextMode = InstanceContextMode.PerCall;
            }
        }
    }
}