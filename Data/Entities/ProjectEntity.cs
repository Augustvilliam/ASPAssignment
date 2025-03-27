using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ProjectEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Column(TypeName = "money")]
        public decimal Budget { get; set; }
        public string? ProjectImagePath { get; set; }

        public ICollection<MemberEntity> Members { get; set; } = new List<MemberEntity>();
    }
}
