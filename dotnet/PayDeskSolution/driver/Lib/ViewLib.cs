using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using driver.Config;

namespace driver.Lib
{
    public class ViewLib
    {        //DataGridView Style
        public static void LoadGridsView(ref DataGridView[] grids, Orientation orient)
        {
            short i = 0;
            short j = 0;
            byte g = 0;

            bool artLoadHasError = false;
            bool chqLoadHasError = false;

            object[] chqStyles = null;
            object[] artStyles = null;

            string[] chqColName = null;
            bool[] chqColVisible = null;
            bool[] chqColAutoSize = null;
            int[] chqColWidth = null;
            int[] chqColDsplIdx = null;

            string[] artColName = null;
            bool[] artColVisible = null;
            bool[] artColAutoSize = null;
            int[] artColWidth = null;
            int[] artColDsplIdx = null;

            try
            {
                if (orient == Orientation.Horizontal)
                {
                    chqStyles = (object[])((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[0])[0];
                    artStyles = (object[])((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[1])[0];
                }
                else
                {
                    chqStyles = (object[])((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[0])[1];
                    artStyles = (object[])((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[1])[1];
                }

                chqColName = (string[])chqStyles[0];
                chqColVisible = (bool[])chqStyles[1];
                chqColAutoSize = (bool[])chqStyles[2];
                chqColWidth = (int[])chqStyles[3];
                chqColDsplIdx = (int[])chqStyles[4];

                artColName = (string[])artStyles[0];
                artColVisible = (bool[])artStyles[1];
                artColAutoSize = (bool[])artStyles[2];
                artColWidth = (int[])artStyles[3];
                artColDsplIdx = (int[])artStyles[4];
            }
            catch
            {
                chqLoadHasError = true;
                artLoadHasError = true;
            }

            for (g = 0; g < grids.Length; g++)
                switch (grids[g].Name)
                {
                    case "grid_Order":
                        {
                            grids[g].AllowUserToOrderColumns = !driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ChqColumnLock;
                            grids[g].AllowUserToResizeColumns = !driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ChqColumnLock;
                            #region chq
                            if (!chqLoadHasError)
                                try
                                {
                                    for (i = 0; i < grids[g].ColumnCount; i++)
                                    {
                                        grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                        for (j = 0; j < chqColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == chqColName[j])
                                            {
                                                grids[g].Columns[i].Visible = chqColVisible[j];

                                                if (chqColAutoSize[j])
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                                else
                                                {
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                                                    grids[g].Columns[i].Width = chqColWidth[j];
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    int c = 0;
                                    for (i = 0; i < grids[g].ColumnCount && c < 10; i++)
                                        for (j = 0; j < chqColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == chqColName[j])
                                            {
                                                if (grids[g].Columns[i].DisplayIndex != chqColDsplIdx[j])
                                                {
                                                    grids[g].Columns[i].DisplayIndex = chqColDsplIdx[j];
                                                    i = -1;
                                                    c++;
                                                    break;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                }
                                catch { chqLoadHasError = true; }
                            #endregion
                            if (!chqLoadHasError)
                                break;
                            #region OnError
                            for (j = 0; j < 1; j++)
                            {
                                for (i = 0; i < grids[g].ColumnCount; i++)
                                {
                                    grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                    switch (grids[g].Columns[i].Name.ToString())
                                    {
                                        case "BC":
                                            {
                                                grids[g].Columns[i].Width = 140;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 0;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "NAME":
                                            {
                                                grids[g].Columns[i].Width = 225;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 1;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "DESC":
                                            {
                                                grids[g].Columns[i].Width = 365;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 2;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "UNIT":
                                            {
                                                grids[g].Columns[i].Width = 55;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 3;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "TOT":
                                            {
                                                grids[g].Columns[i].Width = 65;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 4;
                                                grids[g].Columns[i].ReadOnly = false;
                                                break;
                                            }
                                        case "PRICE":
                                            {
                                                grids[g].Columns[i].Width = 78;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 5;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        case "ASUM":
                                            {
                                                grids[g].Columns[i].Width = 90;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 9;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                        default:
                                            {
                                                grids[g].Columns[i].Visible = false;
                                                grids[g].Columns[i].ReadOnly = true;
                                                break;
                                            }
                                    }
                                }

                                if (grids[g].Columns["BC"].DisplayIndex != 0 ||
                                    grids[g].Columns["NAME"].DisplayIndex != 1 ||
                                    grids[g].Columns["DESC"].DisplayIndex != 2 ||
                                    grids[g].Columns["UNIT"].DisplayIndex != 3 ||
                                    grids[g].Columns["TOT"].DisplayIndex != 4 ||
                                    grids[g].Columns["PRICE"].DisplayIndex != 5 ||
                                    grids[g].Columns["ASUM"].DisplayIndex != 9)
                                    j = -1;
                            }
                            #endregion
                            break;
                        }
                    case "grid_Products":
                        {
                            grids[g].AllowUserToOrderColumns = !driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ArtColumnLock;
                            grids[g].AllowUserToResizeColumns = !driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ArtColumnLock;
                            #region art
                            if (!artLoadHasError)
                                try
                                {
                                    for (i = 0; i < grids[g].ColumnCount; i++)
                                    {
                                        grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                        for (j = 0; j < artColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == artColName[j])
                                            {
                                                grids[g].Columns[i].Visible = artColVisible[j];

                                                if (artColAutoSize[j])
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                                else
                                                {
                                                    grids[g].Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                                                    grids[g].Columns[i].Width = artColWidth[j];
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    for (i = 0; i < grids[g].ColumnCount; i++)
                                        for (j = 0; j < artColName.Length; j++)
                                        {
                                            if (grids[g].Columns[i].Name == artColName[j])
                                            {
                                                if (grids[g].Columns[i].DisplayIndex != artColDsplIdx[j])
                                                {
                                                    grids[g].Columns[i].DisplayIndex = artColDsplIdx[j];
                                                    j = 0;
                                                    break;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                }
                                catch { artLoadHasError = true; }
                            #endregion
                            if (!artLoadHasError)
                                break;
                            #region OnError
                            for (j = 0; j < 1; j++)
                            {
                                for (i = 0; i < grids[g].ColumnCount; i++)
                                {
                                    grids[g].Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                                    switch (grids[g].Columns[i].Name.ToString())
                                    {
                                        case "ID":
                                            {
                                                grids[g].Columns[i].Width = 85;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 0;
                                                break;
                                            }
                                        case "BC":
                                            {
                                                grids[g].Columns[i].Width = 110;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 1;
                                                break;
                                            }
                                        case "DESC":
                                            {
                                                grids[g].Columns[i].Width = 640;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 2;
                                                break;
                                            }
                                        case "UNIT":
                                            {
                                                grids[g].Columns[i].Width = 55;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 3;
                                                break;
                                            }
                                        case "PRICE":
                                            {
                                                grids[g].Columns[i].Width = 75;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 4;
                                                break;
                                            }
                                        case "VG":
                                            {
                                                grids[g].Columns[i].Width = 50;
                                                grids[g].Columns[i].Visible = true;
                                                grids[g].Columns[i].DisplayIndex = 5;
                                                break;
                                            }

                                        default:
                                            {
                                                grids[g].Columns[i].Visible = false;
                                                break;
                                            }

                                    }//switch
                                }//for byte i

                                if (grids[g].Columns["ID"].DisplayIndex != 0 ||
                                    grids[g].Columns["BC"].DisplayIndex != 1 ||
                                    grids[g].Columns["DESC"].DisplayIndex != 2 ||
                                    grids[g].Columns["UNIT"].DisplayIndex != 3 ||
                                    grids[g].Columns["PRICE"].DisplayIndex != 4 ||
                                    grids[g].Columns["VG"].DisplayIndex != 5)
                                    j = -1;

                            }//for ushort j
                            #endregion
                            break;
                        }
                }


            if (artLoadHasError || chqLoadHasError)
                SaveGridsView(grids, orient);

            for (g = 0; g < grids.Length; g++)
                for (i = 0; i < grids[g].Columns.Count; i++)
                {
                    switch (grids[g].Columns[i].Name.ToString())
                    {
                        case "ID": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[0]; break; }
                        case "BC": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[1]; break; }
                        case "NAME": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[2]; break; }
                        case "DESC": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[3]; break; }
                        case "UNIT": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[4]; break; }
                        case "VG": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[5]; break; }
                        case "TID": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[6]; break; }
                        case "TQ": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[7]; break; }
                        case "PACK": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[8]; ; break; }
                        case "WEIGHT": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[9]; break; }
                        case "PRICE": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[10]; break; }
                        case "PR1": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[11]; break; }
                        case "PR2": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[12]; break; }
                        case "PR3": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[13]; break; }
                        case "Q2": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[14]; break; }
                        case "Q3": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[15]; break; }

                        case "TOT": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[16]; break; }
                        case "TAX_VAL": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[17]; break; }
                        case "USEDDISC": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[18]; break; }
                        case "DISC": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[19]; break; }
                        case "SUM": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[20]; break; }
                        case "ASUM": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[21]; break; }
                        case "TAX_MONEY": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[22]; break; }
                        case "CRTOT": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[23]; break; }
                        case "ORIGPRICE": { grids[g].Columns[i].HeaderText = driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ColumnCaption[24]; break; }
                    }
                }

        }//??
        public static void SaveGridsView(DataGridView[] grids, Orientation orient)
        {
            int i = 0;

            object[] chqStyles = new object[5];
            object[] artStyles = new object[5];

            string[] chqColName = null;
            bool[] chqColVisible = null;
            bool[] chqColAutoSize = null;
            int[] chqColWidth = null;
            int[] chqColDsplIdx = null;

            string[] artColName = null;
            bool[] artColVisible = null;
            bool[] artColAutoSize = null;
            int[] artColWidth = null;
            int[] artColDsplIdx = null;

            object[] styles = new object[2];
            styles[0] = new object[2];
            styles[1] = new object[2];

            for (int g = 0; g < grids.Length; g++)
                switch (grids[g].Name)
                {
                    case "grid_Order":
                        {
                            chqColName = new string[grids[g].ColumnCount];
                            chqColVisible = new bool[grids[g].ColumnCount];
                            chqColAutoSize = new bool[grids[g].ColumnCount];
                            chqColWidth = new int[grids[g].ColumnCount];
                            chqColDsplIdx = new int[grids[g].ColumnCount];

                            for (i = 0; i < grids[g].ColumnCount; i++)
                            {
                                chqColName[i] = grids[g].Columns[i].Name;
                                chqColWidth[i] = grids[g].Columns[i].Width;
                                if (grids[g].Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                    chqColAutoSize[i] = true;
                                else
                                    chqColAutoSize[i] = false;
                                chqColVisible[i] = grids[g].Columns[i].Visible;
                                chqColDsplIdx[i] = grids[g].Columns[i].DisplayIndex;
                            }
                            break;
                        }
                    case "grid_Products":
                        {
                            artColName = new string[grids[g].ColumnCount];
                            artColVisible = new bool[grids[g].ColumnCount];
                            artColAutoSize = new bool[grids[g].ColumnCount];
                            artColWidth = new int[grids[g].ColumnCount];
                            artColDsplIdx = new int[grids[g].ColumnCount];

                            for (i = 0; i < grids[g].ColumnCount; i++)
                            {
                                artColName[i] = grids[g].Columns[i].Name;
                                artColWidth[i] = grids[g].Columns[i].Width;
                                if (grids[g].Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                                    artColAutoSize[i] = true;
                                else
                                    artColAutoSize[i] = false;
                                artColVisible[i] = grids[g].Columns[i].Visible;
                                artColDsplIdx[i] = grids[g].Columns[i].DisplayIndex;
                            }
                            break;
                        }
                }

            chqStyles[0] = chqColName;
            chqStyles[1] = chqColVisible;
            chqStyles[2] = chqColAutoSize;
            chqStyles[3] = chqColWidth;
            chqStyles[4] = chqColDsplIdx;

            artStyles[0] = artColName;
            artStyles[1] = artColVisible;
            artStyles[2] = artColAutoSize;
            artStyles[3] = artColWidth;
            artStyles[4] = artColDsplIdx;

            if (orient == Orientation.Horizontal)
            {
                ((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[0])[0] = chqStyles;
                ((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[1])[0] = artStyles;
            }
            else
            {
                ((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[0])[1] = chqStyles;
                ((object[])driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_GridsView[1])[1] = artStyles;
            }
        }//??
    }
}
