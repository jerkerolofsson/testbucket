using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestAccounts.Models;
public class EmailAccountTypes
{
    public const string Pop3 = "pop3";
    public const string Imap = "imap";
    public const string MicrosoftExchange = "exchange";

    public static readonly string[] All = [Pop3, Imap, MicrosoftExchange];
}
