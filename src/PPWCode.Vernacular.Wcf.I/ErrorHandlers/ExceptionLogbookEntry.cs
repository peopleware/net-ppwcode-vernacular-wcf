using System;
using System.Diagnostics;
using System.Reflection;

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public struct ExceptionLogbookEntry
    {
        private readonly string m_AssemblyName;
        private readonly string m_Date;
        private readonly string m_Event;
        private readonly string m_ExceptionMessage;
        private readonly string m_ExceptionName;
        private readonly string m_FileName;
        private readonly string m_HostName;
        private readonly int m_LineNumber;
        private readonly string m_MachineName;
        private readonly string m_MemberAccessed;
        private readonly string m_ProvidedFault;
        private readonly string m_ProvidedMessage;
        private readonly string m_Time;
        private readonly string m_TypeName;

        public ExceptionLogbookEntry(string assemblyName, string fileName, int lineNumber, string typeName, string methodName, string exceptionName, string exceptionMessage, string providedFault, string providedMessage)
        {
            m_MachineName = Environment.MachineName;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            m_HostName = entryAssembly == null
                             ? Process.GetCurrentProcess().MainModule.ModuleName
                             : entryAssembly.GetName().Name;
            m_AssemblyName = assemblyName;
            m_FileName = fileName;
            m_LineNumber = lineNumber;
            m_TypeName = typeName;
            m_MemberAccessed = methodName;
            m_Date = DateTime.Now.ToShortDateString();
            m_Time = DateTime.Now.ToLongTimeString();
            m_ExceptionName = exceptionName;
            m_ExceptionMessage = exceptionMessage;
            m_ProvidedFault = providedFault;
            m_ProvidedMessage = providedMessage;
            m_Event = String.Empty;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "MachineName: {1}{0}" +
                    "HostName: {2}{0}" +
                    "AssemblyName: {3}{0}" +
                    "FileName: {4}{0}" +
                    "LineNumber: {5}{0}" +
                    "TypeName: {6}{0}" +
                    "MemberAccessed: {7}{0}" +
                    "Date: {8}{0}" +
                    "Time: {9}{0}" +
                    "ExceptionName: {10}{0}" +
                    "ExceptionMessage: {11}{0}" +
                    "ProvidedFault: {12}{0}" +
                    "ProvidedMessage: {13}{0}" +
                    "Event: {14}{0}",
                    Environment.NewLine,
                    m_MachineName,
                    m_HostName,
                    m_AssemblyName,
                    m_FileName,
                    m_LineNumber,
                    m_TypeName,
                    m_MemberAccessed,
                    m_Date,
                    m_Time,
                    m_ExceptionName,
                    m_ExceptionMessage,
                    m_ProvidedFault,
                    m_ProvidedMessage,
                    m_Event);
        }
    }
}