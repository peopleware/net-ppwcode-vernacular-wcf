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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace PPWCode.Vernacular.Wcf.I.Behaviors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ServiceThrottlingAttribute
        : Attribute,
          IServiceBehavior
    {
        public int MaxConcurrentCalls { get; set; }

        public int MaxConcurrentInstances { get; set; }
        
        public int MaxConcurrentSessions { get; set; }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            ServiceThrottlingBehavior currentThrottle = serviceDescription.Behaviors.Find<ServiceThrottlingBehavior>();
            if (currentThrottle == null)
            {
                serviceDescription.Behaviors.Add(GetConfiguredServiceThrottlingBehaviour());
            }
        }

        private ServiceThrottlingBehavior GetConfiguredServiceThrottlingBehaviour()
        {
            ServiceThrottlingBehavior behaviour = new ServiceThrottlingBehavior();
            if (MaxConcurrentCalls > 0)
            {
                behaviour.MaxConcurrentCalls = MaxConcurrentCalls;
            }

            if (MaxConcurrentInstances > 0)
            {
                behaviour.MaxConcurrentInstances = MaxConcurrentInstances;
            }

            if (MaxConcurrentSessions > 0)
            {
                behaviour.MaxConcurrentSessions = MaxConcurrentSessions;
            }

            return behaviour;
        }
    }
}