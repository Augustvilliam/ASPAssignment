using Data.Contexts;

namespace Data.Repository
{
    public class MemberRepository : GenericRepository<MemberEntity>
    {
        public MemberRepository(DataContext context) : base(context) { }
    }
}
