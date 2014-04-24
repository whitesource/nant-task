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
using System.Collections.Generic;
using System.Text;
using System.IO;

using Whitesource.Agent.Api.Dispatch;
using Whitesource.Agent.Api.Model;
using Whitesource.Agent.Report.Model;

namespace Whitesource.Agent.Report
{

    /**
     * This class generates the HTML report.
     */
    public class ReportGenerator
    {

        /* --- Static members --- */

        private const float MAX_BAR_HEIGHT = 50;
        private const int LICENSE_LIMIT = 6;
        private const String OTHER_LICENSE = "Other types";

        private const String REJECT = "<span class='wssRed'>Rejections found</span>";
        private const String APPROVE = "<span class='wssGreen'>All approved</span>";
        private const String LINE_SEPARATOR = "<div class='wssLineSeparaterContainer'><div class='wssLineSeparator'></div></div>";

        /* --- Public methods --- */

        public void GenerateReport(CheckPoliciesResult result, String buildName, String buildNumber, StreamWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN' 'http://www.w3.org/TR/html4/loose.dtd'><html><head><title>White Source - Policy Check Summary</title><meta content='text/html; charset=iso-8859-1' http-equiv='Content-Type' />");
            writer.WriteLine("<script>function toggleDetails(anchor, detailsId){var details = document.getElementById(detailsId);if(details.style.display == 'block' ) {details.style.display = 'none';anchor.innerHTML = 'show details'} else {details.style.display = 'block';anchor.innerHTML = 'hide details'}return details;}</script>");
            writer.WriteLine("<link rel='stylesheet' href='wss.css'></head><body class='wssBody'><div align='center' style='width: 100%;'><div class='wssContainer'><a class='wssAnchor' href='http://www.whitesourcesoftware.com' target='_blank'><img align='left' src='http://saas.whitesourcesoftware.com/Wss/background/WhiteSource_Logo.png' style='left: 10;' /></a>");
            writer.WriteLine("<div align='center'><h1> &nbsp;</h1><h1 class='wssHeader'>Policy Check Summary</h1><h2>");

            if (result.HasRejections())
            {
                writer.WriteLine(REJECT);
            }
            else
            {
                writer.WriteLine(APPROVE);
            }
            writer.WriteLine("</h2><br/></div>");

            writer.WriteLine("<div align='left'><div class='wssTextAlign' style='width: 500px; padding-left: 10px;'><div><span class='wssGeneralIcon'></span><div class='wssProjectHeader' style='display: inline-block'>General Details</div></div><table style='width: 100%;' class='wssTable wssTextColor'><tbody>");
            if (!String.IsNullOrEmpty(buildName))
            {
                writer.WriteLine("<tr><td>Build Name</td><td>$buildName</td></tr>".Replace("$buildName", buildName));
            }

            if (!String.IsNullOrEmpty(buildNumber))
            {
                writer.WriteLine("<tr><td>Build Number</td><td>$buildNumber</td></tr>".Replace("$buildNumber", buildNumber));
            }
            writer.WriteLine("<tr><td>Report creation time</td><td>$creationTime</td></tr></tbody></table></div></div>".Replace("$creationTime", DateTime.Now.ToString()));
            writer.WriteLine(LINE_SEPARATOR);

            writer.WriteLine("<div class='wssTextAlign'><div style='padding-bottom: 1px;'><span class='wssProjectIcon'></span><div class='wssProjectHeader' style='display: inline-block'>New Projects</div><span class='wssTextColor wssProjectStats'>");
            writer.WriteLine("found $result.newProjects.size() new projects </span></div>".Replace("$result.newProjects.size()", result.NewProjects.Count.ToString()));

            WriteProjectsSummary(result.NewProjects, "new-project", writer);
            writer.WriteLine(LINE_SEPARATOR);
            writer.WriteLine("<div style='padding-bottom: 1px;'><span class='wssProjectIcon'></span><div class='wssProjectHeader' style='display: inline-block'>Existing Projects</div><span class='wssTextColor wssProjectStats'>found $result.existingProjects.size() existing projects </span></div>"
                .Replace("$result.existingProjects.size()", result.ExistingProjects.Count.ToString()));
            WriteProjectsSummary(result.ExistingProjects, "existing-project", writer);
            writer.WriteLine("</div>");

            writer.WriteLine(LINE_SEPARATOR);

            List<LicenseHistogramDataPoint> licenses = CreateLicenseHistogram(result);
            if (licenses.Count > 0)
            {
                writer.WriteLine("<div class='wssTextAlign' style='margin: 10px;'><div style='padding-bottom: 1px;'><span class='wssLicenseIcon'></span><div class='wssProjectHeader' style='display: inline-block'>License Distribution</div></div><table class='wssLicenses wssTextColor'><tr>");
                foreach (LicenseHistogramDataPoint license in licenses)
                {
                    writer.WriteLine("<td style='width: 100px; padding: 0px; vertical-align: bottom;'><table style='border-spacing: 0px 0px; width:100px; text-align:center'><tr><td style=' font-size:12px;'>$license.occurrences<td></tr><tr><td style='padding-bottom:0px; padding-right:40px; padding-left:40px'>"
                        .Replace("$license.occurrences", license.Occurrences.ToString()));
                    writer.WriteLine("<table height='$license.height' class='wssLicenseBar' title='$license.name: $license.occurrences'><tr><td></td></tr></table></td></tr></table></td>"
                        .Replace("$license.height", license.Height)
                        .Replace("$license.name", license.Name)
                        .Replace("$license.occurrences", license.Occurrences.ToString()));
                }

                writer.WriteLine("</tr></table><table style='width: 100%;'><tr>");

                foreach (LicenseHistogramDataPoint license in licenses)
                {
                    writer.WriteLine("<td><table style='width: 100px;'><tr><td class='wssTextColor' style='font-size:80%; text-align:center;' title='$license.name'>$license.shortName</td></tr></table></td>"
                        .Replace("$license.name", license.Name)
                        .Replace("$license.shortName", license.GetShortName()));
                }
                writer.WriteLine("</tr></table></div>");
            }

            writer.Write("</div></div></body></html>");
            writer.Flush();
            writer.Close();
        }

        /* --- Private methods --- */

        private void WriteProjectsSummary(Dictionary<string, PolicyCheckResourceNode> dictionary, String detailsPrefix, StreamWriter writer)
        {
            foreach (KeyValuePair<string, PolicyCheckResourceNode> pair in dictionary)
            {
                writer.WriteLine("<div class='wssProjectEntry'><div style='width: 100%;' class='wssBorder wssProjectCaptionBackground'><div style='display: inline-block; padding-left: 10px;'>");
                writer.WriteLine("<div class='wssTextColor wssProjectTitle'>$entry.key</div></div><div style='display: inline-block; padding-left: 10px'>".Replace("$entry.key", pair.Key));
                if (pair.Value.HasRejections())
                {
                    writer.WriteLine(REJECT);
                }
                else
                {
                    writer.WriteLine(APPROVE);
                }

                String detailsId = detailsPrefix + "-details-" + dictionary.Count;
                writer.WriteLine("</div><a class='wssAnchor' href='#' onclick=\"toggleDetails(this, '$detailsId')\" style='float:right; padding: 5px; line-height: 28px;'>show details</a></div>".Replace("$detailsId", detailsId));

                writer.WriteLine("<div id='$detailsId' style='width: 100%; display: none;' class='wssBorder wssDetailsBackground'>".Replace("$detailsId", detailsId));
                WriteProjectDependenciesTree(pair.Value, writer);
                writer.WriteLine("</div></div>");
            }
        }

        private void WriteProjectDependenciesTree(PolicyCheckResourceNode root, StreamWriter writer)
        {
            if (root.Children.Count == 0)
            {
                writer.WriteLine("No libraries found in project");
            }
            else
            {
                writer.WriteLine("<ul>");
                foreach (PolicyCheckResourceNode child in root.Children)
                {
                    WriteDependencyNode(child, writer);
                }
                writer.WriteLine("</ul>");
            }
        }

        private void WriteDependencyNode(PolicyCheckResourceNode node, StreamWriter writer)
        {
            writer.WriteLine("<li><div class='wssDependencyNode'><a class='wssAnchor' href='$node.resource.link' target='_blank'>".Replace("$node.resource.link", node.Resource.Link));
            writer.WriteLine("$node.resource.displayName</a><div class='wssTextColor' style='display: inline; padding-left: 20px;'>".Replace("$node.resource.displayName", node.Resource.DisplayName));
            foreach (String license in node.Resource.Licenses)
            {
                writer.WriteLine("$license &nbsp;".Replace("$license", license));
            }
            writer.WriteLine("</div>");

            RequestPolicyInfo policy = node.Policy;
            if (policy != null)
            {
                if (policy.ActionType.ToLower().Equals("reject"))
                {
                    writer.WriteLine("<div style='float: right;'><span class='wssRed'>REJECTED</span> <span class='wssAnchor' title='Rejected by policy $policyName'>info</span></div>".Replace("$policyName", policy.DisplayName));
                }
                else
                {
                    writer.WriteLine("<div style='float: right;'><span class='wssGreen'>APPROVED</span> <span class='wssAnchor' title='Approved by policy $policyName'>info</span></div>".Replace("$policyName", policy.DisplayName));
                }
            }
            writer.WriteLine("</div>");

            if (node.Children.Count > 0)
            {
                writer.WriteLine("<ul>");
                foreach (PolicyCheckResourceNode child in node.Children)
                {
                    WriteDependencyNode(child, writer);
                }
                writer.WriteLine("</ul>");
            }
            writer.WriteLine("</li>");
        }

        private List<LicenseHistogramDataPoint> CreateLicenseHistogram(CheckPoliciesResult result)
        {
            List<LicenseHistogramDataPoint> dataPoints = new List<LicenseHistogramDataPoint>();

            // create distribution histogram
            Dictionary<String, int> licenseHistogram = new Dictionary<String, int>();
            foreach (KeyValuePair<String, List<ResourceInfo>> entry in result.ProjectNewResources)
            {
                foreach (ResourceInfo resource in entry.Value)
                {
                    foreach (String license in resource.Licenses)
                    {
                        int occurrence = 0;
                        if (licenseHistogram.TryGetValue(license, out occurrence))
                        {
                            licenseHistogram.Add(license, occurrence + 1);
                        }
                        else
                        {
                            licenseHistogram.Add(license, 1);
                        }
                    }
                }
            }

            // sort by count descending
            List<KeyValuePair<String, int>> licenses = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<String, int> pair in licenseHistogram)
            {
                licenses.Add(pair);
            }
            licenses.Sort(new ValueComparator());


            // create data points
            int licenseCount = licenses.Count;
            if (licenseCount != 0)
            {
                // first licenses
                foreach (KeyValuePair<String, int> pair in licenses.GetRange(0, Math.Min(LICENSE_LIMIT, licenseCount)))
                {
                    dataPoints.Add(new LicenseHistogramDataPoint(pair.Key, pair.Value));
                }

                // aggregation of histogram tail
                int tailSize = licenseCount - LICENSE_LIMIT;
                int tailSum = 0;
                if (tailSize > 0)
                {
                    foreach (KeyValuePair<String, int> pair in licenses.GetRange(LICENSE_LIMIT, licenseCount))
                    {
                        tailSum += pair.Value;
                    }
                    dataPoints.Add(new LicenseHistogramDataPoint(OTHER_LICENSE + " (" + tailSize + ")", tailSum));
                }

                // normalize bar height
                float factor = MAX_BAR_HEIGHT / (float)Math.Max(tailSum, licenses[0].Value);
                foreach (LicenseHistogramDataPoint dataPoint in dataPoints)
                {
                    dataPoint.Height = ((int)(factor * dataPoint.Occurrences)).ToString();
                }
            }

            return dataPoints;
        }

        /* --- Nested classes --- */

        class ValueComparator : Comparer<KeyValuePair<String, int>>
        {

            public override int Compare(KeyValuePair<String, int> o1, KeyValuePair<String, int> o2)
            {
                return o2.Value.CompareTo(o1.Value);
            }
        }

    }
}
