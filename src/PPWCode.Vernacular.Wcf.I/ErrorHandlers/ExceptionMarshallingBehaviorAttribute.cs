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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public class ExceptionMarshallingBehaviorAttribute :
        Attribute,
        IServiceBehavior,
        IEndpointBehavior,
        IContractBehavior
    {
        void IContractBehavior.AddBindingParameters(ContractDescription contract, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contract, ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            ApplyClientBehavior(runtime);
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contract, ServiceEndpoint endpoint, DispatchRuntime runtime)
        {
            ApplyDispatchBehavior(runtime.ChannelDispatcher);
        }

        void IContractBehavior.Validate(ContractDescription contract, ServiceEndpoint endpoint)
        {
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            ApplyClientBehavior(runtime);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher dispatcher)
        {
            ApplyDispatchBehavior(dispatcher.ChannelDispatcher);
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription service, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription service, ServiceHostBase host)
        {
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                ApplyDispatchBehavior(dispatcher);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription service, ServiceHostBase host)
        {
        }

        private void ApplyClientBehavior(ClientRuntime runtime)
        {
            // Don't add a message inspector if it already exists
            if (!runtime.MessageInspectors.OfType<ExceptionMarshallingMessageInspector>().Any())
            {
                runtime.MessageInspectors.Add(new ExceptionMarshallingMessageInspector());
            }
        }

        private void ApplyDispatchBehavior(ChannelDispatcher dispatcher)
        {
            // Don't add an error handler if it already exists
            if (!dispatcher.ErrorHandlers.OfType<ExceptionMarshallingErrorHandler>().Any())
            {
                dispatcher.ErrorHandlers.Add(new ExceptionMarshallingErrorHandler());
            }
        }
    }
}