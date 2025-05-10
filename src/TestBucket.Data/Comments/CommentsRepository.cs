using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code;
using TestBucket.Domain.Comments;
using TestBucket.Domain.Comments.Models;

namespace TestBucket.Data.Comments;
internal class CommentsRepository : ICommentRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public CommentsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(Comment comment)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment comment)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Comments.Where(x => x.Id == comment.Id).ExecuteDeleteAsync();
    }
    public async Task UpdateAsync(Comment comment)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync();
    }
}
