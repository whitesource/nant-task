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
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Whitesource.Agent.Api
{
    public sealed class ChecksumUtils
    {

        /* --- Constructors --- */

        /**
         * Private default constructor
         */
        private ChecksumUtils()
        {
        }

        /* --- Static methods --- */

        /**
         * Calculates the given file's SHA-1 hash code.
         *
         * @param filePath Path to the given file.
         *
         * @return Calculated SHA-1 for the given file.
         */
        public static String GetSha1Hash(String filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                SHA1 sha = new SHA1Managed();
                byte[] hash = sha.ComputeHash(fs);
                StringBuilder builder = new StringBuilder(2 * hash.Length);
                foreach (byte b in hash)
                {
                    builder.AppendFormat("{0:X2}", b);
                }
                return builder.ToString();
            }
        }
    }
}

