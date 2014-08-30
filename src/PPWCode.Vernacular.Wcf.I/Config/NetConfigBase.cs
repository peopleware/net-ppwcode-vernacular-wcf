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
using System.Diagnostics.Contracts;
using System.Xml;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;
using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public abstract class NetConfigBase<T>
        where T : class
    {
        protected const string HostKey = "Host";
        protected const string OpenOnDemandKey = "OpenOnDemand";
        public const string AsyncCapabilityKey = "AsyncCapability";

        // Binding timeout properties
        protected const string OpenTimeoutKey = "OpenTimeout";
        protected const string CloseTimeoutKey = "CloseTimeout";
        protected const string SendTimeoutKey = "SendTimeout";
        protected const string ReceiveTimeoutKey = "ReceiveTimeout";

        // ReaderQuotas
        protected const string MaxArrayLengthKey = "MaxArrayLength";
        protected const string MaxDepthKey = "MaxDepth";
        protected const string MaxStringContentLengthKey = "MaxStringContentLength";
        protected const string MaxBytesPerReadKey = "MaxBytesPerRead";
        protected const string MaxNameTableCharCountKey = "MaxNameTableCharCount";

        private readonly string m_Namespace;

        static NetConfigBase()
        {
            if (!typeof(T).IsInterface)
            {
                throw new ProgrammingError("Only interfaces are allowed");
            }
        }

        protected NetConfigBase(string @namespace)
        {
            Contract.Requires(@namespace != null);

            m_Namespace = @namespace;
        }

        protected virtual string Namespace
        {
            get { return m_Namespace; }
        }

        protected virtual string ServiceName
        {
            get { return typeof(T).Name; }
        }

        protected virtual bool DefaultOpenOnDemand
        {
            get { return ConfigHelper.GetAppSetting(OpenOnDemandKey, true); }
        }

        protected virtual bool OpenOnDemand
        {
            get { return GetAppSetting(OpenOnDemandKey, DefaultOpenOnDemand); }
        }

        protected virtual bool DefaultAsyncCapability
        {
            get { return ConfigHelper.GetAppSetting(AsyncCapabilityKey, true); }
        }

        protected virtual bool AsyncCapability
        {
            get { return GetAppSetting(AsyncCapabilityKey, DefaultAsyncCapability); }
        }

        protected virtual string DefaultHost
        {
            get { return ConfigHelper.GetAppSetting(HostKey, "localhost"); }
        }

        protected virtual string Host
        {
            get
            {
                string host = GetAppSetting<string>(HostKey, null);
                return string.IsNullOrWhiteSpace(host) ? DefaultHost : host;
            }
        }

        protected virtual string DefaultSendTimeout
        {
            get { return ConfigHelper.GetAppSetting(SendTimeoutKey, "00:01:00"); }
        }

        protected virtual TimeSpan SendTimeout
        {
            get { return GetTimeout(SendTimeoutKey, DefaultSendTimeout); }
        }

        protected virtual string DefaultReceiveTimeout
        {
            get { return ConfigHelper.GetAppSetting(ReceiveTimeoutKey, "00:10:00"); }
        }

        protected virtual TimeSpan ReceiveTimeout
        {
            get { return GetTimeout(ReceiveTimeoutKey, DefaultReceiveTimeout); }
        }

        protected virtual string DefaultOpenTimeout
        {
            get { return ConfigHelper.GetAppSetting(OpenTimeoutKey, "00:01:00"); }
        }

        protected virtual TimeSpan OpenTimeout
        {
            get { return GetTimeout(OpenTimeoutKey, DefaultOpenTimeout); }
        }

        protected virtual string DefaultCloseTimeout
        {
            get { return ConfigHelper.GetAppSetting(CloseTimeoutKey, "00:01:00"); }
        }

        protected virtual TimeSpan CloseTimeout
        {
            get { return GetTimeout(CloseTimeoutKey, DefaultCloseTimeout); }
        }

        protected virtual int DefaultMaxArrayLength
        {
            get { return ConfigHelper.GetAppSetting(MaxArrayLengthKey, 16384); }
        }

        protected virtual int MaxArrayLength
        {
            get { return GetAppSetting(MaxArrayLengthKey, DefaultMaxArrayLength); }
        }

        protected virtual int DefaultMaxDepth
        {
            get { return ConfigHelper.GetAppSetting(MaxDepthKey, 32); }
        }

        protected virtual int MaxDepth
        {
            get { return GetAppSetting(MaxDepthKey, DefaultMaxDepth); }
        }

        protected virtual int DefaultMaxStringContentLength
        {
            get { return ConfigHelper.GetAppSetting(MaxStringContentLengthKey, 8192); }
        }

        protected virtual int MaxStringContentLength
        {
            get { return GetAppSetting(MaxStringContentLengthKey, DefaultMaxStringContentLength); }
        }

        protected virtual int DefaultMaxBytesPerRead
        {
            get { return ConfigHelper.GetAppSetting(MaxBytesPerReadKey, 4096); }
        }

        protected virtual int MaxBytesPerRead
        {
            get { return GetAppSetting(MaxBytesPerReadKey, DefaultMaxBytesPerRead); }
        }

        protected virtual int DefaultMaxNameTableCharCount
        {
            get { return ConfigHelper.GetAppSetting(MaxNameTableCharCountKey, 16384); }
        }

        protected virtual int MaxNameTableCharCount
        {
            get { return GetAppSetting(MaxNameTableCharCountKey, DefaultMaxNameTableCharCount); }
        }

        protected TValue GetAppSetting<TValue>(string key, TValue defaultValue)
            where TValue : IConvertible
        {
            return ConfigHelper.GetAppSetting(string.Concat(ServiceName, "_", key), defaultValue);
        }

        protected TimeSpan GetTimeout(string key, string defaultTimeout)
        {
            string timeSpanAsString = ConfigHelper.GetAppSetting(string.Concat(ServiceName, "_", key), defaultTimeout);
            TimeSpan timeSpan;
            if (!TimeSpan.TryParse(timeSpanAsString, out timeSpan))
            {
                timeSpan = TimeSpan.Parse(defaultTimeout);
            }

            return timeSpan;
        }

        protected TimeSpan GetTimeout(string key, TimeSpan defaultTimeout)
        {
            return GetTimeout(key, defaultTimeout.ToString());
        }

        protected virtual XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return
                    new XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = MaxArrayLength,
                        MaxDepth = MaxDepth,
                        MaxStringContentLength = MaxStringContentLength,
                        MaxBytesPerRead = MaxBytesPerRead,
                        MaxNameTableCharCount = MaxNameTableCharCount
                    };
            }
        }

        public abstract IWcfClientModel GetClientModel(params object[] extensions);

        public abstract IWcfServiceModel GetServiceModel(params object[] extensions);
    }
}