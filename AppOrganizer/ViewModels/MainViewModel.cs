using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using AppOrganizer.Models;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Linq;
using System;

namespace AppOrganizer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ProfileModel? selectedProfile = null;

        public ProfileModel? SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                selectedProfile = value;
                OnPropertyChanged();
            }
        }

        private Program? selectedProgram = null;

        public Program? SelectedProgram
        {
            get { return selectedProgram; }
            set
            {
                selectedProgram = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProfileModel> Model { get; set; } = new ObservableCollection<ProfileModel>(ModelHelper.LoadModel());

        public ICommand SelectionChangedProfileList { get; set; }
        public ICommand SelectionChangedProgramList { get; set; }
        public ICommand RunPrograms { get; set; }
        public ICommand AddProgram { get; set; }
        public ICommand DeleteProgram { get; set; }
        public ICommand ModProgram { get; set; }
        public ICommand AddProfile { get; set; }
        public ICommand DeleteProfile { get; set; }
        public ICommand Closing { get; set; }

        void runPrograms()
        {
            if (SelectedProfile is null)
                return;

            foreach (var program in SelectedProfile.Programs)
            {
                Process process = new();
                var processInfo = new ProcessStartInfo();
                processInfo.FileName = program.Executable;
                processInfo.Arguments = program.Arguments;
                var workingDirectory = program.Executable.Split('\\').ToList();
                workingDirectory.RemoveAt(workingDirectory.Count - 1);
                processInfo.WorkingDirectory = string.Join('\\', workingDirectory);

                process.StartInfo = processInfo;

                try
                {
                    process.Start();
                }
                catch (System.Exception e) when (e.GetType() == typeof(System.ComponentModel.Win32Exception))
                {
                    processInfo.Verb = "runas";
                    processInfo.UseShellExecute = true;
                    process.Start();
                }
            }
        }

        void addProgram()
        {
            (string path, string args) = GetFilenameAndArgs();
            if (path == string.Empty)
                return;

            SelectedProfile?.Programs.Add(new Program { Executable = path, Arguments = args });
        }

        void deleteProgram()
        {
            if (SelectedProgram is null)
                return;
            SelectedProfile?.Programs.Remove(SelectedProgram);
        }

        void modProgram()
        {
            if (SelectedProfile is null || SelectedProgram is null)
                return;

            (string path, string args) = GetFilenameAndArgs();
            if (path == string.Empty)
                return;

            SelectedProfile.Programs[SelectedProfile.Programs.IndexOf(SelectedProgram)] = new Program
            { Executable = path, Arguments = args };
        }

        void closing()
        {
            Model.SaveModel();
        }

        void addProfile()
        {
            NewProfileWindow window = new();
            if (window.ShowDialog() == true)
                Model.Add(new ProfileModel { Name = window.ProfileName, Programs = new ObservableCollection<Program>() });
        }

        void deleteProfile()
        {
            if (SelectedProfile is null)
                return;

            Model.Remove(SelectedProfile);
            SelectedProfile = null;
        }

        public MainViewModel()
        {
            SelectionChangedProfileList = new RelayCommand(o => SelectedProfile = (ProfileModel)o);
            SelectionChangedProgramList = new RelayCommand(o => SelectedProgram = o == null ? null : (Program)o);
            RunPrograms = new RelayCommand(o => runPrograms(), o => CanRunPrograms());
            AddProgram = new RelayCommand(o => addProgram(), o => IsProfileNull());
            DeleteProgram = new RelayCommand(o => deleteProgram(), o => IsProgramNull());
            ModProgram = new RelayCommand(o => modProgram(), o => IsProgramNull());
            AddProfile = new RelayCommand(o => addProfile());
            DeleteProfile = new RelayCommand(o => deleteProfile(), o => IsProfileNull());
            Closing = new RelayCommand(o => closing());
        }

        void genDummyData()
        {
            Model.Add(new ProfileModel
            {
                Name = "a",
                Programs = new ObservableCollection<Program> {
                new Program { Executable = "a", Arguments = "b" },
                new Program { Executable = "b", Arguments = "b" },
                new Program { Executable = "c", Arguments = "b" }
            }
            });
            Model.Add(new ProfileModel
            {
                Name = "b",
                Programs = new ObservableCollection<Program> {
                new Program { Executable = "d", Arguments = "b" },
                new Program { Executable = "e", Arguments = "b" },
                new Program { Executable = "f", Arguments = "b" }
            }
            });
            Model.Add(new ProfileModel { Name = "c" });
            Model.Add(new ProfileModel { Name = "d" });
        }

        bool IsProfileNull() => SelectedProfile == null ? false : true;
        bool IsProgramNull() => SelectedProgram == null ? false : true;
        bool CanRunPrograms() => SelectedProfile != null ? SelectedProfile.Programs.Any() : false;
        (string, string) GetFilenameAndArgs()
        {
            OpenFileDialog dialog = new()
            {
                DereferenceLinks = false,
                Filter = "Executable Files (*.exe)|*.exe|Shortcuts (*.lnk)|*.lnk|Batch Files (*.bat)|*.bat"
            };

            if (dialog.ShowDialog() == true)
            {
                var executablePath = dialog.FileName;

                if (new[] { "exe", "bat" }.Contains(executablePath.Split('.').Last()))
                    return (dialog.FileName, "");

                if (executablePath.Split('.').Last() == "url")
                    return ("", "");

                if (executablePath.Split('.').Last() == "lnk")
                {
                    WshShell shell = new();
                    IWshShortcut shortcut = shell.CreateShortcut(dialog.FileName);
                    return (shortcut.TargetPath, shortcut.Arguments);
                }
            }

            return ("", "");
        }

    }
}
