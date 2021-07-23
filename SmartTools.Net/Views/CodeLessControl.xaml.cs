﻿using AduSkin.Controls.Metro;
using HandyControl.Controls;
using SmartTools.Net.CustomControls;
using SmartTools.Net.Services;
using SmartTools.Net.Utils;
using SmartTools.Net.Db;
using SmartTools.Net.Models;
using SmartTools.Net.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.Versioning;
using SmartSoft.common.Utils.solution;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SmartTools.Net.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [SupportedOSPlatform("windows10.0")]
    public partial class CodeLessControl : UserControl
    {
        #region initialize
        CodeLessVM codeLessVM = new CodeLessVM();
        private string _currentLan = string.Empty;
        public CodeLessControl()
        {
            InitializeComponent();
            _currentLan = "zh-cn";
            //this.Background = new ImageBrush() { ImageSource = FileUtil.GetImage("imgs.bg.png"), Stretch = Stretch.Fill, Opacity = 0.5 };
        }
        #endregion

        #region UserControl_Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = codeLessVM;
            new ControlUtil().HideBoundingBox(this);
        }
        #endregion

        #region connect database
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            var logading = Dialog.Show(new Loading());
            if (string.IsNullOrWhiteSpace(codeLessVM.connectString))
            {
                HandyControl.Controls.MessageBox.Error("连接字符串不能为空!");
                return;
            }

            try
            {
                codeLessVM.databaselist= new CodeLessService(codeLessVM.connectString).GetDatabase();

                databases.ItemsSource = codeLessVM.databaselist;
                databases.SelectedValuePath = "name";
                databases.DisplayMemberPath = "name";
                //HandyControl.Controls.MessageBox.Success("数据库连接成功");

            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Error(ex.Message);
            }
            logading.Close();
        }
        #endregion

        #region databases_Selected
        private void databases_Selected(object sender, RoutedEventArgs e)
        {
            var logading = Dialog.Show(new Loading());
            if (string.IsNullOrWhiteSpace(codeLessVM.connectString))
            {
                HandyControl.Controls.MessageBox.Error("连接字符串不能为空!");
                return;
            }

            dbtables.ItemsSource = new List<DbTable>();

            try
            {
                if (!string.IsNullOrWhiteSpace(codeLessVM.database))
                {
                    dbtables.ItemsSource = new CodeLessService(codeLessVM.connectString).GetDbTable(codeLessVM.database);
                    dbtables.SelectedValuePath = "name";
                    dbtables.DisplayMemberPath = "name";
                }
                else
                {
                    dbtables.SelectedIndex = -1;
                    dbtables.ItemsSource = null;
                    primarykey.SelectedIndex = -1;
                    primarykey.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Error(ex.Message);
            }
            logading.Close();
        }
        #endregion

        #region GenCode_Click
        private void GenCode_Click(object sender, RoutedEventArgs e)
        {
            var logading = Dialog.Show(new Loading());

            if (string.IsNullOrWhiteSpace(codeLessVM.connectString))
            {
                HandyControl.Controls.MessageBox.Error("连接字符串不能为空!");
                logading.Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(codeLessVM.database))
            {
                HandyControl.Controls.MessageBox.Error("请选择数据库!");
                logading.Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(codeLessVM.dbTable))
            {
                HandyControl.Controls.MessageBox.Error("请选择数据库表!");
                logading.Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(codeLessVM.primarykey))
            {
                HandyControl.Controls.MessageBox.Error("请选择业务主键!");
                logading.Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(codeLessVM._rootnamespace))
            {
                HandyControl.Controls.MessageBox.Error("请填写命名空间!");
                logading.Close();
                return;
            }


            try
            {
                //var tbinfo = new CodeLessService(codeLessVM.connectString).GetDbTableInfo(codeLessVM.database,codeLessVM.dbTable);
                new CodeBuilder(codeLessVM.DbTableInfos,
                    codeLessVM._rootnamespace,
                    $"Wechat{FiledUtil.GetModelName(codeLessVM.dbTable)}",
                    codeLessVM.database,
                    codeLessVM.dbTable,
                    codeLessVM.buildpath,
                    "wechat",
                    "项目",
                    codeLessVM.primarykey,
                    codeLessVM.searchParams)
                .BuildModel()
                .BuildSearchModel()
                .BuildService()
                .BuildController()
                .BuildDbinfo();
                HandyControl.Controls.MessageBox.Success("生成成功");
                //var result = HandyControl.Controls.MessageBox.Show("生成成功,是否打开输出文件夹？", "温馨提示", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK);
                //if (result == MessageBoxResult.OK)
                //{
                //    Process.Start("explorer.exe", $@"{AppDomain.CurrentDomain.BaseDirectory}Oupput\");
                //}
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Error(ex.Message);
            }
            finally
            {
                logading.Close();
            }
        }
        #endregion

        #region seelect sln file
        /// <summary>
        /// selectfile_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectfile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Title = "选择解决方案"

            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                codeLessVM.Slnfileaddr = openFileDialog.FileName;
                codeLessVM.Projectlist = new SolutionUtil(codeLessVM.slnfileaddr).SlnParse();
                codeLessVM.Arealist = new List<string>();
            }
            else
            {
                HandyControl.Controls.MessageBox.Error("未选择任何解决方案!");
            }
        }
        #endregion

        #region project_SelectionChanged
        /// <summary>
        /// project_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void project_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (codeLessVM.project != null)
            {
                var path = codeLessVM.projectlist.Where(x => x.projName == codeLessVM.project).FirstOrDefault().projFullName;
                codeLessVM.projectPath = $"{path.Substring(0, path.LastIndexOf("/"))}/Areas";
                codeLessVM.Arealist = new List<string>();
                if (Directory.Exists(@$"{codeLessVM.slnfileaddr.Substring(0, codeLessVM.slnfileaddr.LastIndexOf("\\"))}\{codeLessVM.projectPath}"))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(@$"{codeLessVM.slnfileaddr.Substring(0, codeLessVM.slnfileaddr.LastIndexOf("\\"))}\{codeLessVM.projectPath}");
                    var files = directoryInfo.GetDirectories();
                    List<string> areas = new List<string>();
                    foreach (var file in files)
                    {
                        areas.Add(file.Name);
                    }
                    codeLessVM.Arealist = areas;
                }
            }
        }
        #endregion

        #region projectArea_SelectionChanged
        /// <summary>
        /// projectArea_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Directory.Exists(@$"{codeLessVM.slnfileaddr.Substring(0, codeLessVM.slnfileaddr.LastIndexOf("\\"))}\{codeLessVM.projectPath}"))
            {
                codeLessVM.buildpath = @$"{codeLessVM.slnfileaddr.Substring(0, codeLessVM.slnfileaddr.LastIndexOf("\\"))}\{codeLessVM.projectPath}\{codeLessVM.projectArea}".Replace("\\","/");
                var path = codeLessVM.projectlist.Where(x => x.projName == codeLessVM.project).FirstOrDefault().projName;
                codeLessVM.Rootnamespace = $"{path.Substring(0, path.LastIndexOf("."))}.Areas.{codeLessVM.projectArea}";
            }
        }
        #endregion

        #region dbtables_SelectionChanged
        private void dbtables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(codeLessVM.dbTable))
                codeLessVM.DbTableInfos = new CodeLessService(codeLessVM.connectString).GetDbTableInfo(codeLessVM.database, codeLessVM.dbTable);
            else
                codeLessVM.DbTableInfos = new List<DbTableInfo>();
        }
        #endregion

        #region CheckComboBox_SelectionChanged
        /// <summary>
        /// CheckComboBox_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            codeLessVM.searchParams = searchparam.SelectedItems.Cast<DbTableInfo>().ToList();
        }
        #endregion
    }
}
