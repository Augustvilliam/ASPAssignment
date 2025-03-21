

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class MemberAdressEntity
{
    [Key, ForeignKey("Member")]
    public string MemberId { get; set; } = null!;
    public string Adress { get; set; } = null!;

    public virtual MemberEntity Member { get; set; } = null!;
}
