using System;
using System.Collections.Generic;

namespace Business.Dtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string? ProjectImagePath { get; set; }

        public List<string> MemberIds { get; set; } = new();

        public string Status { get; set; } = "Ongoing";

        public IEnumerable<MemberDto> Members { get; set; } = Array.Empty<MemberDto>();
    }
}
