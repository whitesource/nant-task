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
using System.Runtime.Serialization;

using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Api.Dispatch
{
    /**
     * Result of the check policies operation.
     * 
     * @author tom.shapira
     */
    [DataContract]
    public class CheckPoliciesResult
    {

        /* --- Members --- */

        /**
         * Name of organization in.
         */
        [DataMember(Name = "organization")]
        public String Organization { get; set; }

        /**
         * Map of project name to the root of its dependency graph with policies application result.
         */
        [DataMember(Name = "existingProjects")]
        public Dictionary<String, PolicyCheckResourceNode> ExistingProjects { get; set; }

        /**
         * Map of project name to the root of its dependency graph with policies application result.
         */
        [DataMember(Name = "newProjects")]
        public Dictionary<String, PolicyCheckResourceNode> NewProjects { get; set; }

        /**
         * Map of project name to its set of new resources to insert into inventory.
         */
        [DataMember(Name = "projectNewResources")]
        public Dictionary<String, List<ResourceInfo>> ProjectNewResources { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public CheckPoliciesResult()
        {
            ExistingProjects = new Dictionary<String, PolicyCheckResourceNode>();
            NewProjects = new Dictionary<String, PolicyCheckResourceNode>();
            ProjectNewResources = new Dictionary<String, List<ResourceInfo>>();
        }

        /**
         * Constructor
         *
         * @param organization Name of the domain.
         */
        public CheckPoliciesResult(String organization)
            : this()
        {
            this.Organization = organization;
        }

        /* --- Public methods --- */

        /**
         * @return True if some project in this result have some rejected dependency.
         */
        public bool HasRejections()
        {
            bool hasRejections = false;

            List<PolicyCheckResourceNode> roots = new List<PolicyCheckResourceNode>();
            roots.AddRange(ExistingProjects.Values);
            roots.AddRange(NewProjects.Values);

            IEnumerator<PolicyCheckResourceNode> enumerator = roots.GetEnumerator();
            while (enumerator.MoveNext() && !hasRejections)
            {
                hasRejections = enumerator.Current.HasRejections();
            }

            return hasRejections;
        }
    }
}