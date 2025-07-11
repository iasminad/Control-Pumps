using DataModel;
using System.Collections.Generic;

public static class InMemoryStore
{
    public static List<ProcessStatusEvent> ProcessEvents { get; set; } = new List<ProcessStatusEvent>();
}
