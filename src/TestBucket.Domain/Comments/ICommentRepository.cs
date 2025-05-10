using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Comments;
public interface ICommentRepository
{
    Task AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task UpdateAsync(Comment comment);
}
