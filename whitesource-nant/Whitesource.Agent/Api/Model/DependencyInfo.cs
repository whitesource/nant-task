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
        public String GroupId { get; set; }

        [DataMember(Name = "artifactId")]
        public String ArtifactId { get; set; }

        [DataMember(Name = "version")]
        public String Version { get; set; }

        [DataMember(Name = "type")]
        public String Type { get; set; }

        [DataMember(Name = "classifier")]
        public String Classifier { get; set; }

        [DataMember(Name = "scope")]
        public String Scope { get; set; }

        [DataMember(Name = "sha1")]
        public String Sha1 { get; set; }

        [DataMember(Name = "systemPath")]
        public String SystemPath { get; set; }

        [DataMember(Name = "exclusions")]
        public List<ExclusionInfo> Exclusions { get; set; }

        [DataMember(Name = "optional")]
        public bool Optional { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public DependencyInfo()
        {
            Exclusions = new List<ExclusionInfo>();
        }

        /**
         * Constructor
         * 
         * @param groupId
         * @param artifactId
         * @param version
         */
        public DependencyInfo(String groupId, String artifactId, String version)
            : this()
        {
            this.GroupId = groupId;
            this.ArtifactId = artifactId;
            this.Version = version;
        }

        /**
         * Constructor
         * 
         * @param sha1
         */
        public DependencyInfo(String sha1)
            : this()
        {
            this.Sha1 = sha1;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("DependencyInfo@").Append(GetHashCode().ToString("X4"))
                .Append("[")
                .Append("groupId= ").Append(GroupId).Append(",")
                .Append("artifactId= ").Append(ArtifactId).Append(",")
                .Append("version= ").Append(Version)
                .Append(" ]");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return new Coordinates(GroupId, ArtifactId, Version).GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }

            DependencyInfo other = (DependencyInfo)obj;

            return (GroupId == null) ? (other.GroupId == null) : GroupId.Equals(other.GroupId)
                    && ((ArtifactId == null) ? (other.ArtifactId == null) : ArtifactId.Equals(other.ArtifactId))
                    && ((Version == null) ? (other.Version == null) : Version.Equals(other.Version));
        }

    }
}