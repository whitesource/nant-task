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

namespace Whitesource.Agent.Api.Dispatch
{
    [DataContract]
    public class UpdateInventoryResult
    {

        /* --- Members --- */

        [DataMember(Name = "organization")]
        public String organization { get; set; }

        [DataMember(Name = "updatedProjects")]
        public List<String> updatedProjects  { get; set; }

        [DataMember(Name = "createdProjects")]
        public List<String> createdProjects { get; set; }

        /* --- Constructors --- */

        /**
	     * Default constructor (for JSON parsing)
	     */
        public UpdateInventoryResult()
        {
            updatedProjects = new List<String>();
            createdProjects = new List<String>();
        }

        /**
	     * Constructor
	     * 
	     * @param organization Name of the domain.
	     */
        public UpdateInventoryResult(String organization)
        {
            updatedProjects = new List<String>();
            createdProjects = new List<String>();
            this.organization = organization;
        }
    }
}