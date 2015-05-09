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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.ServiceModel;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    [ServiceContract]
    public interface IOperations
    {
        [OperationContract]
        int GetInt();

        [OperationContract]
        byte[] GetByteArrayInt(long size);

        [OperationContract]
        void ThrowSemanticException();

        [OperationContract]
        void ThrowNonSerializableException();
    }

    public class Operations : IOperations
    {
        private int m_Value;

        public static readonly IList<int> s_GetIntResults = new List<int>();

        public int GetInt()
        {
            int result = ++m_Value;
            s_GetIntResults.Add(result);
            return result;
        }

        public byte[] GetByteArrayInt(long size)
        {
            Contract.Requires(size >= 0);

            byte[] result = new byte[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = (byte)(i % 256);
            }
            return result;
        }

        public void ThrowSemanticException()
        {
            throw new SemanticException("Throw semantic exception");
        }

        public void ThrowNonSerializableException()
        {
            throw new NonSerializableException("Throw non serializable exception");
        }
    }

    public class NonSerializableException : SemanticException
    {
        /// <ensures csharp="this.Message == message" vb="Me.Message = message ">this.Message == message</ensures>
        public NonSerializableException(string message)
            : base(message)
        {
        }
    }
}