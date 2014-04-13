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
using System.Text;
using System.Runtime.Serialization;

namespace Whitesource.Agent.Api.Model
{
    /**
     * WhiteSource model for exclusion of transitive dependencies. 
     * 
     * @author tom.shapira
     */
    [DataContract]
    public class ExclusionInfo
    {

        /* --- Members --- */

        [DataMember(Name = "artifactId")]
        public String ArtifactId { get; set; }

        [DataMember(Name = "groupId")]
        public String GroupId { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public ExclusionInfo()
        {
        }

        /**
         * Constructor
         * 
         * @param artifactId
         * @param groupId
         */
        public ExclusionInfo(String artifactId, String groupId)
        {
            this.ArtifactId = artifactId;
            this.GroupId = groupId;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ExclusionInfo@").Append(GetHashCode().ToString("X4"))
                .Append("[")
                .Append("groupId= ").Append(GroupId).Append(",")
                .Append("artifactId= ").Append(ArtifactId)
                .Append(" ]");

            return sb.ToString();
        }

    }
}