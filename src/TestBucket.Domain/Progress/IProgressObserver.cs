using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Progress;
public interface IProgressObserver
{
    Task NotifyAsync(ProgressTask progressTask);
}
