using System;
using System.ComponentModel;
using DataModel;
using Communicator;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Graph.Models;

namespace Simulator
{
    public class SistemControlViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Pompa> Pompe { get; set; }
        public SistemControl SistemControl { get; set; }
        public TimerManager TimerManager { get; set; }
        public bool IsPresiuneJoasa => SistemControl.SenzorB2.PresiuneCurenta < SistemControl.PresiuneSetata;
        public float IntervalControlSecunde { get; set; } = 2; 
        private DateTime _timpUltimMesaj = DateTime.MinValue;


        public ICommand PornesteSistemCommand { get; }
        public ICommand OpresteSistemCommand { get; }
        public ICommand OpresteToatePompeleCommand { get; }

        public SistemControlViewModel()
        {
            SistemControl = new SistemControl();
            Pompe = new ObservableCollection<Pompa>(SistemControl.Pompe);
            TimerManager = new TimerManager(SistemControl, this);
            SistemControl.PresiuneSetata = 6.5f;
            PresiuneCurenta = SistemControl.SenzorB2.PresiuneCurenta;
            TimerManager.Start();


            PornesteSistemCommand = new RelayCommand(_ => StartSistem());
            OpresteSistemCommand = new RelayCommand(_ => StopSistem());
            OpresteToatePompeleCommand = new RelayCommand(_ => SistemControl.OpresteToatePompele());

            SistemControl.PresiuneSetata = 6.5f;
        }
        public TimeSpan CalculTimpTotal()
        {
            return Pompe.Aggregate(TimeSpan.Zero, (acc, p) => acc + p.TimpFunctionare);
        }

        public void PostStatusToApi()
        {
            try
            {
                var status = new ProcessStatusEvent
                {
                    Id = 1,
                    EstePornita = SistemControl.Pompe.Any(p => p.EstePornita),
                    NumarPorniri = SistemControl.Pompe.Count(p => p.EstePornita),
                    TimpFunctionare = SistemControl.CalculTimpTotal(),
                    timpPornire = SistemControl.Pompe.FirstOrDefault(p => p.EstePornita)?.timpPornire
                };

                HttpHelper.PostDataToWebAPI(status);
            }
            catch (Exception ex)
            {
                StatusMesaj += $"\n❌ Eroare la trimiterea datelor: {ex.Message}";
            }
        }

        private void StartSistem()
        {
            TimerManager.Start();
        }

        private void StopSistem()
        {
            TimerManager.Stop();
            SistemControl.OpresteToatePompele();
            UpdateUI();
        }
        private double _rataDescarcareSlider;
        public double RataDescarcareSlider
        {
            get => _rataDescarcareSlider;
            set
            {
                _rataDescarcareSlider = value;
                OnPropertyChanged(nameof(RataDescarcareSlider));
            }
        }

        private string _statusMesaj;
        public string StatusMesaj
        {
            get => _statusMesaj;
            set
            {
                _statusMesaj = value;
                OnPropertyChanged(nameof(StatusMesaj));
            }
        }
        private double _presiune;
        public double Presiune
        {
            get => _presiune;
            set
            {
                _presiune = value;
                OnPropertyChanged(nameof(Presiune));
            }
        }
        private bool _isSenzorB1Checked;
        public bool IsSenzorB1Checked
        {
            get => _isSenzorB1Checked;
            set
            {
                if (_isSenzorB1Checked != value)
                {
                    _isSenzorB1Checked = value;
                    OnPropertyChanged(nameof(IsSenzorB1Checked));
                    if (value)
                    {
                        ShowAlarmLabelForFiveSeconds();
                    }
                    else
                    {
                        IsAlarmLabelVisible = false;
                    }
                }
            }
        }

        private bool _isAlarmLabelVisible;
        public bool IsAlarmLabelVisible
        {
            get => _isAlarmLabelVisible;
            set
            {
                _isAlarmLabelVisible = value;
                OnPropertyChanged(nameof(IsAlarmLabelVisible));
            }
        }
        private async void ShowAlarmLabelForFiveSeconds()
        {
            IsAlarmLabelVisible = true;
            await System.Threading.Tasks.Task.Delay(5000);
            IsAlarmLabelVisible = false;
            OnPropertyChanged(nameof(IsAlarmLabelVisible)); 
        }

        public void UpdateUICuMesaj(float presiune)
        {
            PresiuneCurenta = presiune;
            Presiune = presiune;
            AlarmaActiva = SistemControl.SenzorB1.EsteActiv();
            OnPropertyChanged(nameof(IsPresiuneJoasa));
            OnPropertyChanged(nameof(LampaP1Color));
            OnPropertyChanged(nameof(LampaP2Color));
            OnPropertyChanged(nameof(LampaP3Color));
            OnPropertyChanged(nameof(LampaP4Color));
            OnPropertyChanged(nameof(IsSenzorB1Checked));


            var pompeActive = SistemControl.Pompe.Where(p => p.EstePornita).Select(p => p.Id);
            var nrPompe = pompeActive.Count();
            string listaPompe = string.Join(", ", pompeActive);

            string nouMesaj = "";

            if (AlarmaActiva)
            {
                nouMesaj = $"🚨 Presiune critică ({presiune:F2} bar)! Pompele oprite. Alarmă activă!";
                SistemControl.OpresteToatePompele();
            }
            else if (presiune < SistemControl.PresiuneSetata - 0.25f)
            {
                var pompe = SistemControl.Pompe.Where(p => p.EstePornita).Select(p => p.Id);
                nouMesaj = $"Presiune scăzută. Pornim pompe: {string.Join(", ", pompe)}";
            }
            else if (presiune > SistemControl.PresiuneSetata + 0.25f)
            {
                var pompe = SistemControl.Pompe.Where(p => p.EstePornita).Select(p => p.Id);
                nouMesaj = $" Presiune mare. Oprim pompe: {string.Join(", ", pompe)}";
            }
            else
            {
                var pompe = SistemControl.Pompe.Where(p => p.EstePornita).Select(p => p.Id);
                nouMesaj = $"Presiune stabilă. Menținem sistemul. Pompe active: {string.Join(", ", pompe)}";
            }

         
            if ((DateTime.Now - _timpUltimMesaj).TotalSeconds >= 3 || nouMesaj != StatusMesaj)
            {
                StatusMesaj = nouMesaj;
                _timpUltimMesaj = DateTime.Now;

                try
                {
                    string logLine = $"{DateTime.Now:HH:mm:ss} | {nouMesaj}";
                    System.IO.File.AppendAllLines("log_presiune.txt", new[] { logLine });
                }
                catch { }
            }


        }
        public void UpdateUI()
        {
            PresiuneCurenta = SistemControl.SenzorB2.PresiuneCurenta;
            AlarmaActiva = SistemControl.SenzorB1.EsteActiv();

            OnPropertyChanged(nameof(LampaP1Color));
            OnPropertyChanged(nameof(LampaP2Color));
            OnPropertyChanged(nameof(LampaP3Color));
            OnPropertyChanged(nameof(LampaP4Color));
            OnPropertyChanged(nameof(IsPresiuneJoasa));
            OnPropertyChanged(nameof(IsSenzorB1Checked));


            if (AlarmaActiva)
                StatusMesaj = " Alarmă activată (B1)!";
            else if (IsPresiuneJoasa)
                StatusMesaj = "Presiune sub prag – pornim pompe.";
            else
                StatusMesaj = " Presiune în parametri – sistem stabil.";

            StatusMesaj += "\nPompe pornite: " +
                string.Join(", ", SistemControl.Pompe.Where(p => p.EstePornita).Select(p => p.Id));

            PostStatusToApi();
        }

        private float _presiuneCurenta;
        public float PresiuneCurenta
        {
            get => _presiuneCurenta;
            set
            {
                _presiuneCurenta = value;
                OnPropertyChanged(nameof(PresiuneCurenta));
            }
        }

        private bool _alarmaActiva;
        public bool AlarmaActiva
        {
            get => _alarmaActiva;
            set
            {
                _alarmaActiva = value;
                OnPropertyChanged(nameof(AlarmaActiva));
            }
        }
        public Brush LampaP1Color => SistemControl.Pompe[0].EstePornita ? Brushes.Green : Brushes.Gray;
        public Brush LampaP2Color => SistemControl.Pompe[1].EstePornita ? Brushes.Green : Brushes.Gray;
        public Brush LampaP3Color => SistemControl.Pompe[2].EstePornita ? Brushes.Green : Brushes.Gray;
        public Brush LampaP4Color => SistemControl.Pompe[3].EstePornita ? Brushes.Green : Brushes.Gray;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nume)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nume));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}