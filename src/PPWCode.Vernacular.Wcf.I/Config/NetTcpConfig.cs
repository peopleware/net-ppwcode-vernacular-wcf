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
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public class NetTcpConfig<T> : NetConfigBase<T>
        where T : class
    {
        protected const string PortKey = "Port";

        protected const string ServicePrincipalNameKey = "ServicePrincipalName";

        protected const string UserPrincipalNameKey = "UserPrincipalName";

        protected const string DnsKey = "Dns";

        protected const string TransactionFlowKey = "TransactionFlow";

        protected const string InactivityTimeoutKey = "InactivityTimeout";

        protected const string MaxBufferSizeKey = "MaxBufferSize";

        protected const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";

        protected const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";

        public NetTcpConfig(string @namespace)
            : base(@namespace)
        {
        }

        protected virtual int DefaultPort
        {
            get { return ConfigHelper.GetAppSetting<int>(PortKey); }
        }

        protected virtual int Port
        {
            get { return GetAppSetting(PortKey, DefaultPort); }
        }

        protected virtual string DefaultInactivityTimeout
        {
            get { return ConfigHelper.GetAppSetting(InactivityTimeoutKey, "00:10:00"); }
        }

        protected virtual TimeSpan InactivityTimeout
        {
            get { return GetTimeout(InactivityTimeoutKey, DefaultInactivityTimeout); }
        }

        protected virtual int DefaultMaxBufferSize
        {
            get { return ConfigHelper.GetAppSetting(MaxBufferSizeKey, 65536); }
        }

        protected virtual int MaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, DefaultMaxBufferSize); }
        }

        protected virtual string ServicePrincipalName
        {
            get { return GetAppSetting(ServicePrincipalNameKey, (string)null); }
        }

        protected virtual string UserPrincipalName
        {
            get { return GetAppSetting(UserPrincipalNameKey, (string)null); }
        }

        protected virtual string Dns
        {
            get { return GetAppSetting(DnsKey, (string)null); }
        }

        protected virtual bool DefaultTransactionFlow
        {
            get { return ConfigHelper.GetAppSetting(TransactionFlowKey, false); }
        }

        protected virtual bool TransactionFlow
        {
            get { return GetAppSetting(TransactionFlowKey, DefaultTransactionFlow); }
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

        public virtual string BaseAddress
        {
            get { return string.Format(@"net.tcp://{0}:{1}/{2}", Host, Port, Namespace); }
        }

        public virtual string Address
        {
            get { return string.Format("{0}/{1}", BaseAddress, ServiceName); }
        }

        protected virtual NetTcpSecurity Security
        {
            get
            {
                TcpTransportSecurity tcpTransportSecurity =
                    new TcpTransportSecurity
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows,
                        ProtectionLevel = ProtectionLevel.EncryptAndSign
                    };

                return new NetTcpSecurity
                       {
                           Mode = SecurityMode.Transport,
                           Transport = tcpTransportSecurity
                       };
            }
        }

        public virtual Binding Binding
        {
            get
            {
                OptionalReliableSession optionalReliableSession =
                    new OptionalReliableSession
                    {
                        Enabled = true,
                        InactivityTimeout = InactivityTimeout,
                        Ordered = true
                    };

                return
                    new NetTcpBinding
                    {
                        Name = ServiceName,
                        Namespace = string.Format(@"http://{0}", Namespace),
                        ReliableSession = optionalReliableSession,
                        Security = Security,
                        SendTimeout = SendTimeout,
                        ReceiveTimeout = ReceiveTimeout,
                        OpenTimeout = OpenTimeout,
                        CloseTimeout = CloseTimeout,
                        TransactionFlow = TransactionFlow,
                        MaxBufferSize = MaxBufferSize,
                        MaxBufferPoolSize = MaxBufferPoolSize,
                        MaxReceivedMessageSize = MaxReceivedMessageSize,
                        ReaderQuotas = ReaderQuotas
                    };
            }
        }

        public virtual EndpointAddress EndpointAddress
        {
            get
            {
                Uri uri = new Uri(Address, UriKind.Absolute);

                EndpointIdentity identity = null;
                if (!string.IsNullOrWhiteSpace(ServicePrincipalName))
                {
                    identity = EndpointIdentity.CreateSpnIdentity(ServicePrincipalName);
                }
                else if (!string.IsNullOrWhiteSpace(UserPrincipalName))
                {
                    identity = EndpointIdentity.CreateUpnIdentity(UserPrincipalName);
                }
                else if (!string.IsNullOrWhiteSpace(Dns))
                {
                    identity = EndpointIdentity.CreateDnsIdentity(Dns);
                }

                return identity != null
                           ? new EndpointAddress(uri, identity)
                           : new EndpointAddress(uri);
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