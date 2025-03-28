using Data.Contexts;
using Data.Entities;

namespace Data.Repository
{
    public class MemberRepository : GenericRepository<MemberEntity>
    {
        public MemberRepository(DataContext context) : base(context) { }
    }
}
