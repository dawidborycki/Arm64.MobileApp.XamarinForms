using Arm64.MobileApp.XamarinForms.Helpers;
using Arm64.MobileApp.XamarinForms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace Arm64.MobileApp.XamarinForms.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string computationTime = "";

        public string ComputationTime
        {
            get => computationTime;
            set => SetProperty(ref computationTime, value);
        }

        private readonly List<DataPoint2d> computationTimeHistory = new List<DataPoint2d>();

        public ObservableCollection<DataPoint2d> DataPoints { get; set; } =
            new ObservableCollection<DataPoint2d>();

        private SimpleCommand runCalculationsCommand;        
        public SimpleCommand RunCalculationsCommand
        {
            get
            {
                if (runCalculationsCommand == null)
                {
                    runCalculationsCommand = new SimpleCommand((object parameter) =>
                    {
                        try
                        {
                            var computationTime = PerformanceHelper.MeasurePerformance(
                                MatrixHelper.SquareMatrixMultiplication, executionCount: 10);

                            ComputationTime = $"Platform: {Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")}" +
                                $"\nComputation time: {computationTime:f2} ms";

                            computationTimeHistory.Add(new DataPoint2d
                            {
                                X = computationTimeHistory.Count + 1,
                                Y = computationTime
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    });
                }

                return runCalculationsCommand;
            }
        }


        private SimpleCommand plotResultsCommand;
        public SimpleCommand PlotResultsCommand
        {
            get
            {
                if (plotResultsCommand == null)
                {
                    plotResultsCommand = new SimpleCommand((object parameter) =>
                    {
                        // Clear DataPoints and update with new data on the UI thread
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DataPoints.Clear();
                            foreach (var point in computationTimeHistory)
                            {
                                DataPoints.Add(point);
                            }
                        });
                    });
                }

                return plotResultsCommand;
            }
        }
    }
}
