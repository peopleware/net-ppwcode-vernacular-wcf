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
using System.ServiceModel;

using Castle.Facilities.Logging;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;

using log4net.Appender;
using log4net.Config;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    public class BaseFixtures
    {
        private MemoryAppender m_MemoryAppender;

        protected IWindsorContainer Container
        {
            get
            {
                WindsorContainer container = new WindsorContainer();
                container.AddFacility<WcfFacility>(
                    f => { f.CloseTimeout = TimeSpan.Zero; });
                LoggingFacility logging = new LoggingFacility(LoggerImplementation.ExtendedLog4net);
                container.AddFacility(logging);
                m_MemoryAppender = new MemoryAppender();
                BasicConfigurator.Configure(MemoryAppender);
                return container;
            }
        }

        protected MemoryAppender MemoryAppender
        {
            get { return m_MemoryAppender; }
        }

        protected IOperations GetClient()
        {
            return ChannelFactory<IOperations>.CreateChannel(
                new NetTcpBinding { PortSharingEnabled = true },
                new EndpointAddress("net.tcp://localhost/Operations"));
        }

        protected IOperations2 GetClient2()
        {
            return ChannelFactory<IOperations2>.CreateChannel(
                new NetTcpBinding { PortSharingEnabled = true },
                new EndpointAddress("net.tcp://localhost/Operations2"));
        }
    }
}