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

namespace PPWCode.Vernacular.Wcf.I
{
    [DataContract(IsReference = true), Serializable]
    public class WcfDto : IExtensibleDataObject
    {
        /// <summary>
        ///     Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Runtime.Serialization.ExtensionDataObject" /> that contains data that is not recognized as
        ///     belonging to the data contract.
        /// </returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}