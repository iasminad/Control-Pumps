using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{ 
        public class Pompa: ProcessStatusEvent
        {
            public void Porneste()
            {
                if (!EstePornita)
                {
                    EstePornita = true;
                    NumarPorniri++;
                    timpPornire = DateTime.Now;
                }
            }

            public void Opreste()
            {
                if (EstePornita)
                {
                    EstePornita = false;
                    if (timpPornire.HasValue)
                    {
                        TimpFunctionare += DateTime.Now - timpPornire.Value;
                        timpPornire = null;
                    }
                }
            }

            public void ResetFunctionare()
            {
                TimpFunctionare = TimeSpan.Zero;
                NumarPorniri = 0;
                timpPornire = null;
                EstePornita = false;
            }
        }
}