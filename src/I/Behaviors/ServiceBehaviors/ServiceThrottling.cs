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

using System.ServiceModel.Description;

namespace PPWCode.Vernacular.Wcf.I.Behaviors
{
    public class ServiceThrottling : ConfigureServiceBehavior<ServiceThrottlingBehavior>
    {
        private readonly int m_MaxConcurrentCalls;
        private readonly int m_MaxConcurrentSessions;
        private readonly int m_MaxConcurrentInstances;

        public ServiceThrottling(int maxConcurrentCalls, int maxConcurrentSessions, int maxConcurrentInstances)
        {
            m_MaxConcurrentCalls = maxConcurrentCalls;
            m_MaxConcurrentSessions = maxConcurrentSessions;
            m_MaxConcurrentInstances = maxConcurrentInstances;
        }

        protected override void Configure(ServiceThrottlingBehavior serviceBehavior)
        {
            serviceBehavior.MaxConcurrentCalls = m_MaxConcurrentCalls;
            serviceBehavior.MaxConcurrentSessions = m_MaxConcurrentSessions;
            serviceBehavior.MaxConcurrentInstances = m_MaxConcurrentInstances;
        }
    }
}