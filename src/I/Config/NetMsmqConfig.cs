// Copyright 2016 by PeopleWare n.v..
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
    public class NetMsmqConfig<T> : NetConfigBase<T>
        where T : class
    {
        protected const string QueueNameKey = "QueueName";

        // Specific msmq binding propteries
        protected const string DurableKey = "Durable";

        protected const string ExactlyOnceKey = "ExactlyOnce";

        protected const string MaxRetryCyclesKey = "MaxRetryCycles";

        protected const string RetryCycleDelayKey = "RetryCycleDelay";

        protected const string ReceiveRetryCountKey = "ReceiveRetryCount";

        protected const string ReceiveErrorHandlingKey = "ReceiveErrorHandling";

        protected const string MaxBufferPoolSizeKey = "MaxBufferPoolSize";

        protected const string MaxReceivedMessageSizeKey = "MaxReceivedMessageSize";

        protected const string DeadLetterQueueKey = "DeadLetterQueue";

        protected const string CustomDeadLetterQueueKey = "CustomDeadLetterQueue";

        protected const string TimeToLiveKey = "TimeToLive";

        private readonly bool m_IsPrivate;

        public NetMsmqConfig(string @namespace, bool isPrivate)
            : base(@namespace)
        {
            m_IsPrivate = isPrivate;
        }

        protected virtual bool DefaultDurable
        {
            get { return ConfigHelper.GetAppSetting(DurableKey, true); }
        }

        protected virtual bool Durable
        {
            get { return GetAppSetting(DurableKey, DefaultDurable); }
        }

        protected virtual bool DefaultExactlyOnce
        {
            get { return ConfigHelper.GetAppSetting(ExactlyOnceKey, true); }
        }

        protected virtual bool ExactlyOnce
        {
            get { return GetAppSetting(ExactlyOnceKey, DefaultExactlyOnce); }
        }

        protected virtual int DefaultMaxRetryCycles
        {
            get { return ConfigHelper.GetAppSetting(MaxRetryCyclesKey, 2); }
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

        protected virtual int DefaultReceiveRetryCount
        {
            get { return ConfigHelper.GetAppSetting(ReceiveRetryCountKey, 5); }
        }

        protected virtual int ReceiveRetryCount
        {
            get { return GetAppSetting(ReceiveRetryCountKey, DefaultReceiveRetryCount); }
        }

        protected virtual string DefaultReceiveErrorHandling
        {
            get { return ConfigHelper.GetAppSetting(ReceiveErrorHandlingKey, "Fault"); }
        }

        protected virtual ReceiveErrorHandling ReceiveErrorHandling
        {
            get
            {
                string result = GetAppSetting(ReceiveErrorHandlingKey, DefaultReceiveErrorHandling);
                return (ReceiveErrorHandling)Enum.Parse(typeof(ReceiveErrorHandling), result);
            }
        }

        protected virtual int DefaultMaxBufferPoolSize
        {
            get { return ConfigHelper.GetAppSetting(MaxBufferPoolSizeKey, 8); }
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

        protected virtual string DefaultDeadLetterQueue
        {
            get { return ConfigHelper.GetAppSetting(DeadLetterQueueKey, ExactlyOnce ? "System" : "None"); }
        }

        protected virtual DeadLetterQueue DeadLetterQueue
        {
            get
            {
                string result = GetAppSetting(DeadLetterQueueKey, DefaultDeadLetterQueue);
                return (DeadLetterQueue)Enum.Parse(typeof(DeadLetterQueue), result);
            }
        }

        protected virtual string DefaultCustomDeadLetterQueue
        {
            get { return ConfigHelper.GetAppSetting<string>(CustomDeadLetterQueueKey); }
        }

        protected virtual Uri CustomDeadLetterQueue
        {
            get
            {
                string result = GetAppSetting(CustomDeadLetterQueueKey, DefaultCustomDeadLetterQueue);
                return result != null ? new Uri(result, UriKind.Absolute) : null;
            }
        }

        protected virtual string DefaultTimeToLive
        {
            get { return ConfigHelper.GetAppSetting(TimeToLiveKey, "1.00:00:00"); }
        }

        protected virtual TimeSpan TimeToLive
        {
            get { return GetTimeout(TimeToLiveKey, DefaultTimeToLive); }
        }

        public virtual string QueueName
        {
            get { return GetAppSetting(QueueNameKey, "QueueName"); }
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

        public virtual Binding Binding
        {
            get
            {
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
                           ReaderQuotas = ReaderQuotas,
                           Durable = Durable,
                           ExactlyOnce = ExactlyOnce,
                           MaxReceivedMessageSize = MaxReceivedMessageSize,
                           MaxRetryCycles = MaxRetryCycles,
                           ReceiveRetryCount = ReceiveRetryCount,
                           RetryCycleDelay = RetryCycleDelay,
                           DeadLetterQueue = DeadLetterQueue,
                           ReceiveErrorHandling = ReceiveErrorHandling,
                           CustomDeadLetterQueue = CustomDeadLetterQueue,
                           TimeToLive = TimeToLive
                       };
            }
        }

        protected override DefaultClientModel CreateDefaultClientModel(params object[] extensions)
        {
            IWcfEndpoint endpoint =
                WcfEndpoint
                    .ForContract<T>()
                    .BoundTo(Binding)
                    .At(Address)
                    .AddExtensions(extensions);

            DefaultClientModel clientModel = new DefaultClientModel(endpoint);

            return clientModel;
        }

        protected override DefaultServiceModel CreateDefaultServiceModel(params object[] extensions)
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