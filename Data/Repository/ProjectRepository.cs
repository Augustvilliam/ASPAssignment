using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Entities;

namespace Data.Repository;

public class ProjectRepository : GenericRepository<ProjectEntity>
{
    public ProjectRepository(DataContext context) : base(context) { }
}
