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

using System.Security.Principal;
using System.ServiceModel;

using Castle.Facilities.WcfIntegration;

namespace PPWCode.Vernacular.Wcf.I.Behaviors
{
    public class AllowedImpersonationLevelAuthorization : AbstractChannelFactoryAware
    {
        private readonly TokenImpersonationLevel m_TokenImpersonationLevel;

        public AllowedImpersonationLevelAuthorization(TokenImpersonationLevel tokenImpersonationLevel)
        {
            m_TokenImpersonationLevel = tokenImpersonationLevel;
        }

        /// <inheritdoc />
        public override void Opening(ChannelFactory channelFactory)
        {
            base.Opening(channelFactory);

            if (channelFactory.Credentials != null)
            {
                channelFactory.Credentials.Windows.AllowedImpersonationLevel = m_TokenImpersonationLevel;
            }
        }
    }
}