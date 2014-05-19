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
        private const string HostKey = "Host";
        private const string PortKey = "Port";
        private const string ServicePrincipalNameKey = "ServicePrincipalName";
        private const string InactivityTimeoutKey = "InactivityTimeout";
        private const string SendTimeoutKey = "SendTimeout";
        private const string ReceiveTimeoutKey = "ReceiveTimeout";
        private const string OpenTimeoutKey = "OpenTimeout";
        private const string CloseTimeoutKey = "CloseTimeout";
        private const string MaxBufferSizeKey = "MaxBufferSize";
        private const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";
        private const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";
        private const string MaxArrayLengthKey = "MaxArrayLength";
        private const string MaxDepthKey = "MaxDepth";
        private const string MaxStringContentLengthKey = "MaxStringContentLength";
        private const string MaxBytesPerReadKey = "MaxBytesPerRead";
        private const string MaxNameTableCharCountKey = "MaxNameTableCharCount";

        private readonly string m_Namespace;

        static NetTcpConfig()
        {
            if (!typeof(T).IsInterface)
            {
                throw new Exception("Only interfaces are allowed");
            }
        }

        public NetTcpConfig(string @namespace)
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException("namespace");
            }
            Contract.EndContractBlock();

            m_Namespace = @namespace;
        }

        private string ServiceName
        {
            get { return typeof(T).Name; }
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

        private string DefaultSendTimeout
        {
            get { return ConfigHelper.GetAppSetting(SendTimeoutKey, "00:01:00"); }
        }

        private TimeSpan SendTimeout
        {
            get { return GetTimeout(SendTimeoutKey, DefaultSendTimeout); }
        }

        private string DefaultReceiveTimeout
        {
            get { return ConfigHelper.GetAppSetting(ReceiveTimeoutKey, "00:10:00"); }
        }

        private TimeSpan ReceiveTimeout
        {
            get { return GetTimeout(ReceiveTimeoutKey, DefaultReceiveTimeout); }
        }

        private string DefaultOpenTimeout
        {
            get { return ConfigHelper.GetAppSetting(OpenTimeoutKey, "00:01:00"); }
        }

        private TimeSpan OpenTimeout
        {
            get { return GetTimeout(OpenTimeoutKey, DefaultOpenTimeout); }
        }

        private string DefaultCloseTimeout
        {
            get { return ConfigHelper.GetAppSetting(CloseTimeoutKey, "00:01:00"); }
        }

        private TimeSpan CloseTimeout
        {
            get { return GetTimeout(CloseTimeoutKey, DefaultCloseTimeout); }
        }

        private int DefaultMaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, 65536); }
        }

        private int MaxBufferSize
        {
            get { return GetAppSetting(MaxBufferSizeKey, DefaultMaxBufferSize); }
        }

        private int DefaultMaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, 512 * 1024); }
        }

        private int MaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, DefaultMaxBufferPoolSize); }
        }

        private int DefaultMaxReceivedMessageSize
        {
            get { return GetAppSetting(MaxReceivedMessageSizeKey, 65536); }
        }

        private int MaxReceivedMessageSize
        {
            get { return GetAppSetting(MaxReceivedMessageSizeKey, DefaultMaxReceivedMessageSize); }
        }

        private int DefaultMaxArrayLength
        {
            get { return GetAppSetting(MaxArrayLengthKey, 16384); }
        }

        private int MaxArrayLength
        {
            get { return GetAppSetting(MaxArrayLengthKey, DefaultMaxArrayLength); }
        }

        private int DefaultMaxDepth
        {
            get { return GetAppSetting(MaxDepthKey, 32); }
        }

        private int MaxDepth
        {
            get { return GetAppSetting(MaxDepthKey, DefaultMaxDepth); }
        }

        private int DefaultMaxStringContentLength
        {
            get { return GetAppSetting(MaxStringContentLengthKey, 8192); }
        }

        private int MaxStringContentLength
        {
            get { return GetAppSetting(MaxStringContentLengthKey, DefaultMaxStringContentLength); }
        }

        private int DefaultMaxBytesPerRead
        {
            get { return GetAppSetting(MaxBytesPerReadKey, 4096); }
        }

        private int MaxBytesPerRead
        {
            get { return GetAppSetting(MaxBytesPerReadKey, DefaultMaxBytesPerRead); }
        }

        private int DefaultMaxNameTableCharCount
        {
            get { return GetAppSetting(MaxNameTableCharCountKey, 16384); }
        }

        private int MaxNameTableCharCount
        {
            get { return GetAppSetting(MaxNameTableCharCountKey, DefaultMaxNameTableCharCount); }
        }

        private string DefaultHost
        {
            get { return ConfigHelper.GetAppSetting(HostKey, "localhost"); }
        }

        private string Host
        {
            get
            {
                string host = GetAppSetting<string>(HostKey, null);
                return string.IsNullOrWhiteSpace(host) ? DefaultHost : host;
            }
        }

        private string ServicePrincipalName
        {
            get { return GetAppSetting(ServicePrincipalNameKey, (string)null); }
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