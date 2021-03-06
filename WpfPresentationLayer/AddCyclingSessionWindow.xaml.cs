﻿using DataLayer;
using DomainLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfPresentationLayer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddCyclingSessionWindow : Window
    {
        string _databaseString;
        public AddCyclingSessionWindow(string databaseString)
        {
            InitializeComponent();
            _databaseString = databaseString;
            var trainingTypeValues = Enum.GetValues(typeof(TrainingType));
            var bikeTypeValues = Enum.GetValues(typeof(BikeType));
            trainingsTypeList.ItemsSource = trainingTypeValues;
            bikeTypeList.ItemsSource = bikeTypeValues;


            int[] hourNumbers = new int[24];
            for (int i = 0; i < 24; i++)
                hourNumbers[i] = i;
            startHoursSelection.ItemsSource = hourNumbers;
            durationHoursTextBox.ItemsSource = hourNumbers;

            int[] minuteNumbers = new int[60];
            for (int i = 0; i < 60; i++)
                minuteNumbers[i] = i;
            startMinutesSelection.ItemsSource = minuteNumbers;
            durationMinutesTextBox.ItemsSource = minuteNumbers;

            setInitialValues();
        }
        private void setInitialValues()
        {
            trainingsTypeList.SelectedIndex = 0;
            bikeTypeList.SelectedIndex = 0;
            startHoursSelection.SelectedIndex = 0;
            durationHoursTextBox.SelectedIndex = 0;
            startMinutesSelection.SelectedIndex = 0;
            durationMinutesTextBox.SelectedIndex = 0;
            trainingDatePicker.SelectedDate = DateTime.Today;
            kmPerHoursTextBox.Text = "";
            distanceTextBox.Text = "";
            wattTextBox.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime startTime = (DateTime)trainingDatePicker.SelectedDate;

                //startTime
                int startHours = (int)startHoursSelection.SelectedItem;
                int startMinutes = (int)startMinutesSelection.SelectedItem;
                startTime = startTime.AddHours(startHours);
                startTime = startTime.AddMinutes(startMinutes);

                //duration

                int durationHours = int.Parse(durationHoursTextBox.Text);
                int durationMinutes = int.Parse(durationMinutesTextBox.Text);


                TimeSpan duration = new TimeSpan(durationHours, durationMinutes, 0);

                //trainingType
                TrainingType trainingType = (TrainingType)trainingsTypeList.SelectedItem;

                string comment = null;
                if (commentTextBox.Text != "")
                    comment = commentTextBox.Text;

                int distance = int.Parse(distanceTextBox.Text);

                float? averageSpeed = null;
                if (kmPerHoursTextBox.Text != "")
                    averageSpeed = float.Parse(kmPerHoursTextBox.Text);

                int? watt = null;
                if (wattTextBox.Text != "")
                    watt = int.Parse(wattTextBox.Text);

                BikeType bikeType = (BikeType)bikeTypeList.SelectedItem;

                TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContext(_databaseString)));
                m.AddCyclingTraining(startTime, distance, duration, averageSpeed,watt, trainingType, comment,bikeType);
                setInitialValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
