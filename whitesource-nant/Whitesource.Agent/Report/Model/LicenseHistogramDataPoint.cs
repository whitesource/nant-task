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

namespace Whitesource.Agent.Report.Model
{
    /**
     * Details object for specific data point in a license histogram.
     * 
     * @author tom.shapira
     */
    public class LicenseHistogramDataPoint
    {
        /* --- Static members --- */

        private const int LICENSE_NAME_MAX_LENGTH = 16;

        /* --- Members --- */

        public String Name { get; set; }

        public int Occurrences { get; set; }

        public String Height { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public LicenseHistogramDataPoint()
        {
            Occurrences = 0;
        }

        /**
         * Constructor
         * @param name
         * @param occurrences
         */
        public LicenseHistogramDataPoint(String name, int occurrences)
        {
            this.Name = name;
            this.Occurrences = occurrences;
        }

        /* --- Public methods --- */
        public String GetShortName()
        {
            String shortName = Name;
            if (shortName.Length > LICENSE_NAME_MAX_LENGTH)
            {
                shortName = shortName.Substring(0, LICENSE_NAME_MAX_LENGTH - 2) + "..";
            }
            return shortName;
        }
    }
}
