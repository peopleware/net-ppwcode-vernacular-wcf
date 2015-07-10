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
using System.ServiceModel;

namespace PPWCode.Vernacular.Wcf.I.Tests
{
    [ServiceContract]
    public interface IOperations2
    {
        [OperationContract]
        int GetInt();
    }

    public class Operations2 : IOperations2
    {
        private int m_Value;

        public static readonly IList<int> s_GetIntResults = new List<int>();

        public int GetInt()
        {
            int result = ++m_Value;
            s_GetIntResults.Add(result);
            return result;
        }
    }
}