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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public class ExceptionMarshallingErrorHandler : IErrorHandler
    {
        bool IErrorHandler.HandleError(Exception error)
        {
            return !(error is FaultException);
        }

        void IErrorHandler.ProvideFault(Exception e, MessageVersion version, ref Message fault)
        {
            if (!(e is FaultException))
            {
                Type exceptionType = e.GetType();
                if (!exceptionType.IsDefined(typeof(SerializableAttribute), false))
                {
                    // Exception isn´t serializable, blame the programmer
                    e = HandleNonSerializableException(e);
                }

                // Generate fault message manually
                MessageFault messageFault = MessageFault.CreateFault(
                    new FaultCode("Sender"),
                    new FaultReason(e.Message),
                    e,
                    new NetDataContractSerializer());
                fault = Message.CreateMessage(version, messageFault, null);
            }
        }

        private static Exception HandleNonSerializableException(Exception e)
        {
            string msg = e.Message;
            msg = string.Format(
                "Exception of type {1} wasn't serializable, rethrown as plain exception.{0}{0}Original message:{0}{2}{0}{0}Original Stacktrace:{0}{3}",
                Environment.NewLine,
                e.GetType().Name,
                msg,
                e.StackTrace);

            Exception inner = e.InnerException;
            while (inner != null)
            {
                msg = string.Format(
                    "{1}{0}{0}InnerException:{0}{2}{0}{0}StackTrace:{0}{3}",
                    Environment.NewLine,
                    msg,
                    inner.Message,
                    inner.StackTrace);
                inner = inner.InnerException;
            }

            return new ProgrammingError(msg);
        }
    }
}