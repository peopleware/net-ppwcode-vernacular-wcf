using System;
using System.Diagnostics.Contracts;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public sealed class NetTcpConfig<T>
        where T : class
    {
        private const string DefaultHost = "localhost";
        private const string DefaultInactivityTimeout = "00:10:00";
        private const string DefaultSendTimeout = "00:01:00";
        private const string DefaultReceiveTimeout = "00:10:00";
        private const string DefaultOpenTimeout = "00:01:00";
        private const string DefaultCloseTimeout = "00:01:00";
        private const int DefaultMaxBufferSize = 65536;
        private const int DefaultMaxBufferPoolSize = 512 * 1024;
        private const int DefaultMaxReceivedMessageSize = 65536;
        private const int DefaultMaxArrayLength = 16384;
        private const int DefaultMaxDepth = 32;
        private const int DefaultMaxStringContentLength = 8192;
        private const int DefaultMaxBytesPerRead = 4096;
        private const int DefaultMaxNameTableCharCount = 16384;
        private readonly int m_DefaultPort;
        private readonly string m_Namespace;

        static NetTcpConfig()
        {
            if (!typeof(T).IsInterface)
            {
                throw new Exception("Only interfaces are allowed");
            }
        }

        public NetTcpConfig(string @namespace, int defaultPort)
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException("namespace");
            }
            if (defaultPort < 1024)
            {
                throw new ArgumentOutOfRangeException("defaultPort", "Only port's higher then 1024 are allowed");
            }
            Contract.EndContractBlock();

            m_Namespace = @namespace;
            m_DefaultPort = defaultPort;
        }

        private string ServiceName
        {
            get { return typeof(T).Name; }
        }

        private int Port
        {
            get { return GetAppSetting("Port", DefaultPort); }
        }

        private TimeSpan InactivityTimeout
        {
            get { return GetTimeout("InactivityTimeout", DefaultInactivityTimeout); }
        }

        private TimeSpan SendTimeout
        {
            get { return GetTimeout("SendTimeout", DefaultSendTimeout); }
        }

        private TimeSpan ReceiveTimeout
        {
            get { return GetTimeout("ReceiveTimeout", DefaultReceiveTimeout); }
        }

        private TimeSpan OpenTimeout
        {
            get { return GetTimeout("OpenTimeout", DefaultOpenTimeout); }
        }

        private TimeSpan CloseTimeout
        {
            get { return GetTimeout("CloseTimeout", DefaultCloseTimeout); }
        }

        private int MaxBufferSize
        {
            get { return GetInteger("MaxBufferSize", DefaultMaxBufferSize); }
        }

        private int MaxBufferPoolSize
        {
            get { return GetInteger("MaxBufferPoolSize", DefaultMaxBufferPoolSize); }
        }

        private int MaxReceivedMessageSize
        {
            get { return GetInteger("MaxReceivedMessageSize", DefaultMaxReceivedMessageSize); }
        }

        private int MaxArrayLength
        {
            get { return GetInteger("MaxArrayLength", DefaultMaxArrayLength); }
        }

        private int MaxDepth
        {
            get { return GetInteger("MaxDepth", DefaultMaxDepth); }
        }

        private int MaxStringContentLength
        {
            get { return GetInteger("MaxStringContentLength", DefaultMaxStringContentLength); }
        }

        private int MaxBytesPerRead
        {
            get { return GetInteger("MaxBytesPerRead", DefaultMaxBytesPerRead); }
        }

        private int MaxNameTableCharCount
        {
            get { return GetInteger("MaxNameTableCharCount", DefaultMaxNameTableCharCount); }
        }

        private string Host
        {
            get
            {
                string host = GetAppSetting("Host", DefaultHost);
                return string.IsNullOrWhiteSpace(host) ? DefaultHost : host;
            }
        }

        private string ServicePrincipalName
        {
            get { return GetAppSetting("ServicePrincipalName", (string)null); }
        }

        private string BaseAddress
        {
            get { return string.Format(@"net.tcp://{0}:{1}/{2}", Host, Port, Namespace); }
        }

        private string Address
        {
            get { return string.Format("{0}/{1}", BaseAddress, ServiceName); }
        }

        private Binding Binding
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

                TcpTransportSecurity tcpTransportSecurity =
                    new TcpTransportSecurity
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows,
                        ProtectionLevel = ProtectionLevel.EncryptAndSign
                    };

                NetTcpSecurity security =
                    new NetTcpSecurity
                    {
                        Mode = SecurityMode.Transport,
                        Transport = tcpTransportSecurity
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
                        Namespace = string.Format(@"http://{0}/{1}", Namespace, ServiceName),
                        ReliableSession = optionalReliableSession,
                        Security = security,
                        SendTimeout = SendTimeout,
                        ReceiveTimeout = ReceiveTimeout,
                        OpenTimeout = OpenTimeout,
                        CloseTimeout = CloseTimeout,
                        TransactionFlow = true,
                        MaxBufferSize = MaxBufferSize,
                        MaxBufferPoolSize = MaxBufferPoolSize,
                        MaxReceivedMessageSize = MaxReceivedMessageSize,
                        ReaderQuotas = readerQuotas
                    };
            }
        }

        private string Namespace
        {
            get { return m_Namespace; }
        }

        private int DefaultPort
        {
            get { return m_DefaultPort; }
        }

        private TValue GetAppSetting<TValue>(string key, TValue defaultValue)
            where TValue : IConvertible
        {
            return ConfigHelper.GetAppSetting(string.Concat(ServiceName, "_", key), defaultValue);
        }

        private TimeSpan GetTimeout(string key, string defaultTimeout)
        {
            string timeSpanAsString = ConfigHelper.GetAppSetting(string.Concat(ServiceName, "_", key), defaultTimeout);
            TimeSpan timeSpan;
            if (!TimeSpan.TryParse(timeSpanAsString, out timeSpan))
            {
                timeSpan = TimeSpan.Parse(defaultTimeout);
            }
            return timeSpan;
        }

        private int GetInteger(string key, int defaultInt)
        {
            return ConfigHelper.GetAppSetting(string.Concat(ServiceName, "_", key), defaultInt);
        }

        public IWcfClientModel GetClientModel(params object[] extensions)
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

        public IWcfServiceModel GetServiceModel(params object[] extensions)
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