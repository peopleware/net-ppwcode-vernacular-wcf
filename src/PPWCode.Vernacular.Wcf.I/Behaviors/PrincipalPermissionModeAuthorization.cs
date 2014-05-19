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