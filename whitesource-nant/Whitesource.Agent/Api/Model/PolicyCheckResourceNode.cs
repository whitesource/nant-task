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
     * A node for each resource in a dependency graph with associated policy application results.
     *
     * @author tom.shapira
     */
    [DataContract]
    public class PolicyCheckResourceNode
    {

        /* --- Members --- */

        [DataMember(Name = "resource")]
        public ResourceInfo Resource { get; set; }

        [DataMember(Name = "policy")]
        public RequestPolicyInfo Policy { get; set; }

        [DataMember(Name = "children")]
        public List<PolicyCheckResourceNode> Children { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public PolicyCheckResourceNode()
        {
            Children = new List<PolicyCheckResourceNode>();
        }

        /**
         * Constructor
         *
         * @param resource
         * @param policy
         */
        public PolicyCheckResourceNode(ResourceInfo resource, RequestPolicyInfo policy)
            : this()
        {
            this.Resource = resource;
            this.Policy = policy;
        }

        /* --- Public methods --- */

        public bool HasRejections()
        {
            bool rejections = Policy != null && "Reject".Equals(Policy.ActionType);

            IEnumerator<PolicyCheckResourceNode> enumerator = Children.GetEnumerator();
            while (enumerator.MoveNext() && !rejections)
            {
                rejections = enumerator.Current.HasRejections();
            }

            return rejections;
        }

    }
}
