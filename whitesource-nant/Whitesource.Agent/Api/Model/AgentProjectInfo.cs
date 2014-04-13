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
     * Class to hold all information about a project to update. 
     * 
     * @author tom.shapira
     */
    [DataContract]
    public class AgentProjectInfo
    {

        /* --- Members --- */

        [DataMember(Name = "coordinates")]
        public Coordinates Coordinates { get; set; }

        [DataMember(Name = "parentCoordinates")]
        public Coordinates ParentCoordinates { get; set; }

        [DataMember(Name = "dependencies")]
        public List<DependencyInfo> Dependencies { get; set; }

        [DataMember(Name = "projectToken")]
        public String ProjectToken { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         * 
         */
        public AgentProjectInfo()
        {
            Dependencies = new List<DependencyInfo>();
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("AgentProjectInfo@").Append(GetHashCode().ToString("X4"))
                .Append("[")
                .Append("coordinates= ").Append(Coordinates).Append(",")
                .Append("parentCoordinates= ").Append(ParentCoordinates).Append(",")
                .Append("projectToken= ").Append(ProjectToken)
                .Append(" ]");

            return sb.ToString();
        }

    }
}