﻿using Newtonsoft.Json;

namespace uofi_itp_directory_search.ViewModel {

    public class Employee : EmployeeCompact {

        [JsonProperty("addressline1")]
        public string AddressLine1 { get; set; } = "";

        [JsonProperty("addressline2")]
        public string AddressLine2 { get; set; } = "";

        [JsonProperty("awards")]
        public List<DatedItem> Awards { get; set; } = default!;

        [JsonProperty("biography")]
        public string Biography { get; set; } = "";

        [JsonProperty("courses")]
        public List<Course> Courses { get; set; } = default!;

        [JsonProperty("educationhistory")]
        public List<InstitutionalRangedItem> EducationHistory { get; set; } = default!;

        [JsonProperty("expertsurl")]
        public string ExpertsUrl { get; set; } = "";

        [JsonProperty("grants")]
        public List<BaseItem> Grants { get; set; } = default!;

        [JsonProperty("id")]
        public string Id => GenerateId(Source, NetId);

        [JsonProperty("jobtypelist")]
        public List<string> JobTypeList => JobProfiles?.Select(a => a.JobType.ToLowerInvariant()).Distinct().ToList() ?? [];

        [JsonProperty("lastupdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("linkedinurl")]
        public string LinkedInUrl { get; set; } = "";

        [JsonProperty("links")]
        public List<BaseItem> Links { get; set; } = default!;

        [JsonProperty("officelist")]
        public List<string> OfficeList => JobProfiles?.Select(a => a.Office.ToLowerInvariant()).Distinct().ToList() ?? [];

        [JsonProperty("officejobtypelist")]
        public List<string> OfficeTitleList => JobProfiles?.Select(a => $"{a.Office.ToLowerInvariant()} {a.JobType.ToLowerInvariant()}").Distinct().ToList() ?? [];

        [JsonProperty("organizations")]
        public List<InstitutionalRangedItem> Organizations { get; set; } = default!;

        [JsonProperty("presentations")]
        public List<DatedItem> Presentations { get; set; } = default!;

        [JsonProperty("publications")]
        public List<DatedItem> Publications { get; set; } = default!;

        [JsonProperty("quote")]
        public string Quote { get; set; } = "";

        [JsonProperty("researchstatement")]
        public string ResearchStatement { get; set; } = "";

        [JsonProperty("services")]
        public List<BaseItem> Services { get; set; } = default!;

        [JsonProperty("suggest")]
        public dynamic? Suggest => string.IsNullOrWhiteSpace(NetId) ? null : new { input = new[] { LastName, FirstName }, contexts = new { source = Source } };

        [JsonProperty("teachingstatement")]
        public string TeachingStatement { get; set; } = "";

        [JsonProperty("twittername")]
        public string TwitterName { get; set; } = "";

        public static string GenerateId(string source, string netId) => $"{source}-{netId}";
    }
}