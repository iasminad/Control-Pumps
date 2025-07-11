using System;
using System.Linq;
using System.Timers;
using DataModel;
using Simulator;

namespace DataModel

{
    public class TimerManager
    {
        private readonly SistemControl _sistemControl;
        private readonly SistemControlViewModel _viewModel;
        private readonly Timer _timer;
        private int alarmaCounter = 0;
        private SistemControl sistemControl;
        //private global::Simulator.SistemControlViewModel sistemControlViewModel;

        public SistemControl SistemControl { get; }
        public global::Simulator.SistemControlViewModel SistemControlViewModel { get; }

        public TimerManager(SistemControl sistemControl, SistemControlViewModel viewModel)
        {
            _sistemControl = sistemControl;
            _viewModel = viewModel;

            _timer = new Timer
            {
                Interval = sistemControl.IntervalControlSecunde * 1000, 
                AutoReset = true,
                Enabled = false
            };

            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            float presiuneCurenta = _sistemControl.SenzorB2.PresiuneCurenta;

           
            float descarcare = (float)(_viewModel.RataDescarcareSlider / 100.0) * 0.2f;

         
            bool toateOprite = !_sistemControl.Pompe.Any(p => p.EstePornita);
            if (toateOprite)
            {
                descarcare += 0.05f;
            }

         
            float crestere = _sistemControl.Pompe.Count(p => p.EstePornita) * 0.1f;
            presiuneCurenta += crestere - descarcare;

            
            if (presiuneCurenta < 0) presiuneCurenta = 0;
            if (presiuneCurenta > 12) presiuneCurenta = 12;


            
            _sistemControl.SenzorB2.PresiuneCurenta = presiuneCurenta;
            _sistemControl.SenzorB1.PresiuneCurenta = presiuneCurenta;

            
            _sistemControl.VerificaPresiuneSiControleazaPompe();

       
            if (_sistemControl.VerificaAlarma())
            {
                alarmaCounter++;
                if (alarmaCounter <= 5)
                    Console.Beep();
            }
            else
            {
                alarmaCounter = 0;
            }

         
            _viewModel.UpdateUICuMesaj(presiuneCurenta);

      
            _viewModel.PostStatusToApi();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void SimuleazaPresiune()
        {
     
            float descarcare = 0.1f; 

         
            float crestere = _sistemControl.Pompe.Count(p => p.EstePornita) * 0.15f;

          
            float presiuneNoua = _sistemControl.SenzorB2.PresiuneCurenta + crestere - descarcare;

         
            presiuneNoua = Math.Max(0, Math.Min(10, presiuneNoua));

            _sistemControl.SenzorB2.PresiuneCurenta = presiuneNoua;
            _sistemControl.SenzorB1.PresiuneCurenta = presiuneNoua;
        }

    }
}
