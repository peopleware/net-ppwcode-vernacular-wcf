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

        /// <inheritdoc/>
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