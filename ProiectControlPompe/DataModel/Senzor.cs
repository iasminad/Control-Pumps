namespace DataModel
{
    public class Senzor
    {
        public string Nume { get; set; } 
        public float PragActivare { get; set; }
        public float PresiuneCurenta { get; set; }

        public bool EsteActiv()
        {
            return PresiuneCurenta >= PragActivare;
        }
    }
}