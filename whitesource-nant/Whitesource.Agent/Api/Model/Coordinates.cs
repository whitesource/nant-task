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

using Whitesource.Agent.Api;
using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Api.Model
{
    /**
     * WhiteSource model for artifact's coordinates.
     *
     * @author tom.shapira
     */
    [DataContract]
    public class Coordinates
    {

        /* --- Members --- */

        [DataMember(Name = "groupId")]
        public String GroupId { get; set; }

        [DataMember(Name = "artifactId")]
        public String ArtifactId { get; set; }

        [DataMember(Name = "version")]
        public String Version { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public Coordinates()
        {
        }

        /**
         * Constructor
         *
         * @param groupId
         * @param artifactId
         * @param version
         */
        public Coordinates(String groupId, String artifactId, String version)
        {
            this.GroupId = groupId;
            this.ArtifactId = artifactId;
            this.Version = version;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Coordinates@").Append(GetHashCode().ToString("X4"))
                    .Append("[")
                    .Append("groupId= ").Append(GroupId).Append(",")
                    .Append("artifactId= ").Append(ArtifactId).Append(",")
                    .Append("version= ").Append(Version)
                    .Append(" ]");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int code = APIConstants.HASH_CODE_SEED;
            code = APIConstants.HASH_CODE_FACTOR * code + ((GroupId == null) ? 0 : GroupId.GetHashCode());
            code = APIConstants.HASH_CODE_FACTOR * code + ((ArtifactId == null) ? 0 : ArtifactId.GetHashCode());
            code = APIConstants.HASH_CODE_FACTOR * code + ((Version == null) ? 0 : Version.GetHashCode());

            return code;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }

            Coordinates other = (Coordinates)obj;

            return (GroupId == null) ? (other.GroupId == null) : GroupId.Equals(other.GroupId)
                    && ((ArtifactId == null) ? (other.ArtifactId == null) : ArtifactId.Equals(other.ArtifactId))
                    && ((Version == null) ? (other.Version == null) : Version.Equals(other.Version));
        }

    }
}