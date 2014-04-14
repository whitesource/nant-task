/**
 * Copyright (C) 2012 White Source Ltd.
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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Whitesource.Agent.Api.Model
{
    /**
     * Info object describing a managed resource.
     *
     * @author tom.shapira
     */
    [DataContract]
    public class ResourceInfo
    {

        /* --- Members --- */

        [DataMember(Name = "displayName")]
        public String DisplayName { get; set; }

        [DataMember(Name = "link")]
        public String Link { get; set; }

        [DataMember(Name = "licenses")]
        public List<String> Licenses { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public ResourceInfo()
        {
            Licenses = new List<String>();
        }

        /**
         * Constructor
         *
         * @param displayName
         */
        public ResourceInfo(String displayName)
            : this()
        {
            this.DisplayName = displayName;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            return DisplayName;
        }

    }
}
