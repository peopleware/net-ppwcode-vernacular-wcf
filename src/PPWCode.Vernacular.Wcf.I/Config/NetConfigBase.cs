using System;
using System.Diagnostics.Contracts;

using Castle.Facilities.WcfIntegration;

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;
using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public abstract class NetConfigBase<T>
        where T : class
    {
        private const string HostKey = "Host";

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
            if (@namespace == null)
            {
                throw new ArgumentNullException("namespace");
            }
            Contract.EndContractBlock();

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
            get { return GetAppSetting(MaxArrayLengthKey, 16384); }
        }

        protected virtual int MaxArrayLength
        {
            get { return GetAppSetting(MaxArrayLengthKey, DefaultMaxArrayLength); }
        }

        protected virtual int DefaultMaxDepth
        {
            get { return GetAppSetting(MaxDepthKey, 32); }
        }

        protected virtual int MaxDepth
        {
            get { return GetAppSetting(MaxDepthKey, DefaultMaxDepth); }
        }

        protected virtual int DefaultMaxStringContentLength
        {
            get { return GetAppSetting(MaxStringContentLengthKey, 8192); }
        }

        protected virtual int MaxStringContentLength
        {
            get { return GetAppSetting(MaxStringContentLengthKey, DefaultMaxStringContentLength); }
        }

        protected virtual int DefaultMaxBytesPerRead
        {
            get { return GetAppSetting(MaxBytesPerReadKey, 4096); }
        }

        protected virtual int MaxBytesPerRead
        {
            get { return GetAppSetting(MaxBytesPerReadKey, DefaultMaxBytesPerRead); }
        }

        protected virtual int DefaultMaxNameTableCharCount
        {
            get { return GetAppSetting(MaxNameTableCharCountKey, 16384); }
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

        public abstract IWcfClientModel GetClientModel(params object[] extensions);
        public abstract IWcfServiceModel GetServiceModel(params object[] extensions);
    }
}