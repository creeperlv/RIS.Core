using Avalonia.Controls;
using Avalonia.Platform;
using Newtonsoft.Json;
using RIS.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RIS.Core
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
            this.ExtendClientAreaToDecorationsHint = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Major >= 10)
                {
                    if (Environment.OSVersion.Version.Build >= 22000)
                    {
                        TransparencyLevelHint = WindowTransparencyLevel.Mica;
                    }
                    else
                    {
                        TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
                    }
                }
            }
            Hint0.IsVisible = true;
            Hint1.IsVisible = false;
            HintGrid.IsVisible = true;
            SheetGrid.IsVisible = false;
            ApplyEvents();
        }
        void ApplyEvents()
        {
            Button_OpenProj.Click += async (_, _) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AllowMultiple = false;
                var files = await openFileDialog.ShowAsync(this);
                if (files is not null)
                {
                    LoadProject(files[0]);
                }
            };
            Button_NewProj.Click += async (_, _) =>
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.Filters.Add(new FileDialogFilter { Extensions = new List<string> { ".proj" }, Name = "Project File" });
                var file = await SFD.ShowAsync(this);

                if (file is not null)
                {
                    var File = new FileInfo(file);
                    if (!File.Exists)
                    {
                        File.Create().Close();
                    }
                    LoadProject(File.Directory!);
                }

            };
            Button_AddSheet.Click += (_, _) =>
            {
                var _id = Guid.NewGuid().ToString();
                if (CurrentDirectory is not null)
                {
                    File.Create(Path.Combine(CurrentDirectory.FullName, _id)).Close();
                    LoadProject(CurrentDirectory);
                }
            };
            SheetItemPresenter.SelectionChanged += (sender, e) =>
            {
                try
                {
                    if (SheetItemPresenter.SelectedIndex >= 0)
                    {
                        HintGrid.IsVisible = false;
                        SheetGrid.IsVisible = true;
                        ApplyInfomation(RISMap[ListFileMap[SheetItemPresenter.SelectedIndex]]);
                    }
                }
                catch (Exception)
                {
                }
            };
            Button_Save.Click += (_, _) =>
            {
                try
                {
                    if (CurrentDirectory is not null)
                    {
                        RISItemData data = new RISItemData();
                        data.ID = RISID.Text;
                        data.Assign = RISAssign.Text;
                        data.Context = RISContext.Text;
                        data.RISMiti = RISMiti.Text;
                        data.Prob = RISProb.SelectedIndex;
                        data.Impact = RISImpact.SelectedIndex;
                        data.RISStatus = RISStatus.Text;
                        data.Description = RISDescription.Text;
                        data.RISTrigger = RISTrigger.Text;
                        if (RISDate.SelectedDate.HasValue)
                        {
                            data.Date = RISDate.SelectedDate.Value.UtcDateTime;
                        }
                        var _path = Path.Combine(CurrentDirectory.FullName, ListFileMap[SheetItemPresenter.SelectedIndex]);
                        if (File.Exists(_path)) File.Delete(_path);
                        File.WriteAllText(_path, JsonConvert.SerializeObject(data, JsonSerializerSettings));
                        LoadProject(CurrentDirectory);
                    }
                }
                catch (Exception)
                {
                }
            };
        }
        DirectoryInfo? CurrentDirectory;
        void ApplyInfomation(RISItemData? data)
        {
            if (data is null)
            {
                RISID.Text = "";
                RISAssign.Text = "";
                RISDate.SelectedDate = DateTime.Now;
                RISProb.SelectedIndex = 0;
                RISImpact.SelectedIndex = 0;
                RISContext.Text = "";
                RISStatus.Text = "";
                RISTrigger.Text = "";
                RISMiti.Text = "";
                RISDescription.Text = "";
            }
            else
            {
                RISID.Text = data.ID;
                RISAssign.Text = data.Assign;
                RISDescription.Text = data.Description;
                RISDate.SelectedDate = data.Date;
                RISProb.SelectedIndex = data.Prob;
                RISImpact.SelectedIndex = data.Impact;
                RISContext.Text = data.Context;
                RISStatus.Text = data.RISStatus;
                RISTrigger.Text = data.RISTrigger;
                RISMiti.Text = data.RISMiti;
            }
        }
        void CreateProject(string File)
        {

        }
        void LoadProject(string Location)
        {
            if (Location.ToUpper().EndsWith(".PROJ"))
            {
                FileInfo _fi = new(Location);
                if (_fi.Directory is not null)
                    LoadProject(_fi.Directory);
            }
            else LoadProject(new DirectoryInfo(Location));
        }
        Dictionary<int, string> ListFileMap = new Dictionary<int, string>();
        Dictionary<string, RISItemData?> RISMap = new();
        List<ListBoxItem> items = new List<ListBoxItem>();
        void LoadProject(DirectoryInfo Directory)
        {
            Trace.WriteLine("Opening Directory...");
            CurrentDirectory = Directory;
            Hint0.IsVisible = false;
            Hint1.IsVisible = true;
            var files = Directory.EnumerateFiles().OrderBy(p => p.CreationTime);
            int i = 0;
            ListFileMap.Clear();
            RISMap.Clear();
            items.Clear();
            foreach (var item in files)
            {
                if (item.Name.EndsWith(".proj"))
                {

                }
                else
                {
                    var _i = LoadSingleFile(item);
                    ListFileMap.Add(i, item.Name);
                    RISMap.Add(item.Name, _i);
                    if (_i is null)
                    {
                        items.Add(new ListBoxItem { Content = item.Name });
                    }
                    else
                    {
                        items.Add(new ListBoxItem { Content = _i.ID });
                    }
                    i++;
                }
            }
            SheetItemPresenter.Items = null;
            SheetItemPresenter.Items = items;

        }
        JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
        RISItemData? LoadSingleFile(FileInfo file)
        {
            return JsonConvert.DeserializeObject<RISItemData>(File.ReadAllText(file.FullName!)!, JsonSerializerSettings);
        }
    }
}
