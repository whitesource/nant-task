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
using System.Runtime.Serialization;

namespace Whitesource.Agent.Api.Model
{
    /**
     * Info object describing a policy for inventory requests.
     *
     * @author tom.shapira
     */
    [DataContract]
    public class RequestPolicyInfo
    {

        /* --- Members --- */

        [DataMember(Name = "displayName")]
        public String DisplayName { get; set; }

        [DataMember(Name = "filterType")]
        public String FilterType { get; set; }

        [DataMember(Name = "filterLogic")]
        public String FilterLogic { get; set; }

        [DataMember(Name = "actionType")]
        public String ActionType { get; set; }

        [DataMember(Name = "actionLogic")]
        public String ActionLogic { get; set; }

        [DataMember(Name = "projectLevel")]
        public bool ProjectLevel { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public RequestPolicyInfo()
        {
        }

        /**
         * Constructor
         *
         * @param displayName
         */
        public RequestPolicyInfo(String displayName)
        {
            this.DisplayName = displayName;
        }

    }
}
