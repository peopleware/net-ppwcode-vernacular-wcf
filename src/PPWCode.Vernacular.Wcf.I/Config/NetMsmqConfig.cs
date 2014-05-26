using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public class NetMsmqConfig<T> : NetConfigBase<T>
        where T : class
    {
        protected const string QueueNameKey = "QueueName";

        // Specific msmq binding propteries
        protected const string DurableKey = "Durable";
        protected const string ExactlyOnceKey = "ExactlyOnce";
        protected const string MaxRetryCyclesKey = "MaxRetryCycles";
        protected const string RetryCycleDelayKey = "RetryCycleDelay";
        protected const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";
        protected const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";

        private readonly bool m_IsPrivate;

        public NetMsmqConfig(string @namespace, bool isPrivate)
            : base(@namespace)
        {
            m_IsPrivate = isPrivate;
        }

        protected virtual string QueueName
        {
            get { return GetAppSetting(QueueNameKey, "QueueName"); }
        }

        protected virtual bool DefaultDurable
        {
            get { return GetAppSetting(DurableKey, true); }
        }

        protected virtual bool Durable
        {
            get { return GetAppSetting(DurableKey, DefaultDurable); }
        }

        protected virtual bool DefaultExactlyOnce
        {
            get { return GetAppSetting(ExactlyOnceKey, true); }
        }

        protected virtual bool ExactlyOnce
        {
            get { return GetAppSetting(ExactlyOnceKey, DefaultExactlyOnce); }
        }

        protected virtual int DefaultMaxRetryCycles
        {
            get { return GetAppSetting(MaxRetryCyclesKey, 2); }
        }

        protected virtual int MaxRetryCycles
        {
            get { return GetAppSetting(MaxRetryCyclesKey, DefaultMaxRetryCycles); }
        }

        protected virtual string DefaultRetryCycleDelay
        {
            get { return ConfigHelper.GetAppSetting(RetryCycleDelayKey, "00:10:00"); }
        }

        protected virtual TimeSpan RetryCycleDelay
        {
            get { return GetTimeout(RetryCycleDelayKey, DefaultRetryCycleDelay); }
        }

        protected virtual int DefaultMaxBufferPoolSize
        {
            get { return GetAppSetting(MaxBufferPoolSizeKey, 8); }
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

        public virtual string Address
        {
            get
            {
                return m_IsPrivate
                           ? string.Format(@"net.msmq://{0}/private/{1}", Host, QueueName)
                           : string.Format(@"net.msmq://{0}/{1}", Host, QueueName);
            }
        }

        protected virtual NetMsmqSecurity Security
        {
            get
            {
                return new NetMsmqSecurity
                       {
                           Mode = NetMsmqSecurityMode.Transport
                       };
            }
        }

        protected virtual Binding Binding
        {
            get
            {
                XmlDictionaryReaderQuotas readerQuotas =
                    new XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = MaxArrayLength,
                        MaxDepth = MaxDepth,
                        MaxStringContentLength = MaxStringContentLength,
                        MaxBytesPerRead = MaxBytesPerRead,
                        MaxNameTableCharCount = MaxNameTableCharCount
                    };

                return new NetMsmqBinding
                       {
                           Name = ServiceName,
                           Namespace = string.Format(@"http://{0}", Namespace),
                           Security = Security,
                           SendTimeout = SendTimeout,
                           ReceiveTimeout = ReceiveTimeout,
                           OpenTimeout = OpenTimeout,
                           CloseTimeout = CloseTimeout,
                           MaxBufferPoolSize = MaxBufferPoolSize,
                           ReaderQuotas = readerQuotas,
                           Durable = Durable,
                           ExactlyOnce = ExactlyOnce,
                           MaxReceivedMessageSize = MaxReceivedMessageSize,
                           MaxRetryCycles = MaxRetryCycles,
                           RetryCycleDelay = RetryCycleDelay,
                       };
            }
        }

        public override IWcfClientModel GetClientModel(params object[] extensions)
        {
            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(Address)
                    .AddExtensions(extensions);

            return new DefaultClientModel(endpoint);
        }

        public override IWcfServiceModel GetServiceModel(params object[] extensions)
        {
            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(Address)
                    .AddExtensions(extensions);

            return new DefaultServiceModel(endpoint);
        }
    }
}