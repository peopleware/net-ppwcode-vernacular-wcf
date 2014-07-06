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
using System.ServiceModel.Description;

using Castle.Facilities.WcfIntegration;

namespace PPWCode.Vernacular.Wcf.I.Behaviors
{
    public class PrincipalPermissionModeAuthorization : AbstractServiceHostAware
    {
        private readonly PrincipalPermissionMode m_PrincipalPermissionMode;

        public PrincipalPermissionModeAuthorization(PrincipalPermissionMode principalPermissionMode)
        {
            m_PrincipalPermissionMode = principalPermissionMode;
        }

        protected override void Opening(ServiceHost serviceHost)
        {
            ServiceAuthorizationBehavior authorization = EnsureServiceBehavior<ServiceAuthorizationBehavior>(serviceHost);
            authorization.PrincipalPermissionMode = m_PrincipalPermissionMode;
        }

        private static T EnsureServiceBehavior<T>(ServiceHost serviceHost)
            where T : class, IServiceBehavior, new()
        {
            T behavior = serviceHost.Description.Behaviors.Find<T>();

            if (behavior == null)
            {
                behavior = new T();
                serviceHost.Description.Behaviors.Add(behavior);
            }

            return behavior;
        }
    }
}