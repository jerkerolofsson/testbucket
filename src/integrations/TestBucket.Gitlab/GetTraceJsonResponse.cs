using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Gitlab;
public class GetTraceJsonResponse
{
    public int id { get; set; }
    public string? status { get; set; }
    public bool complete { get; set; }
    public string? state { get; set; }
    public bool append { get; set; }
    public bool truncated { get; set; }
    public int offset { get; set; }
    public int size { get; set; }
    public int total { get; set; }
    public Line[]? lines { get; set; }
}

public class Line
{
    public int offset { get; set; }
    public Content[]? content { get; set; }
    public string? section { get; set; }
    public bool section_header { get; set; }
    public string? section_duration { get; set; }
}

public class Content
{
    public string? text { get; set; }

    /// <summary>
    /// term-fg-l-cyan term-bold
    /// term-fg-l-green term-bold
    /// </summary>
    public string? style { get; set; }
}
