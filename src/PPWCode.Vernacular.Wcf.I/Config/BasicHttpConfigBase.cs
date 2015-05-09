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
using System.ServiceModel.Channels;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public abstract class BasicHttpConfigBase<T> : NetConfigBase<T>
        where T : class
    {
        protected const string PortKey = "Port";
        protected const string ProtocolKey = "Protocol";
        protected const string MaxBufferSizeKey = "MaxBufferSize";
        protected const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";
        protected const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";

        protected BasicHttpConfigBase(string @namespace)
            : base(@namespace)
        {
        }

        protected abstract int DefaultPort { get; }

        protected abstract string Protocol { get; }

        protected abstract HttpBindingBase CreateHttpBinding();

        protected virtual int Port
        {
            get { return GetAppSetting(PortKey, DefaultPort); }
        }

        public virtual string BaseAddress
        {
            get { return string.Format(@"{0}://{1}:{2}/{3}", Protocol, Host, Port, Namespace); }
        }

        public virtual string Address
        {
            get { return string.Format("{0}/{1}", BaseAddress, ServiceName); }
        }

        protected virtual int DefaultMaxBufferSize
        {
            get { return ConfigHelper.GetAppSetting(MaxBufferSizeKey, 65536); }
        }

        protected virtual int MaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, DefaultMaxBufferSize); }
        }

        protected virtual int DefaultMaxBufferPoolSize
        {
            get { return ConfigHelper.GetAppSetting(MaxBufferPoolSizeKey, 512 * 1024); }
        }

        protected virtual int MaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, DefaultMaxBufferPoolSize); }
        }

        protected virtual int DefaultMaxReceivedMessageSize
        {
            get { return ConfigHelper.GetAppSetting(MaxReceivedMessageSizeKey, 65536); }
        }

        protected virtual int MaxReceivedMessageSize
        {
            get { return GetAppSetting(MaxReceivedMessageSizeKey, DefaultMaxReceivedMessageSize); }
        }

        public virtual Binding Binding
        {
            get
            {
                HttpBindingBase binding = CreateHttpBinding();
                binding.AllowCookies = false;
                binding.BypassProxyOnLocal = false;
                binding.SendTimeout = SendTimeout;
                binding.ReceiveTimeout = ReceiveTimeout;
                binding.OpenTimeout = OpenTimeout;
                binding.CloseTimeout = CloseTimeout;
                binding.MaxBufferSize = MaxBufferSize;
                binding.MaxBufferPoolSize = MaxBufferPoolSize;
                binding.MaxReceivedMessageSize = MaxReceivedMessageSize;
                binding.ReaderQuotas = ReaderQuotas;

                return binding;
            }
        }

        public virtual EndpointAddress EndpointAddress
        {
            get
            {
                Uri uri = new Uri(Address, UriKind.Absolute);
                return new EndpointAddress(uri);
            }
        }

        public override IWcfClientModel GetClientModel(params object[] extensions)
        {
            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(EndpointAddress)
                    .AddExtensions(extensions);

            DefaultClientModel clientModel = new DefaultClientModel(endpoint);
            if (OpenOnDemand)
            {
                clientModel.OpenOnDemand();
            }

            if (!AsyncCapability)
            {
                clientModel.WithoutAsyncCapability();
            }

            return clientModel;
        }

        public override IWcfServiceModel GetServiceModel(params object[] extensions)
        {
            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(ServiceName)
                    .AddExtensions(extensions);

            return
                new DefaultServiceModel
                {
                    BaseAddresses = new[] { new Uri(BaseAddress, UriKind.Absolute) },
                    Endpoints = new[] { endpoint },
                };
        }
    }
}