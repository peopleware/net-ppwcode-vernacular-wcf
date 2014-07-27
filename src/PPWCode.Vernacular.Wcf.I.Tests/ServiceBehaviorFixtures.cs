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

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

using Castle.Facilities.Logging;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

using log4net.Appender;
using log4net.Config;
using log4net.Core;

using NUnit.Framework;

using PPWCode.Vernacular.Wcf.I.Behaviors;
using PPWCode.Vernacular.Wcf.I.Config;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ServiceBehaviorFixtures
    {
        private MemoryAppender m_MemoryAppender;

        private IWindsorContainer Container
        {
            get
            {
                WindsorContainer container = new WindsorContainer();
                container.AddFacility<WcfFacility>(
                    f =>
                    {
                        f.CloseTimeout = TimeSpan.Zero;
                        f.Services.OpenServiceHostsEagerly = true;
                    });
                LoggingFacility logging = new LoggingFacility(LoggerImplementation.ExtendedLog4net);
                container.AddFacility(logging);
                m_MemoryAppender = new MemoryAppender();
                BasicConfigurator.Configure(m_MemoryAppender);
                return container;
            }
        }

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
                        .LifestyleSingleton()
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Services),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifestylePerWcfOperation()
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
                        .LifestyleSingleton()
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Services),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifestylePerWcfOperation()
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
                        .LifestyleSingleton()
                        .Attribute(WcfConstants.ExtensionScopeKey).Eq(WcfExtensionScope.Explicit),
                    Component.For<IOperations>()
                        .ImplementedBy<Operations>()
                        .LifestylePerWcfOperation()
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations"))),
                    Component.For<IOperations2>()
                        .ImplementedBy<Operations2>()
                        .LifestylePerWcfOperation()
                        .AsWcfService(
                            new DefaultServiceModel()
                                .AddEndpoints(WcfEndpoint
                                                  .BoundTo(new NetTcpBinding { PortSharingEnabled = true })
                                                  .At("net.tcp://localhost/Operations2"))
                                .AddExtensions("serviceBehaviorIOperations2")));

                WcfFacility wcfFacility = container.Kernel.GetFacilities().OfType<WcfFacility>().Single();

                // IOperations is PerSession
                // Check on behavior + check nr. instances created
                {
                    ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single(h => h.Description.Endpoints.Any(e => e.Contract.ContractType == typeof(IOperations)));
                    ServiceBehaviorAttribute serviceBehavior = host.Description.Behaviors.OfType<ServiceBehaviorAttribute>().SingleOrDefault();
                    Assert.IsNotNull(serviceBehavior);
                    Assert.AreEqual(InstanceContextMode.PerSession, serviceBehavior.InstanceContextMode);

                    IOperations client = ChannelFactory<IOperations>.CreateChannel(
                        new NetTcpBinding { PortSharingEnabled = true },
                        new EndpointAddress("net.tcp://localhost/Operations"));

                    client.GetInt();
                    LoggingEvent[] events = m_MemoryAppender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                    Assert.IsTrue(events.All(e => Convert.ToInt32(e.MessageObject) == 1));
                    m_MemoryAppender.Clear();

                    client.GetInt();
                    events = m_MemoryAppender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                    Assert.IsTrue(events.All(e => Convert.ToInt32(e.MessageObject) == 2));
                    m_MemoryAppender.Clear();
                }

                // IOperations2 is PerCall
                // Check on behavior + check nr. instances created
                {
                    ServiceHost host = wcfFacility.Services.ManagedServiceHosts.Single(h => h.Description.Endpoints.Any(e => e.Contract.ContractType == typeof(IOperations2)));
                    ServiceBehaviorAttribute serviceBehavior = host.Description.Behaviors.OfType<ServiceBehaviorAttribute>().SingleOrDefault();
                    Assert.IsNotNull(serviceBehavior);
                    Assert.AreEqual(InstanceContextMode.PerCall, serviceBehavior.InstanceContextMode);

                    IOperations2 client = ChannelFactory<IOperations2>.CreateChannel(
                        new NetTcpBinding { PortSharingEnabled = true },
                        new EndpointAddress("net.tcp://localhost/Operations2"));
                    client.GetInt();
                    client.GetInt();
                    LoggingEvent[] events = m_MemoryAppender.GetEvents();
                    Assert.AreEqual(2, events.Length);
                    Assert.IsTrue(events.All(e => Convert.ToInt32(e.MessageObject) == 1));
                }
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