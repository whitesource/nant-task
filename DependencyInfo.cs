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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Api.Model
{
    /**
     * WhiteSource Model for a project's dependency 
     * 
     * @author tom.shapira
     */
    [DataContract]
    public class DependencyInfo
    {

        /* --- Members --- */

        [DataMember(Name = "groupId")]
        public String groupId { get; set; }

        [DataMember(Name = "artifactId")]
        public String artifactId { get; set; }

        [DataMember(Name = "version")]
        public String version { get; set; }

        [DataMember(Name = "type")]
        public String type { get; set; }

        [DataMember(Name = "classifier")]
        public String classifier { get; set; }

        [DataMember(Name = "scope")]
        public String scope { get; set; }

        [DataMember(Name = "sha1")]
        public String sha1 { get; set; }

        [DataMember(Name = "systemPath")]
        public String systemPath { get; set; }

        [DataMember(Name = "exclusions")]
        public List<ExclusionInfo> exclusions { get; set; }

        [DataMember(Name = "optional")]
        public bool optional { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public DependencyInfo()
        {
            exclusions = new List<ExclusionInfo>();
        }

        /**
         * Constructor
         * 
         * @param groupId
         * @param artifactId
         * @param version
         */
        public DependencyInfo(String groupId, String artifactId, String version)
        {
            exclusions = new List<ExclusionInfo>();
            this.groupId = groupId;
            this.artifactId = artifactId;
            this.version = version;
        }

        /**
         * Constructor
         * 
         * @param sha1
         */
        public DependencyInfo(String sha1)
        {
            exclusions = new List<ExclusionInfo>();
            this.sha1 = sha1;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("DependencyInfo@").Append(GetHashCode().ToString("X4"))
                .Append("[")
                .Append("groupId= ").Append(groupId).Append(",")
                .Append("artifactId= ").Append(artifactId).Append(",")
                .Append("version= ").Append(version)
                .Append(" ]");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return new Coordinates(groupId, artifactId, version).GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }

            DependencyInfo other = (DependencyInfo)obj;

            return (groupId == null) ? (other.groupId == null) : groupId.Equals(other.groupId)
                    && ((artifactId == null) ? (other.artifactId == null) : artifactId.Equals(other.artifactId))
                    && ((version == null) ? (other.version == null) : version.Equals(other.version));
        }

    }
}