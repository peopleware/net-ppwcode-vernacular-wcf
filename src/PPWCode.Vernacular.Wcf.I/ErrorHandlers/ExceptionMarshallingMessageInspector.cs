/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace PPWCode.Vernacular.Wcf.I.ErrorHandlers
{
    public class ExceptionMarshallingMessageInspector :
        IClientMessageInspector
    {
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                // Create a copy of the original reply to allow default processing of the message
                MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
                Message copy = buffer.CreateMessage();  // Create a copy to work with
                reply = buffer.CreateMessage();         // Restore the original message

                object faultDetail = ReadFaultDetail(copy);
                Exception exception = faultDetail as Exception;
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        private static object ReadFaultDetail(Message reply)
        {
            const string DetailElementName = "Detail";

            using (XmlDictionaryReader reader = reply.GetReaderAtBodyContents())
            {
                // Find <soap:Detail>
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == DetailElementName)
                    {
                        break;
                    }
                }

                // Did we find it?
                if (reader.NodeType != XmlNodeType.Element || reader.LocalName != DetailElementName)
                {
                    return null;
                }

                // Move to the contents of <soap:Detail>
                if (!reader.Read())
                {
                    return null;
                }

                // Deserialize the fault
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                try
                {
                    return serializer.ReadObject(reader);
                }
                catch (FileNotFoundException)
                {
                    // Serializer was unable to find assembly where exception is defined
                    return null;
                }
                catch (SerializationException)
                {
                    // Error during deserialization
                    return null;
                }
            }
        }
    }
}
