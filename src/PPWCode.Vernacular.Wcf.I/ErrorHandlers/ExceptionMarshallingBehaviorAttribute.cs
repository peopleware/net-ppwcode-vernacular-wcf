/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Using

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

#endregion

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public class ExceptionMarshallingBehaviorAttribute :
        Attribute,
        IServiceBehavior,
        IEndpointBehavior,
        IContractBehavior
    {
        //private static readonly ILog s_Logger = LogManager.GetLogger(typeof(ExceptionMarshallingBehaviorAttribute));

        #region IContractBehavior Members

        void IContractBehavior.AddBindingParameters(ContractDescription contract, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contract, ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            //if (s_Logger.IsDebugEnabled)
            //{
            //    s_Logger.DebugFormat("Applying client ExceptionMarshallingBehavior to contract {0}", contract.ContractType);
            //}
            ApplyClientBehavior(runtime);
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contract, ServiceEndpoint endpoint, DispatchRuntime runtime)
        {
            //if (s_Logger.IsDebugEnabled)
            //{
            //    s_Logger.DebugFormat("Applying dispatch ExceptionMarshallingBehavior to contract {0}", contract.ContractType.FullName);
            //}
            ApplyDispatchBehavior(runtime.ChannelDispatcher);
        }

        void IContractBehavior.Validate(ContractDescription contract, ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region IEndpointBehavior Members

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            //if (s_Logger.IsDebugEnabled)
            //{
            //    s_Logger.DebugFormat("Applying client ExceptionMarshallingBehavior to endpoint {0}", endpoint.Address);
            //}
            ApplyClientBehavior(runtime);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher dispatcher)
        {
            //if (s_Logger.IsDebugEnabled)
            //{
            //    s_Logger.DebugFormat("Applying dispatch ExceptionMarshallingBehavior to endpoint {0}", endpoint.Address);
            //}
            ApplyDispatchBehavior(dispatcher.ChannelDispatcher);
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region IServiceBehavior Members

        void IServiceBehavior.AddBindingParameters(ServiceDescription service, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription service, ServiceHostBase host)
        {
            //if (s_Logger.IsDebugEnabled)
            //{
            //    s_Logger.DebugFormat("Applying dispatch ExceptionMarshallingBehavior to service {0}", service.ServiceType.FullName);
            //}
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                ApplyDispatchBehavior(dispatcher);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription service, ServiceHostBase host)
        {
        }

        #endregion

        #region Private Members

        private void ApplyClientBehavior(ClientRuntime runtime)
        {
            // Don't add a message inspector if it already exists
            if (runtime.MessageInspectors.OfType<ExceptionMarshallingMessageInspector>().Any())
            {
                return;
            }

            runtime.MessageInspectors.Add(new ExceptionMarshallingMessageInspector());
        }

        private void ApplyDispatchBehavior(ChannelDispatcher dispatcher)
        {
            // Don't add an error handler if it already exists
            if (dispatcher.ErrorHandlers.OfType<ExceptionMarshallingErrorHandler>().Any())
            {
                return;
            }

            dispatcher.ErrorHandlers.Add(new ExceptionMarshallingErrorHandler());
        }

        #endregion
    }
}
