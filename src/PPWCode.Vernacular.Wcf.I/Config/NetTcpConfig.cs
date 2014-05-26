using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public class NetTcpConfig<T> : NetConfigBase<T>
        where T : class
    {
        protected const string PortKey = "Port";
        protected const string ServicePrincipalNameKey = "ServicePrincipalName";
        protected const string TransactionFlowKey = "TransactionFlow";
        protected const string InactivityTimeoutKey = "InactivityTimeout";
        protected const string MaxBufferSizeKey = "MaxBufferSize";
        protected const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";
        protected const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";

        public NetTcpConfig(string @namespace)
            : base(@namespace)
        {
        }

        private int DefaultPort
        {
            get { return ConfigHelper.GetAppSetting<int>(PortKey); }
        }

        private int Port
        {
            get { return GetAppSetting(PortKey, DefaultPort); }
        }

        private string DefaultInactivityTimeout
        {
            get { return ConfigHelper.GetAppSetting(InactivityTimeoutKey, "00:10:00"); }
        }

        private TimeSpan InactivityTimeout
        {
            get { return GetTimeout(InactivityTimeoutKey, DefaultInactivityTimeout); }
        }

        private int DefaultMaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, 65536); }
        }

        private int MaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, DefaultMaxBufferSize); }
        }

        private string ServicePrincipalName
        {
            get { return GetAppSetting(ServicePrincipalNameKey, (string)null); }
        }

        private bool DefaultTransactionFlow
        {
            get { return ConfigHelper.GetAppSetting(TransactionFlowKey, false); }
        }

        private bool TransactionFlow
        {
            get { return GetAppSetting(TransactionFlowKey, DefaultTransactionFlow); }
        }

        protected virtual int DefaultMaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, 512 * 1024); }
        }

        protected virtual int MaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, DefaultMaxBufferPoolSize); }
        }

        protected virtual int DefaultMaxReceivedMessageSize
        {
            get { return GetAppSetting(MaxReceivedMessageSizeKey, 65536); }
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

        protected virtual Binding Binding
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

                XmlDictionaryReaderQuotas readerQuotas =
                    new XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = MaxArrayLength,
                        MaxDepth = MaxDepth,
                        MaxStringContentLength = MaxStringContentLength,
                        MaxBytesPerRead = MaxBytesPerRead,
                        MaxNameTableCharCount = MaxNameTableCharCount
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
                        ReaderQuotas = readerQuotas
                    };
            }
        }

        public override IWcfClientModel GetClientModel(params object[] extensions)
        {
            Uri uri = new Uri(Address, UriKind.Absolute);

            EndpointIdentity identity =
                string.IsNullOrWhiteSpace(ServicePrincipalName)
                    ? null
                    : new SpnEndpointIdentity(ServicePrincipalName);
            EndpointAddress endpointAddress = new EndpointAddress(uri, identity);

            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(endpointAddress)
                    .AddExtensions(extensions);

            return new DefaultClientModel(endpoint);
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