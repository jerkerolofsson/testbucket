using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Models
{
    public record class ProfileImage(string MediaType, byte[] Bytes);
}
