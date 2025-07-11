using System;

namespace DataModel
{
    public class ProcessStatusEvent
    {
        public int Id { get; set; }
        public bool EstePornita { get; set; }
        public int NumarPorniri { get; set; }
        public TimeSpan TimpFunctionare { get; set; }
        public DateTime? timpPornire { get; set; }
    }
}