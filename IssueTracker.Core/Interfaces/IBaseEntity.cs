using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Core.Interfaces
{
    public interface IBaseEntity
    {
        int IsDeleted { get; set; }
        DateTime? CreatedDate { get; set; }
    }
}
