using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DataModel
{
    public class SistemControl
    {
        public List<Pompa> Pompe { get; private set; }
        public Senzor SenzorB1 { get; set; }
        public Senzor SenzorB2 { get; set; }

        public float PresiuneSetata { get; set; }
        public float IntervalControlSecunde { get; set; } = 1;

        public SistemControl()
        {
            Pompe = new List<Pompa>
            {
                new Pompa { Id = 1 },
                new Pompa { Id = 2 },
                new Pompa { Id = 3 },
                new Pompa { Id = 4 }
            };

            SenzorB1 = new Senzor { Nume = "B1", PragActivare = 9.0f };
            SenzorB2 = new Senzor { Nume = "B2", PragActivare = 5.0f };
        }

        public void OpresteToatePompele()
        {
            foreach (var pompa in Pompe)
                pompa.Opreste();
        }

        public bool VerificaAlarma()
        {
            return SenzorB1.EsteActiv();
        }

        public Pompa DeterminaPompaPentruRotatie()
        {
            return Pompe.OrderBy(p => p.TimpFunctionare).FirstOrDefault();
        }

        public void VerificaPresiuneSiControleazaPompe()
        {
            float delta = 0.25f;

            if (SenzorB1.EsteActiv())
            {
                OpresteToatePompele();
                return;
            }

            if (SenzorB2.PresiuneCurenta < PresiuneSetata - delta)
            {
                var dePornit = Pompe
                    .Where(p => !p.EstePornita)
                    .OrderBy(p => p.NumarPorniri)
                    .ThenBy(p => p.TimpFunctionare)
                    .FirstOrDefault();

                dePornit?.Porneste();
            }
            else if (SenzorB2.PresiuneCurenta > PresiuneSetata + delta)
            {
                var deOprit = Pompe
                    .Where(p => p.EstePornita)
                    .OrderByDescending(p => p.TimpFunctionare)
                    .FirstOrDefault();

                deOprit?.Opreste();
            }
        }
        public TimeSpan CalculTimpTotal()
        {
            return Pompe.Aggregate(TimeSpan.Zero, (total, p) => total + p.TimpFunctionare);
        }

    }
}
