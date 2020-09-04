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
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Castle.Core.Logging;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public sealed class LogErrorHandler : IErrorHandler
    {
        private readonly ILogger m_Logger;

        public LogErrorHandler(ILogger logger)
        {
            m_Logger = logger;
        }

        private ILogger Logger
        {
            get { return m_Logger; }
        }

        bool IErrorHandler.HandleError(Exception error)
        {
            string message = string.Empty;

            try
            {
                message = CreateLogbookentry(error, null).ToString();
            }
            catch (Exception e)
            {
                Logger.Error("An exception occurred while handling the error", e);
            }

            if (error is SemanticException)
            {
                Logger.Info(message, error);
            }
            else
            {
                Logger.Error(message, error);
            }

            return false;
        }

        void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        private static ExceptionLogbookEntry CreateLogbookentry(Exception error, MessageFault fault)
        {
            string typeName;
            string methodName;

            string assemblyName = typeName = methodName = "Unknown";

            try
            {
                if (error.TargetSite != null)
                {
                    assemblyName = error.TargetSite.Module.Assembly.GetName().Name;
                    methodName = error.TargetSite.Name;
                    if (error.TargetSite.DeclaringType != null)
                    {
                        typeName = error.TargetSite.DeclaringType.Name;
                    }
                }
            }
            catch (Exception)
            {
                assemblyName = "Could not reflect assembly name.";
            }

            string fileName = GetFileName(error);
            int lineNumber = GetLineNumber(error);
            string exceptionName = error.GetType().ToString();
            string exceptionMessage = error.Message;
            string providedFault = string.Empty;
            string providedMessage = string.Empty;

            if (fault != null)
            {
                if (fault.Code != null)
                {
                    providedFault = fault.Code.Name;
                }

                if (fault.Reason?.Translations != null && fault.Reason.Translations.Any())
                {
                    providedMessage = fault.Reason.Translations[0].Text;
                }
            }

            return new ExceptionLogbookEntry(assemblyName, fileName, lineNumber, typeName, methodName, exceptionName, exceptionMessage, providedFault, providedMessage);
        }

        private static string GetFileName(Exception error)
        {
            if (error.StackTrace == null)
            {
                return "Unavailable";
            }

            int originalLineIndex = error.StackTrace.IndexOf(":line", StringComparison.Ordinal);
            if (originalLineIndex == -1)
            {
                return "Unavailable";
            }

            string originalLine = error.StackTrace.Substring(0, originalLineIndex);
            string[] sections = originalLine.Split('\\');
            return sections[sections.Length - 1];
        }

        private static int GetLineNumber(Exception error)
        {
            if (error.StackTrace == null)
            {
                return 0;
            }

            string[] sections = error.StackTrace.Split(' ');
            int index = sections.TakeWhile(section => !section.EndsWith(":line")).Count();
            if (index == sections.Length)
            {
                return 0;
            }

            string lineNumber = sections[index + 1];
            int number;
            try
            {
                number = Convert.ToInt32(lineNumber.Substring(0, lineNumber.Length - 2));
            }
            catch (FormatException)
            {
                if (!int.TryParse(lineNumber, out number))
                {
                    number = 0;
                }
            }

            return number;
        }
    }
}