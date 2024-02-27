﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {

    public class Office : BaseDataItem {
        public string Address { get; set; } = "";

        public virtual IEnumerable<SecurityEntry> Admins { get; set; } = default!;

        public virtual Area Area { get; set; } = default!;

        public int AreaId { get; set; }

        public string Audience { get; set; } = "";

        public string Building { get; set; } = "";

        public string BuildingCode { get; set; } = "";

        public string City { get; set; } = "";

        public string Email { get; set; } = "";

        public string ExternalUrl { get; set; } = "";
        public bool HoursIncludeHolidayMessage { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string InternalUrl { get; set; } = "";

        public bool IsInternalOnly { get; set; }
        public virtual IEnumerable<JobProfile> JobProfiles { get; set; } = default!;

        public string Notes { get; set; } = "";
        public virtual IEnumerable<OfficeHour> OfficeHours { get; set; } = default!;
        public virtual OfficeSettings OfficeSettings { get; set; } = default!;
        public OfficeTypeEnum OfficeType { get; set; }
        public string Phone { get; set; } = "";
        public string Room { get; set; } = "";
        public IEnumerable<AreaTag> Tags { get; set; } = default!;
        public string TicketUrl { get; set; } = "";
        public string Title { get; set; } = "";
        public string ZipCode { get; set; } = "";
    }
}