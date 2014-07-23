﻿// Copyright 2014 by PeopleWare n.v..
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

using PPWCode.Util.OddsAndEnds.II.ConfigHelper;

namespace PPWCode.Vernacular.Wcf.I.Config
{
    public class HttpBindingConfig<T> : BasicHttpConfigBase<T>
        where T : class
    {
        public HttpBindingConfig(string @namespace)
            : base(@namespace)
        {
        }

        protected override int DefaultPort
        {
            get { return ConfigHelper.GetAppSetting(PortKey, 80); }
        }

        protected override sealed string Protocol
        {
            get { return "http"; }
        }
    }
}