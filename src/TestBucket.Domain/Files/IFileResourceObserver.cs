using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files;

public interface IFileResourceObserver
{
    Task OnAddedAsync(FileResource file);
    Task OnDeletedAsync(FileResource file);
}
