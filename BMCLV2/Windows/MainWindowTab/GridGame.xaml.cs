﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BMCLV2.I18N;
using BMCLV2.Mod;

namespace BMCLV2.Windows.MainWindowTab
{
    /// <summary>
    /// GridGame.xaml 的交互逻辑
    /// </summary>
    public partial class GridGame
    {
        public GridGame()
        {
            InitializeComponent();
        }
        private readonly FrmMain _mainWindow = BmclCore.MainWindow;
        private void listVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listVer.SelectedIndex == -1)
            {
                listVer.SelectedIndex = 0;
                return;
            }
            this.listVer.ScrollIntoView(listVer.SelectedItem);
            string jsonFilePath = gameinfo.GetGameInfoJsonPath(listVer.SelectedItem.ToString());
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                MessageBox.Show(LangManager.GetLangFromResource("ErrorNoGameJson"));
                _mainWindow.SwitchStartButton(false);
                return;
            }
            _mainWindow.SwitchStartButton(true);
            BmclCore.GameInfo = gameinfo.Read(jsonFilePath);
            if (BmclCore.GameInfo == null)
            {
                MessageBox.Show(LangManager.GetLangFromResource("ErrorJsonEncoding"));
                return;
            }
            labVer.Content = BmclCore.GameInfo.id;
            labTime.Content = DateTime.Parse(BmclCore.GameInfo.time);
            labRelTime.Content = DateTime.Parse(BmclCore.GameInfo.releaseTime);
            labType.Content = BmclCore.GameInfo.type;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(LangManager.GetLangFromResource("DeleteMessageBoxInfo"), LangManager.GetLangFromResource("DeleteMessageBoxTitle"), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    if (BmclCore.GameInfo != null)
                    {
                        FileStream isused = File.OpenWrite(".minecraft\\versions\\" + listVer.SelectedItem + "\\" + BmclCore.GameInfo.id + ".jar");
                        isused.Close();
                    }
                    Directory.Delete(".minecraft\\versions\\" + listVer.SelectedItem, true);
                    if (Directory.Exists(".minecraft\\libraries\\" + listVer.SelectedItem))
                    {
                        Directory.Delete(".minecraft\\libraries\\" + listVer.SelectedItem, true);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(LangManager.GetLangFromResource("DeleteFailedMessageInfo"));
                }
                catch (IOException)
                {
                    MessageBox.Show(LangManager.GetLangFromResource("DeleteFailedMessageInfo"));
                }
                finally
                {
                    ReFlushlistver();
                }
            }
        }

        private void btnReName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rname = Microsoft.VisualBasic.Interaction.InputBox(LangManager.GetLangFromResource("RenameNewName"), LangManager.GetLangFromResource("RenameTitle"), listVer.SelectedItem.ToString());
                if (rname == "") return;
                if (rname == listVer.SelectedItem.ToString()) return;
                if (listVer.Items.IndexOf(rname) != -1) throw new Exception(LangManager.GetLangFromResource("RenameFailedExist"));
                Directory.Move(".minecraft\\versions\\" + listVer.SelectedItem, ".minecraft\\versions\\" + rname);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(LangManager.GetLangFromResource("RenameFailedError"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.ReFlushlistver();
            }
        }

        private void btnModMrg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments =
                     Path.Combine(ModHelper.SetupModPath(listVer.SelectedItem.ToString()), "mods")
            });
        }

        private void btnModdirMrg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments =
                    Path.Combine(ModHelper.SetupModPath(listVer.SelectedItem.ToString()), "moddir")
            });
        }

        private void btnModCfgMrg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments =
                    Path.Combine(ModHelper.SetupModPath(listVer.SelectedItem.ToString()), "config")
            });
        }

        private void listVer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BmclCore.MainWindow.ClickStartButton();
        }

        public void ReFlushlistver()
        {
            listVer.Items.Clear();
            BmclCore.GameManager.ReloadList();
            listVer.ItemsSource = BmclCore.GameManager.GetVersions().Keys;
        }

        public string GetSelectedVersion()
        {
            return listVer.SelectedItem as string;
        }
    }
}
