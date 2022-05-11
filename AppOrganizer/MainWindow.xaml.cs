using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using IWshRuntimeLibrary;
using System.Collections.ObjectModel;
using AppOrganizer.ViewModels;

namespace AppOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Closing += MainViewModel.Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
