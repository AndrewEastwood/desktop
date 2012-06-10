using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;

/* 
 * DATECS FP-3530T v1.10 driver with UI (User Interface)
 * Author: Andriy Ivaskevych
 * Date: 20/11/09
 * Notes:
 *  This driver used with PayDesk only.
 *  
 */
namespace DATECS_FP3530T.DriverUI
{
    sealed class fpmain : Protocol
    {
        // Service symbols
        private byte SEQ = 0x00;
        private byte CMD = 0x00;
        private byte PRE = 0x01;
        private byte TER = 0x03;
        private byte SEP = 0x04;
        private byte POS = 0x05;
        private byte LEN = 0x00;
        private byte NAK = 0x15;
        private byte SYN = 0x16;

        // Constructor
        public fpmain()
        {
            // set driver name
            Protocol_Name = "DATECS FP-2530T";
            // set driver UI
            FP_Panel = new Tree(Protocol_Name);
            // get public functions
            ProtocolPublicFunctions = ((Tree)FP_Panel).PublicFunctions;
            // initialize number format
            NumberFormat.CurrencyDecimalSeparator = ".";
            NumberFormat.NumberDecimalSeparator = ".";
            // custom data
            InitialiseCustomDriverData();
        }

        // Custom Data
        private void InitialiseCustomDriverData()
        {
            // custom varisbles
            string[] state = new string[48]{
                "",
                "",
                "�������� �������",
                "������� ����� ����������",
                "������� �� ��'�������",
                "���� � ��� �� ���� ���������� � ��������� ������� ��������� ��������� RAM",
                "��� �������� ������� � �����������",
                "������� ��� � ������������ ��������",
                "",
                "",
                "³������� ���� ��������",
                "���� ����� ���������� �����. ������� ���������.",
                "��� �������������  2.3  ��������, ��� ������ �� ���������� ���, � ��� ��������",
                "�������� ������� ��������� ���������� �����",
                "���������� ������� �� ��������� ��� ��������� ����������� ��������",
                "��� �������� ������� ������� ������� ����������� ���������",
                "",
                "",
                "³������� ������������ ���",
                "���������� ���������� ������",
                "³������� ���������� ���",
                "���������� ���������� ������",
                "���������� ������ ��� ���������� ������",
                "���������� ������ ��� ���������� ������",
                "",
                "��������� Sw7 ��������� - ��������� ����� �� ���������� ������",
                "��������� Sw6 ��������� - �������",
                "��������� Sw5 ��������� - ������ ������� ����������� ����� �� ������� DOS/Windows 1251",
                "��������� Sw4 ��������� - ����� \"�������� �������\"",
                "��������� Sw3 ��������� - ����������� ������ ����",
                "��������� Sw2 ��������� - �������� ���-�����",
                "��������� Sw1 ��������� - �������� ���-�����",
                "",
                "",
                "",
                "Գ������� ������ � �����������",
                "Գ������� ����� ���������� ��������� �� 50 Z-����",
                "���� ����� ��������� �����",
                "",
                "������� ������� ��� ����� � ��������� ������",
                "",
                "",
                "Գ�������� � ���������� ����� �����������",
                "����������� �������� ������",
                "������� ������������",
                "",
                "Գ������� ������ ����������",
                "Գ������� ������ ����������� � ����� ReadOnly (���� ��� �������)"};

            // added custom driver data
            CustomData.Add("Status", state.Clone());
            CustomData.Add("UserNo", 1);
            CustomData.Add("UserPwd", "0000");
            CustomData.Add("DeskNo", 1);

            // added methods to crirtical space
            ErrorFlags.Add("FP_Sale", false);
            ErrorFlags.Add("FP_PayMoney", false);
            ErrorFlags.Add("FP_Payment", false);
            ErrorFlags.Add("FP_Discount", false);
        }

        // Main Access Point
        internal override object CallFunction(string name, string description, ComPort port, object[] param)
        {
            object value = new object();
            OutputData = new byte[0];
            ReadedBytes = 0;

            if (description == string.Empty)
                description = ((Tree)FP_Panel).GetDescription(name);

            switch (name)
            {
                #region Initialization
                case "SetPrintParams":
                    {
                        break;
                    }
                case "SetDateTime":
                    {
                        SetDateTime sdt = new SetDateTime(Protocol_Name, description);
                        if (sdt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetDateTime(port, sdt.NewDateTime);
                        sdt.Dispose();
                        break;
                    }
                case "Fiscalize":
                    {
                        string[] _errMsgs = new string[9] {
                                "������� ���������� ������ ���������",
                                "�� �������� ���������� �����",
                                "������������ ���������� ����� ��� ���� ���������",
                                "³������� ���",
                                "�� ������� ���� �� ���� (������ Z-���)",
                                "�� ����� �������� ������",
                                "���������� ����� ���������� � ����",
                                "���� ������ (����������) ������",
                                "������� �� ������������"};
                        break;
                    }
                case "SetTaxRate":
                    {
                        SetTaxRate str = new SetTaxRate(Protocol_Name, description);
                        if (str.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] taxData = SetTaxRate(port, str.Password, str.DecimalPoint, str.UseRates, str.Rates);
                            string _infotext = string.Empty;

                            if (Methods.IsEmpty(taxData))
                            {
                                _infotext = string.Format("{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00} [{5}]\r\n{6}: {7:0.00} [{8}]\r\n{9}: {10:0.00} [{11}]\r\n{12}: {13:0.00} [{14}]",
                                    "�������� ������",
                                    "ʳ������ ����� ���� ����", (byte)taxData[0],
                                    "����� �", (double)taxData[5], (bool)taxData[1] ? "���������" : "�� ���������",
                                    "����� �", (double)taxData[6], (bool)taxData[2] ? "���������" : "�� ���������",
                                    "����� �", (double)taxData[7], (bool)taxData[3] ? "���������" : "�� ���������",
                                    "����� �", (double)taxData[8], (bool)taxData[4] ? "���������" : "�� ���������");

                                value = taxData;
                            }
                            else
                                _infotext = "���� �����";

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        str.Dispose();
                        break;
                    }
                case "SetSaleMode":
                    {
                        SetSaleMode ssm = new SetSaleMode(Protocol_Name, description);
                        if (ssm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetSaleMode(port, ssm.SaleMode);
                        ssm.Dispose();
                        break;
                    }
                case "SetSerialNum":
                    {
                        SetSerialNum ssm = new SetSerialNum(Protocol_Name, description);
                        if (ssm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetSerialNum(port, ssm.CountryCode, ssm.SerialNumber);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "������� �������� ������";
                            if (r == 'F')
                                _infotext = "��������������� ��������� ������\r\n���������� ����� ��� �����������\r\n�� ����������� ����";
                            if (r == '\0')
                                _infotext = "���� ������ �� ��������";

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        ssm.Dispose();
                        break;
                    }
                case "SetFixNum":
                    {
                        SetFixNum sfn = new SetFixNum(Protocol_Name, description);
                        if (sfn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetFixNum(port, sfn.FiscalNumber);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "������� �������� ������";
                            if (r == 'F')
                                _infotext = "���������� ����� �� �����������\r\n�� ����������� ����\r\n³������� ��� ��� ��������� �������� Z-���";
                            if (r == '\0')
                                _infotext = "���� ������ �� ��������";

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        sfn.Dispose();
                        break;
                    }
                case "SetTaxNum":
                    {
                        SetTaxNum stn = new SetTaxNum(Protocol_Name, description);
                        if (stn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetTaxNum(port, stn.TaxNumber, stn.TaxType);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "������� �������� ������";
                            if (r == 'F')
                                _infotext = "������� ��������� �������";
                            if (r == '\0')
                                _infotext = "���� ������ �� ��������";

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        stn.Dispose();
                        break;
                    }
                case "SetUserPass":
                    {
                        SetUserPass sup = new SetUserPass(Protocol_Name, description);
                        if (sup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetUserPass(port, sup.OldPassword, sup.NewPassword, sup.UserNo);
                        sup.Dispose();
                        break;
                    }
                case "SetUserName":
                    {
                        SetUserName sun = new SetUserName(Protocol_Name, description);
                        if (sun.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetUserName(port, sun.Password, sun.UserNo, sun.UserName);
                        sun.Dispose();
                        break;
                    }
                case "ResetUserData":
                    {
                        ResetUserData sun = new ResetUserData(Protocol_Name, description);
                        if (sun.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ResetUserData(port, sun.Password, sun.UserNo);
                        sun.Dispose();
                        break;
                    }
                case "SetGetArticle":
                    {
                        SetGetArticle sga = new SetGetArticle(Protocol_Name, description);
                        if (sga.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] artInfo = SetGetArticle(port, sga.Option, sga.Param);

                            string[] _errorMsgs = new string[2]{
                                "������� �������� ������",
                                "������� ��������� �������"};
                            string _infotext = string.Empty;
                            string _infoFormat = string.Empty;
                            if (!Methods.IsEmpty(artInfo) && artInfo[0].ToString() != "F")
                                switch (sga.Option)
                                {
                                    case 'I':
                                        {
                                            _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}",
                                                "�������� ����������",
                                                "������������ ����� ����� ������", artInfo[0],
                                                "������ ������", artInfo[1],
                                                "���������������", artInfo[2]);
                                            break;
                                        }
                                    case 'D':
                                    case 'C':
                                    case 'P':
                                        {
                                            _infotext = _errorMsgs[0];
                                            break;
                                        }
                                    case 'F':
                                    case 'N':
                                    case 'R':
                                        {
                                            _infoFormat = "{0}\r\n\r\n{1}: {2}\r\n{3} - {4}\r\n{5}: {6}\r\n{7}: {8:0.00}\r\n";
                                            _infoFormat += "{9}: {10}\r\n{11}: {12:0.00}\r\n{13}: {14}\r\n{15}: {16:0.00}\r\n{17}: {18}";
                                            _infotext = string.Format(_infoFormat,
                                                "���������� ��� �����",
                                                "����� ������", artInfo[1],
                                                "��������� �����", artInfo[2],
                                                "�����", artInfo[3],
                                                "ֳ��", double.Parse(artInfo[4].ToString(), NumberFormat),
                                                "ʳ������ �������", double.Parse(artInfo[5].ToString(), NumberFormat),
                                                "���� �������", double.Parse(artInfo[6].ToString(), NumberFormat),
                                                "ʳ������ � ����� ����", double.Parse(artInfo[7].ToString(), NumberFormat),
                                                "���� � ����� ����", double.Parse(artInfo[8].ToString(), NumberFormat),
                                                "�����", artInfo[9]);
                                            break;
                                        }
                                }
                            else
                                _infotext = _errorMsgs[1];

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        sga.Dispose();
                        break;
                    }
                case "LoadLogo":
                    {
                        LoadLogo ll = new LoadLogo(Protocol_Name, description);
                        if (ll.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LoadLogo(port, ll.Password, ll.Logo);
                        ll.Dispose();
                        break;
                    }
                case "SetAdminPass":
                    {
                        SetAdminPass sap = new SetAdminPass(Protocol_Name, description);
                        if (sap.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetAdminPass(port, sap.OldPassword, sap.NewPassword);
                        sap.Dispose();
                        break;
                    }
                case "ResetUserPass":
                    {
                        ResetUserPass sup = new ResetUserPass(Protocol_Name, description);
                        if (sup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ResetUserPass(port, sup.Password, sup.UserNo);
                        sup.Dispose();
                        break;
                    }
                #endregion
                #region Other
                case "SetGetMoney":
                    {
                        SetGetMoney sgm = new SetGetMoney(Protocol_Name, description);
                        if (sgm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            double[] money = SetGetMoney(port, sgm.Money);
                            string _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}",
                                "Գ������� �������",
                                "�������� ���� ������ �� ����", money[0],
                                "�������� ���� ��������� �� ����", money[1],
                                "���� � ���", money[2]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = money.Clone();
                        }
                        sgm.Dispose();
                        break;
                    }
                case "PrintDiagInfo":
                    {
                        PrintDiagInfo(port);
                        break;
                    }
                case "Beep":
                    {
                        Beep(port);
                        break;
                    }
                case "OpenBox":
                    {
                        OpenBox ob = new OpenBox(Protocol_Name, description);
                        if (ob.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            OpenBox(port, ob.Impulse);

                        value = new object[1] { ob.Impulse };
                        break;
                    }
                #endregion
                #region Display
                case "DisplText":
                    {
                        DisplText dt = new DisplText(Protocol_Name, description);
                        if (dt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplText(port, dt.TextLine);
                        dt.Dispose();
                        break;
                    }
                case "DisplBotLine":
                    {
                        DisplBotLine dbl = new DisplBotLine(Protocol_Name, description);
                        if (dbl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplBotLine(port, dbl.BottomLine);
                        dbl.Dispose();
                        break;
                    }
                case "DisplTopLine":
                    {
                        DisplTopLine dtl = new DisplTopLine(Protocol_Name, description);
                        if (dtl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplTopLine(port, dtl.TopLine);
                        dtl.Dispose();
                        break;
                    }
                case "ClrDispl":
                    {
                        ClrDispl(port);
                        break;
                    }
                case "DisplayDateTime":
                    {
                        DisplayDateTime(port);
                        break;
                    }
                #endregion
                #region DayReport
                case "ReportXZ":
                    {
                        ReportXZ rxz = new ReportXZ(Protocol_Name, description);
                        if (rxz.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] xzInfo = ReportXZ(port, rxz.Password, rxz.ReportType, new bool[] { rxz.ClearUserSumm, rxz.ClearArtsSumm });
                            string _infotext = string.Empty;
                            if (!Methods.IsEmpty(xzInfo))
                            {
                                string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                                _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                                _infotext = string.Format(_infoFormat,
                                    "���������� ��������� " + (rxz.ReportType == 0 ? "Z" : "X") + "-����",
                                    "����� ����", (uint)xzInfo[0],
                                    "���� �� ���� �", (double)xzInfo[1],
                                    "���� �� ���� �", (double)xzInfo[2],
                                    "���� �� ���� �", (double)xzInfo[3],
                                    "���� �� ���� �", (double)xzInfo[4],
                                    "���� �� ���� �", (double)xzInfo[5]);
                            }

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = xzInfo.Clone();
                        }
                        rxz.Dispose();
                        break;
                    }
                #endregion
                #region Reports
                case "ReportByTax":
                    {
                        ReportByTax rbt = new ReportByTax(Protocol_Name, description);
                        if (rbt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByTax(port, rbt.Password, rbt.StartDate, rbt.EndDate);
                        rbt.Dispose();
                        break;
                    }
                case "ReportByNoFull":
                    {
                        ReportByNoFull rbnf = new ReportByNoFull(Protocol_Name, description);
                        if (rbnf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByNoFull(port, rbnf.Password, rbnf.StartNo, rbnf.EndNo);
                        rbnf.Dispose();
                        break;
                    }
                case "ReportByDateFull":
                    {
                        ReportByDateFull rbdf = new ReportByDateFull(Protocol_Name, description);
                        if (rbdf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByDateFull(port, rbdf.Password, rbdf.StartDate, rbdf.EndDate);
                        rbdf.Dispose();
                        break;
                    }
                case "ReportByDateShort":
                    {
                        ReportByDateShort rbds = new ReportByDateShort(Protocol_Name, description);
                        if (rbds.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByDateShort(port, rbds.Password, rbds.StartDate, rbds.EndDate);
                        rbds.Dispose();
                        break;
                    }
                case "ReportByNoShort":
                    {
                        ReportByNoShort rbns = new ReportByNoShort(Protocol_Name, description);
                        if (rbns.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByNoShort(port, rbns.Password, rbns.StartNo, rbns.EndNo);
                        rbns.Dispose();
                        break;
                    }
                case "ReportByUsers":
                    {
                        ReportByUsers rbu = new ReportByUsers(Protocol_Name, description);
                        if (rbu.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByUsers(port, rbu.Password);
                        rbu.Dispose();
                        break;
                    }
                case "ReportByArts":
                    {
                        ReportByArts rba = new ReportByArts(Protocol_Name, description);
                        if (rba.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByArts(port, rba.Password, rba.ReportMode);
                        rba.Dispose();
                        break;
                    }
                #endregion
                #region Selling
                case "PrintCopy":
                    {
                        PrintCopy pc = new PrintCopy(Protocol_Name, description);
                        if (pc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintCopy(port, pc.Copies);
                        pc.Dispose();
                        break;
                    }
                case "ResetOrder":
                    {
                        string _infotext = string.Format("{0}", "��������� ������� ����������");
                        System.Windows.Forms.DialogResult _res = System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                        if (_res == System.Windows.Forms.DialogResult.Yes)
                            ResetOrder(port);
                        break;
                    }
                case "PrintFText":
                    {
                        PrintFText pft = new PrintFText(Protocol_Name, description);
                        if (pft.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintFText(port, pft.FixText);
                        pft.Dispose();
                        break;
                    }
                case "PrintNText":
                    {
                        PrintNText pnt = new PrintNText(Protocol_Name, description);
                        if (pnt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintNText(port, pnt.NonFixText);
                        pnt.Dispose();
                        break;
                    }
                case "OpenNOrder":
                    {
                        ushort[] oinfo = OpenNOrder(port);
                        if (oinfo[3] != 0)
                        {
                            string[] _errMsgs = new string[4]{
                                "�� ������������� ��������� ���'���",
                                "³������� ���������� ���",
                                "��� �������� ������������ ���",
                                "�� ����������� ���� � ���"};
                            System.Windows.Forms.DialogResult _res = System.Windows.Forms.MessageBox.Show(_errMsgs[oinfo[3] - 1], Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);
                        }

                        value = oinfo.Clone();
                        break;
                    }
                case "CloseNOrder":
                    {
                        ushort[] oinfo = CloseNOrder(port);
                        value = oinfo.Clone();
                        break;
                    }
                case "CloseFOrder":
                    {
                        CloseFOrder(port);
                        break;
                    }
                case "OpenFOrder":
                    {
                        //OpenFOrder(port);
                        break;
                    }
                #endregion
                #region Printer commands
                case "CutChq":
                    {
                        CutChq(port);
                        break;
                    }
                case "LineFeed":
                    {
                        LineFeed lf = new LineFeed(Protocol_Name, description);
                        if (lf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LineFeed(port, lf.Lines);
                        lf.Dispose();
                        break;
                    }
                #endregion
                #region Get Info
                case "GetDateTime":
                    {
                        DateTime dtime = GetDateTime(port);
                        string _infotext = string.Format("{0}: {1}", "������� ���� ��������", dtime.ToString("dd-MM-yyyy HH:mm:ss"));
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = dtime;
                        break;
                    }
                case "GetLastZReport":
                    {
                        GetLastZReport glzr = new GetLastZReport(Protocol_Name, description);
                        if (glzr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] zrep = GetLastZReport(port, glzr.ReportMode);
                            string _infotext = string.Empty;
                            if (!Methods.IsEmpty(zrep))
                            {
                                string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                                _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                                _infotext = string.Format(_infoFormat,
                                    "���������� ���������� Z-����",
                                    "����� ����", (uint)zrep[0],
                                    "���� �� ���� �", (double)zrep[1],
                                    "���� �� ���� �", (double)zrep[2],
                                    "���� �� ���� �", (double)zrep[3],
                                    "���� �� ���� �", (double)zrep[4],
                                    "���� �� ���� �", (double)zrep[5]);
                            }
                            else
                                _infotext = "���� �����";

                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = zrep.Clone();
                        }
                        glzr.Dispose();
                        break;
                    }
                case "GetSummsByDay":
                    {
                        GetSummsByDay gsbd = new GetSummsByDay(Protocol_Name, description);
                        if (gsbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            double[] summ = GetSummsByDay(port, gsbd.ReportMode);
                            string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                            _infoFormat += "{7}: {8:0.00}\r\n{9}: {10:0.00}";
                            string _infotext = string.Format(_infoFormat,
                                "���������� ��� �� ����",
                                "���� �� ���� �", (double)summ[0],
                                "���� �� ���� �", (double)summ[1],
                                "���� �� ���� �", (double)summ[2],
                                "���� �� ���� �", (double)summ[3],
                                "���� �� ���� �", (double)summ[4]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = summ.Clone();
                        }
                        gsbd.Dispose();
                        break;
                    }
                case "GetSumCorByDay":
                    {
                        object[] scorr = GetSumCorByDay(port);
                        string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                        string _infotext = string.Format(_infoFormat,
                            "���� �������� �� ����",
                            "���� ��� �������", (double)scorr[0],
                            "���� �� ������� ���������", (double)scorr[1],
                            "���� ������ �� �������", (double)scorr[2],
                            "���������� ����", (ushort)scorr[3],
                            "Գ������ ����", (ushort)scorr[4],
                            "���� ����������", (ushort)scorr[5]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = scorr.Clone();
                        break;
                    }
                case "GetFreeMem":
                    {
                        uint fmem = GetFreeMem(port);
                        string _infotext = string.Format("{0} {1} {2}", "����� ����� ���'��:", fmem, "������");
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = fmem;
                        break;
                    }
                case "GetState":
                    {
                        byte[] _state = GetState(port);
                        string _infotext = string.Empty;
                        for (int i = 0; i < _state.Length; i++)
                        {
                            if (((string[])CustomData["Status"])[_state[i]].Length == 0)
                                continue;
                            _infotext += ((string[])CustomData["Status"])[_state[i]] + "\r\n";
                            if (i != 0 && _state[i] == 0)
                                break;
                        }
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = _infotext.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Clone();
                        break;
                    }
                case "GetFixTransState":
                    {
                        GetFixTransState gfts = new GetFixTransState(Protocol_Name, description);
                        if (gfts.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] transinfo = GetFixTransState(port, gfts.TransParam);
                            string _infoFormat = "{0}\r\n\r\n{1} {2}\r\n{3}: {4}\r\n{5}: {6:0.00}{7}{8}";
                            string _infotext = string.Format(_infoFormat,
                                "���� ��������� ����������",
                                "���", ((byte)transinfo[0] == 1) ? "��������" : "�� ��������",
                                "˳������� ���������� ����", transinfo[1],
                                "���� �� ����", (double)transinfo[2],
                                (transinfo[3] == null) ? "" : ("\r\n������ �� ����: " + string.Format("{0:0.00}", (double)transinfo[3])),
                                (transinfo[4] == null) ? "" : ("\r\n������� ������ �������� " + ((byte)transinfo[4] == 1 ? "������" : "�� ������")));
                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = transinfo.Clone();
                        }

                        break;
                    }
                case "GetDiagInfo":
                    {
                        string[] dinfo = GetDiagInfo(port);
                        string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4}\r\n{5}: {6}\r\n{7}: {8}";
                        _infoFormat += "\r\n{9} - {10}\r\n{11}: {12}\r\n{13}: {14}";
                        string _infotext = string.Format(_infoFormat,
                            "ĳ���������� ����������",
                            "����� ��", dinfo[0],
                            "���� ���", DateTime.Parse(dinfo[1] + " " + dinfo[2].Insert(2, ":")).ToString("dd/MM/yyyy HH:mm"),
                            "���������� ����", dinfo[3],
                            "���������� Sw", dinfo[4],
                            "��� �����", dinfo[5],
                            "������� �����", dinfo[6],
                            "Գ�������� �����", dinfo[7]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = dinfo.Clone();
                        break;
                    }
                case "GetTaxRates":
                    {
                        double[] taxs = GetTaxRates(port);
                        string _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}%\r\n{3}: {4:0.00}%\r\n{5}: {6:0.00}%\r\n{7}: {8:0.00}%",
                            "�������� ������",
                            "�", taxs[0], "�", taxs[1], "�", taxs[2], "�", taxs[3]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = taxs.Clone();
                        break;
                    }
                case "GetTaxID":
                    {
                        string[] taxid = GetTaxID(port);
                        string _infotext = string.Format("{0}\r\n\r\n{1} {2}",
                            "���������� ���������������� �����", taxid[0], (taxid[1] == "0") ? "��" : "��");
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = taxid.Clone();
                        break;
                    }
                case "GetChqInfo":
                    {
                        object[] cinfo = GetChqInfo(port);
                        string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8:0.00}\r\n{9}: {10:0.00}\r\n{11}: {12:0.00}\r\n{13} - {14}";
                        string _infotext = string.Format(_infoFormat,
                            "���������� ��������� ����������� ����",
                            "�������� ����������", ((byte)cinfo[0] == 1) ? "ͳ" : "���",
                            "���� �� ���� �", (double)cinfo[1],
                            "���� �� ���� �", (double)cinfo[2],
                            "���� �� ���� �", (double)cinfo[3],
                            "���� �� ���� �", (double)cinfo[4],
                            "���� �� ���� *", (double)cinfo[5],
                            "���������� ���", ((byte)cinfo[6] == 1) ? "���" : "ͳ");
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = cinfo.Clone();
                        break;
                    }
                case "GetPaymentInfo":
                    {
                        object[] pinfo = GetPaymentInfo(port);
                        string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8:0.00}\r\n{9} - {10}\r\n{11} - {12}\r\n{13} - {14}";
                        string _infotext = string.Format(_infoFormat,
                            "���������� �� ����� ����������",
                            "������", (double)pinfo[0],
                            "������", (double)pinfo[1],
                            "������� ������", (double)pinfo[2],
                            "���", (double)pinfo[3],
                            "����� ��������� Z-����", (uint)pinfo[4],
                            "����� ���������� Z-����", (uint)pinfo[5],
                            "���������� �������� ����", (uint)pinfo[6]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = pinfo.Clone();
                        break;
                    }
                case "GetUserInfo":
                    {
                        GetUserInfo gui = new GetUserInfo(Protocol_Name, description);
                        if (gui.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] uinfo = GetUserInfo(port, gui.UserNo);
                            string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: [{4}]; {5:0.00}\r\n{6}: [{7}]; {8:0.00}\r\n";
                            _infoFormat += "{9}: [{10}]; {11:0.00}\r\n{12}: [{13}]; {14:0.00}\r\n{15}: [{16}]; {17:0.00}\r\n{18}: {19}";
                            string _infotext = string.Format(_infoFormat,
                                "���������� ��� ������",
                                "���������� ����", (uint)uinfo[0],
                                "ʳ������ � ���� �������", (uint)uinfo[1], (double)uinfo[2],
                                "ʳ������ � ���� ������", (uint)uinfo[3], (double)uinfo[4],
                                "ʳ������ � ���� ���������", (uint)uinfo[5], (double)uinfo[6],
                                "ʳ������ � ���� ���� ���������", (uint)uinfo[7], (double)uinfo[8],
                                "����", (uint)uinfo[9], (double)uinfo[10],
                                "�������������", uinfo[11].ToString());
                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = uinfo.Clone();
                        }
                        gui.Dispose();

                        break;
                    }
                case "GetLastFChqNo":
                    {
                        uint cno = GetLastFChqNo(port);
                        string _infotext = string.Format("{0} {1}", "����� ���������� ����������� ����:", cno);
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = cno;
                        break;
                    }
                case "GetFixMem":
                    {
                        GetFixMem gfm = new GetFixMem(Protocol_Name, description);
                        if (gfm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] fmem = GetFixMem(port, gfm.FixNo, gfm.ReturnType, gfm.SecondaryParam);
                            string _infoFormat = "{0}\r\n\r\n{1} {2}\r\n{3}: {4}\r\n{5}: {6}\r\n{7}: {8}\r\n{9} {10}";
                            string _infotext = string.Format(_infoFormat,
                                "���������� ��������� ���'��",
                                "���", fmem[0],
                                "˳������� ���������� ����", fmem[1],
                                "���� �� ����", fmem[3],
                                "������ �� ����", fmem[4],
                                "������� ������ ��������", fmem[5]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = fmem.Clone();
                        }
                        break;
                    }
                case "GetRemTime":
                    {
                        object[] remtime = GetRemTime(port);
                        string _infotext = string.Format("{0}", "�������� ���� �� ���������� ����");
                        System.Windows.Forms.MessageBox.Show(_infotext, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = remtime.Clone();
                        break;
                    }
                #endregion

                #region Program access methods
                case "FP_SendCustomer":
                    {
                        FP_SendCustomer(port, param);
                        break;
                    }
                case "FP_SetCashier":
                    {
                        FP_SetCashier(port, param);
                        break;
                    }
                case "FP_LastZRepNo":
                    {
                        FP_LastZRepNo(port, param);
                        break;
                    }
                case "FP_LastChqNo":
                    {
                        value = FP_LastChqNo(port, param);
                        break;
                    }
                case "FP_OpenBox":
                    {
                        FP_OpenBox(port);
                        break;
                    }


                #endregion
                #region
                /*
                #region Functions of registration
                case "SendStatus":
                    {
                        object[] FPinfo = SendStatus(port);

                        if (FPinfo != null && FPinfo.Length != 0)
                        {
                            FP_Info fi = new FP_Info(FPinfo, Protocol_Name);
                            fi.ShowDialog();
                            fi.Dispose();
                        }

                        break;
                    }
                case "GetDate":
                    {
                        System.Windows.Forms.MessageBox.Show(GetDate(port).ToLongDateString(),Protocol_Name);
                        break;
                    }
                case "SetDate":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetDate sd = new SetDate(Protocol_Name, description);
                            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetDate(port, sd.date);
                            sd.Dispose();
                        }
                        else
                            SetDate(port, (DateTime)param[0]);

                        break;
                    }
                case "GetTime":
                    {
                        System.Windows.Forms.MessageBox.Show(GetTime(port).ToLongTimeString(),
                            Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "SetTime":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetTime st = new SetTime(Protocol_Name, description);
                            if (st.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetTime(port, st.time);
                            st.Dispose();
                        }
                        else
                            SetTime(port, (DateTime)param[0]);

                        break;
                    }
                case "SetCod":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetCod sc = new SetCod(Protocol_Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetCod(port, sc.oldPass, sc.nom, sc.newPass);
                            sc.Dispose();
                        }
                        else
                            SetCod(port, (uint)param[0], (byte)param[1], (uint)param[2]);

                        break;
                    }
                case "SetCashier":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetCashier sc = new SetCashier(Protocol_Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetCashier(port, sc.nom, sc.pass, sc.id);
                            sc.Dispose();
                        }
                        else
                            SetCashier(port, Convert.ToByte(param[0]), (uint)param[1], param[2].ToString());

                        break;
                    }
                case "PayMoney":
                    {
                        if (Errors[name])
                            ResetOrder(port);

                        uint articleNewID = LoadArtID(port);

                        if (articleNewID + 50 == uint.MaxValue)
                        {
                            System.Windows.Forms.MessageBox.Show("�������� ������� Z-��� ��� ���������� �������", 
                                Protocol_Name, System.Windows.Forms.MessageBoxButtons.OK, 
                                System.Windows.Forms.MessageBoxIcon.Information);
                            return null;
                        }

                        if (param == null || param.Length == 0)
                        {
                            PayMoney s = new PayMoney(Protocol_Name, description);
                            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < s.articles.Length; i++)
                                {
                                    article = (object[])s.articles[i];
                                    articleNewID++;
                                    PayMoney(port, (double)article[0], (byte)article[1], s.dontPrintOne, 
                                        (double)article[2], article[3].ToString()[0], 
                                        article[4].ToString(), articleNewID);
                                }
                            }
                            s.Dispose();
                        }
                        else
                        {
                            System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                            object[] article = new object[5];
                            byte ppt = Convert.ToByte(param[1]);
                            bool useTotDisc = (bool)param[2];

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                article[0] = Methods.GetDouble(dTable.Rows[i]["TOT"]);
                                article[1] = ppt;
                                article[2] = Methods.GetDouble(dTable.Rows[i]["PRICE"]);
                                article[3] = dTable.Rows[i]["VG"];
                                article[4] = dTable.Rows[i]["NAME"].ToString().Replace('�', 'i').Replace('�', 'I');
                                articleNewID++;
                                PayMoney(port, (double)article[0], (byte)article[1], true, 
                                    (double)article[2], article[3].ToString()[0], 
                                    article[4].ToString(), articleNewID);
                                if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                                    Discount(port, (byte)0, (double)dTable.Rows[i]["DISC"], ppt, "");
                            }
                        }
                        SaveArtID(articleNewID);
                        break;
                    }
                case "Comment":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Comment cmm = new Comment(Protocol_Name, description);
                            if (cmm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Comment(port, cmm.commentLine, cmm.retCheque);
                            cmm.Dispose();
                        }
                        else
                            Comment(port, param[0].ToString(), (bool)param[1]);

                        break;
                    }
                case "CplPrint":
                    {
                        CplPrint(port);
                        byte[] mem = GetMemory(port, "2A", 0, 1);
                        //BIT 6 - OnLine state
                        string _status = string.Format("{0} {1}", "���� � ������������� �����", (mem[0] & 0x08) != 0 ? "����������" : "�� ����������");
                        System.Windows.Forms.MessageBox.Show(_status, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "LineFeed":
                    {
                        LineFeed(port);
                        break;
                    }
                case "ResetOrder":
                    {
                        ResetOrder(port);
                        break;
                    }
                case "Avans":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Avans a = new Avans(Protocol_Name, description);
                            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Avans(port, a.copecks);
                            a.Dispose();
                        }
                        else
                            Avans(port, Convert.ToUInt32(param[0]));

                        break;
                    }
                case "Sale":
                    {
                        if (Errors[name])
                            ResetOrder(port);

                        uint articleNewID = LoadArtID(port);

                        if (articleNewID + 50 == uint.MaxValue)
                        {
                            System.Windows.Forms.MessageBox.Show("�������� ������� Z-��� ��� ���������� �������", 
                                Protocol_Name, 
                                System.Windows.Forms.MessageBoxButtons.OK, 
                                System.Windows.Forms.MessageBoxIcon.Warning);
                            return null;
                        }

                        if (param == null || param.Length == 0)
                        {
                            Sale s = new Sale(Protocol_Name, description);
                            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < s.articles.Length; i++)
                                {
                                    article = (object[])s.articles[i];
                                    articleNewID++;
                                    Sale(port, (double)article[0], 
                                        (byte)article[1], s.dontPrintOne, 
                                        (double)article[2], article[3].ToString()[0],
                                        article[4].ToString(), articleNewID, (byte)article[6]);
                                }
                            }
                            s.Dispose();
                        }
                        else
                        {
                            System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                            object[] article = new object[5];
                            byte dose_ppt = Convert.ToByte(param[1]);
                            byte money_ppt = Convert.ToByte(param[3]);
                            bool useTotDisc = (bool)param[2];

                            System.IO.StreamWriter sWr = null;

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                article[0] = Methods.GetDouble(dTable.Rows[i]["TOT"]);
                                article[2] = Methods.GetDouble(dTable.Rows[i]["PRICE"]);
                                article[3] = dTable.Rows[i]["VG"];
                                article[4] = dTable.Rows[i]["NAME"].ToString().Replace('�', 'i').Replace('�', 'I');
                                articleNewID++;

                                sWr = System.IO.File.AppendText("a.txt");
                                sWr.WriteLine(articleNewID);
                                sWr.Close();
                                sWr.Dispose();
                                Sale(port, (double)article[0], dose_ppt, false, (double)article[2],
                                    article[3].ToString()[0], article[4].ToString(), articleNewID, money_ppt);
                                if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                                    Discount(port, (byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, "");
                            }
                            sWr = System.IO.File.AppendText("a.txt");
                            sWr.WriteLine("-----");
                            sWr.Close();
                            sWr.Dispose();
                        }

                        SaveArtID(articleNewID);
                        break;
                    }
                case "Payment":
                    {

                        if (param == null || param.Length == 0)
                        {
                            Payment p = new Payment(Protocol_Name, description);
                            if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] ans = Payment(port, (byte)p.payInfo[0], (bool)p.payInfo[1], 
                                    (double)p.payInfo[2], (bool)p.payInfo[3], p.payInfo[4].ToString());
                                try
                                {
                                    string info = string.Empty;
                                    if (ans[0].ToString() == "1")
                                        info += "����� : ";
                                    else
                                        info += "������� : ";

                                    info += string.Format("{0:F2}", ans[1]);
                                    System.Windows.Forms.MessageBox.Show(info, Protocol_Name,
                                        System.Windows.Forms.MessageBoxButtons.OK,
                                        System.Windows.Forms.MessageBoxIcon.Information);
                                }
                                catch { }
                            }
                            p.Dispose();
                        }
                        else
                            Payment(port, (byte)param[0], (bool)param[1], (double)param[2], 
                                (bool)param[3], param[4].ToString());

                        break;
                    }
                case "SetString":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetString ss = new SetString(Protocol_Name, description);
                            if (ss.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetString(port, ss.lines);
                            ss.Dispose();
                        }
                        else
                            SetString(port, (string[])param);

                        break;
                    }
                case "Give":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Give g = new Give(Protocol_Name, description);
                            if (g.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Give(port, g.copecks);
                            g.Dispose();
                        }
                        else
                            Give(port, (uint)param[0]);

                        break;
                    }
                case "SendCustomer":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SendCustomer sc = new SendCustomer(Protocol_Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SendCustomer(port, sc.lines, sc.show);
                            sc.Dispose();
                        }
                        else
                            SendCustomer(port, (string[])param[0], (bool[])param[1]);

                        break;
                    }
                case "GetMemory":
                    {
                        if (param == null || param.Length == 0)
                        {
                            GetMemory gm = new GetMemory(Protocol_Name, description);
                            if (gm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                byte[] mem = GetMemory(port, gm.block, gm.page, gm.size);
                                string memInfoLine = "DEC :";
                                for (int i = 0; i < mem.Length; i++)
                                {
                                    if (i % 10 == 0)
                                        memInfoLine += "\r\n";
                                    memInfoLine += mem[i].ToString() + " ";
                                }
                                memInfoLine += "\r\n\r\nHEX :";
                                for (int i = 0; i < mem.Length; i++)
                                {
                                    if (i % 10 == 0)
                                        memInfoLine += "\r\n";
                                    memInfoLine += String.Format("{0:X2}", mem[i]) + " ";
                                }
                                System.Windows.Forms.MessageBox.Show(memInfoLine, Protocol_Name,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Information);
                            }
                            gm.Dispose();
                        }
                        else
                        {
                            byte[] mem = GetMemory(port, param[0].ToString(), Convert.ToByte(param[1]), Convert.ToByte(param[2]));
                            value = new object[1];
                            value[0] = Methods.GetNumberFromBCD(mem);
                        }

                        break;
                    }
                case "OpenBox":
                    {
                        OpenBox(port);
                        break;
                    }
                case "PrintCopy":
                    {
                        PrintCopy(port);
                        break;
                    }
                case "PrintVer":
                    {
                        PrintVer(port);
                        break;
                    }
                case "GetBox":
                    {
                        uint copecks = GetBox(port);
                        string _status = string.Format("� ����� : {0}", (double)(copecks / 100) + (double)(copecks % 100) / 100);
                        System.Windows.Forms.MessageBox.Show(_status, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK, 
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "Discount":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Discount d = new Discount(Protocol_Name, description);
                            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Discount(port, (byte)d.discInfo[0], (double)d.discInfo[1], Convert.ToByte(d.discInfo[2]), d.discInfo[3].ToString());
                            d.Dispose();
                        }
                        else
                            Discount(port, (byte)param[0], (double)param[1], Convert.ToByte(param[2]), param[3].ToString());

                        break;
                    }
                case "CplOnline":
                    {
                        CplOnline(port);
                        byte[] mem = GetMemory(port, "2A", 0, 1);
                        //BIT 6 - OnLine state
                        string _status = string.Format("{0} {1}", "����� OnLine", (mem[0] & 0x40) != 0 ? "��������" : "��������");
                        System.Windows.Forms.MessageBox.Show(_status, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "CplInd":
                    {
                        CplInd(port);
                        byte[] mem = GetMemory(port, "29", 0, 1);
                        //BIT 7 - Indicator state
                        string _status = string.Format("{0} {1}", "������ ���� �� ���������", (mem[0] & 0x80) != 0 ? "�� �������" : "�������");
                        System.Windows.Forms.MessageBox.Show(_status, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "ChangeRate":
                    {
                        if (param == null || param.Length == 0)
                        {
                            ChangeRate cr = new ChangeRate(Protocol_Name, description);
                            if (cr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                ChangeRate(port, cr.rate);
                            cr.Dispose();
                        }
                        else
                            ChangeRate(port, (byte)param[0]);

                        break;
                    }
                case "LineSP":
                    {
                        if (param == null || param.Length == 0)
                        {
                            LineSP lsp = new LineSP(Protocol_Name, description);
                            if (lsp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                LineSP(port, lsp.lsp);
                            lsp.Dispose();
                        }
                        else
                            LineSP(port, (byte)param[0]);

                        break;
                    }
                case "TransPrint":
                    {
                        if (param == null || param.Length == 0)
                        {
                            TransPrint tp = new TransPrint(Protocol_Name, description);
                            if (tp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                TransPrint(port, tp.text, tp.endPrint);
                            tp.Dispose();
                        }
                        else
                            TransPrint(port, param[0].ToString(), (bool)param[0]);

                        break;
                    }
                case "GetArticle":
                    {
                        object[] artInfo = new object[0];
                        string info = string.Empty;
                        if (param == null)
                        {
                            GetArticle ga = new GetArticle(Protocol_Name, description);
                            if (ga.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                artInfo = GetArticle(port, ga.articleID);
                            ga.Dispose();
                        }
                        else
                            artInfo = GetArticle(port, (uint)param[0]);

                        if (artInfo.Length != 0)
                        {
                            info += "����� ������" + " : ";
                            info += artInfo[0].ToString() + "\r\n";
                            info += "ʳ������" + " : ";
                            info += artInfo[1].ToString() + "\r\n";
                            info += "ֳ��" + " : ";
                            info += artInfo[2].ToString() + "\r\n";
                            info += "��������� �����" + " : ";
                            info += artInfo[3].ToString() + "\r\n";
                            info += "���� ����" + " : ";
                            info += artInfo[4].ToString() + "\r\n";
                            info += "\r\n\r\n";
                            info += "�������� ��������" + "\r\n";
                            info += "ʳ������" + " : ";
                            info += artInfo[5].ToString() + "\r\n";
                            info += "���� ����" + " : ";
                            info += artInfo[6].ToString() + "\r\n";
                        }
                        else
                            info = "���������� ��� �� ����� �������";

                        System.Windows.Forms.MessageBox.Show(info, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        break;
                    }
                case "GetDayReport":
                    {
                        value = GetDayReport(port);
                        if (param == null && value != null && value.Length != 0)
                        {
                            string dayRepInfoLine = string.Empty;
                            int i = 0;
                            string[] payTypes = new string[] { "������", "������", "���", "������" };

                            double[] sales_group = (double[])((object[])value[1])[0];
                            double[] sales_forms = (double[])((object[])value[1])[1];

                            double[] pays_group = (double[])((object[])value[6])[0];
                            double[] pays_forms = (double[])((object[])value[6])[1];

                            dayRepInfoLine += "˳������� ���� ������ : " + value[0];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "˳������� ������ �� ���������� ������ � ������ ������";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < sales_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'�') + i), sales_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < sales_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], sales_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ������� �� �������� : " + value[2];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ������ �� �������� : " + value[3];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ���� ���������� �������� : " + value[4];
                            dayRepInfoLine += "\r\n\r\n";
                            dayRepInfoLine += "˳������� ���� ������ : " + value[5];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "˳������� ������ �� ���������� ������ � ������ ������";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < pays_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'�') + i), pays_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < pays_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], pays_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ������� �� �������� : " + value[7];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ������ �� �������� : " + value[8];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "����� ���� �������� ������ : " + value[9];

                            System.Windows.Forms.MessageBox.Show(dayRepInfoLine, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "GetCheckSums":
                    {
                        value = GetCheckSums(port);
                        if (value != null && value.Length != 0)
                        {
                            string checkSumsInfoLine = string.Empty;
                            int i = 0;
                            checkSumsInfoLine += "˳������� ���� �� ���������� ������";
                            checkSumsInfoLine += "\r\n";
                            double[] tax = (double[])value[0];
                            for (i = 0; i < tax.Length; i++)
                                checkSumsInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'�') + i), tax[i]);
                            checkSumsInfoLine += "\r\n";
                            double[] sum = (double[])value[1];
                            checkSumsInfoLine += "˳������� ��� ����� �� ������ �����";
                            checkSumsInfoLine += "\r\n";
                            string[] payTypes = new string[] { "������", "������", "���", "������" };
                            for (i = 0; i < sum.Length; i++)
                                checkSumsInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], sum[i]);

                            System.Windows.Forms.MessageBox.Show(checkSumsInfoLine, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "GetTaxRates":
                    {
                        object[] taxData = GetTaxRates(port);
                        if (taxData != null && taxData.Length != 0)
                        {
                            string taxInfoLine = string.Empty;
                            taxInfoLine += "�-��� ���������� ������ : " + taxData[0];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "���� ������������ : " + ((DateTime)taxData[1]).ToLongDateString();
                            taxInfoLine += "\r\n\r\n";
                            taxInfoLine += "������ ���";
                            taxInfoLine += "\r\n";
                            double[] tax = (double[])taxData[2];
                            for (int i = 0; i < tax.Length; i++)
                                taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'�') + i), tax[i]);
                            taxInfoLine += "\r\n";
                            taxInfoLine += "�-��� ���������� ������� �������� ��� : " + taxData[3];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "��� ��� : " + ((taxData[4].ToString() == "0") ? "��������" : "�������");
                            if ((bool)taxData[5])
                            {
                                taxInfoLine += "\r\n\r\n";
                                taxInfoLine += "������ �����";
                                taxInfoLine += "\r\n";
                                tax = (double[])taxData[6];
                                for (int i = 0; i < tax.Length; i++)
                                    taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'�') + i), tax[i]);
                            }
                            System.Windows.Forms.MessageBox.Show(taxInfoLine, Protocol_Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "CplCutter":
                    {
                        CplCutter(port);
                        byte[] mem = GetMemory(port, "301A", 16, 1);
                        //BIT 3 - Cutter state
                        string _status = string.Format("{0} {1}", "�������", (mem[0] & 0x08) == 0 ? "��������" : "�� ��������");
                        System.Windows.Forms.MessageBox.Show(_status, Protocol_Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                #endregion
                #region Function of programming
                case "Fiscalization":
                    {
                        Fiscalazation f = new Fiscalazation(Protocol_Name, description);
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            Fiscalization(port, f.pass, f.fn);
                        f.Dispose();

                        break;
                    }
                case "SetHeadLine":
                    {
                        SetHeadLine shl = new SetHeadLine(Protocol_Name, description);
                        if (shl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetHeadLine(port, shl.pass, shl.line1, shl.line2, shl.line3, shl.line4);
                        shl.Dispose();

                        break;
                    }
                case "SetTaxRate":
                    {
                        SetTaxRate str = new SetTaxRate(Protocol_Name, description);
                        if (str.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetTaxRate(port, str.pass, str.taxCount, str.tax, str.status, str.taxGCount, str.gtax);
                        str.Dispose();

                        break;
                    }
                case "ProgArt":
                    {
                        if (param == null || param.Length == 0)
                        {
                            ProgArt pa = new ProgArt(Protocol_Name, description);
                            if (pa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < pa.articles.Length; i++)
                                {
                                    article = (object[])pa.articles[i];

                                    ProgArt(port, pa.pass, (byte)article[0], 
                                        (double)article[1], article[2].ToString()[0],
                                        article[3].ToString(), (uint)article[4]);
                                }
                            }
                            pa.Dispose();
                        }
                        else
                        {
                            System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                            object[] article = new object[5];
                            byte ppt = Convert.ToByte(param[1]);
                            uint articleNewID = LoadArtID(port);

                            System.IO.StreamWriter sWr = null;

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                article[0] = ppt;
                                article[1] = Methods.GetDouble(dTable.Rows[i]["PRICE"]);
                                article[2] = dTable.Rows[i]["VG"];
                                article[3] = dTable.Rows[i]["NAME"].ToString().Replace('�', 'i').Replace('�', 'I');
                                articleNewID++;

                                ProgArt(port, (ushort)0, (byte)article[0], (double)article[1],
                                    article[3].ToString()[0], article[4].ToString(), articleNewID);
                            }
                        }

                        break;
                    }
                case "LoadBMP":
                    {
                        LoadBMP lbmp = new LoadBMP(Protocol_Name, description);
                        if (lbmp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LoadBMP(port, (ushort)lbmp.imageInfo[0], true, lbmp.imageInfo[1].ToString());
                        lbmp.Dispose();

                        break;
                    }
                #endregion
                #region Function of reports
                case "ArtReport":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        ArtReport(port, pass);
                        break;
                    }
                case "ArtXReport":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        ArtXReport(port, pass);
                        break;
                    }
                case "DayReport":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        DayReport(port, pass);
                        break;
                    }
                case "DayClrReport":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        DayClrReport(port, pass);

                        System.IO.StreamWriter sWr = System.IO.File.AppendText("a.txt");
                        sWr.WriteLine("---------\r\n## Z-rep ##\r\n---------");
                        sWr.Close();
                        sWr.Dispose();

                        break;
                    }
                case "PeriodicReport":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReport pr = new PeriodicReport(Protocol_Name, description);
                        if (pr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReport(port, pass, pr.startDate, pr.endDate);
                        pr.Dispose();

                        break;
                    }
                case "PeriodicReportShort":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReportShort prs = new PeriodicReportShort(Protocol_Name, description);
                        if (prs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReportShort(port, pass, prs.startDate, prs.endDate);
                        prs.Dispose();

                        break;
                    }
                case "PeriodicReport2":
                    {
                        byte[] mem = GetMemory(port, "3003", 16, 2 * 10);
                        uint pass = (uint)Methods.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReport2 pr2 = new PeriodicReport2(Protocol_Name, description);
                        if (pr2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReport2(port, pass, pr2.startNo, pr2.endNo);
                        pr2.Dispose();

                        break;
                    }
                #endregion
            */
                #endregion
            }
            return value;
        }

        // Driver implementation
        // Warning! Don't change this code region
        #region Driver implementation
        #region Functions of registration
        /*
        private object[] SendStatus(ComPort port)
        {
            lastFuncName = "SendStatus";
            CMD = 0;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);
            
            //Next code for command
            GetNextCmdCode();

            object[] ekkrInfo = null;
            if (OutputData != null && OutputData.Length != 0)
            {
                Array.Resize<object>(ref ekkrInfo, 16 + 1 + 1 + 1 + 1 + 1 + 1);

                ekkrInfo[0] = "";
                ekkrInfo[1] = string.Format("{0} : {1}", "����� ��������� ����� � ����", (OutputData[0] & 0x02) != 0 ? "���" : "Ͳ");
                ekkrInfo[2] = string.Format("{0} : {1}", "³������ ������� ��������", (OutputData[0] & 0x04) != 0 ? "���" : "Ͳ");
                ekkrInfo[3] = string.Format("{0} : {1}", "���", (OutputData[0] & 0x08) == 0 ? "�������" : "�������");
                ekkrInfo[4] = string.Format("{0} : {1}", "���", (OutputData[0] & 0x10) == 0 ? "��������" : "�����Ͳ�");
                ekkrInfo[5] = string.Format("{0} : {1}", "���� ������\\������� (���� �������)", (OutputData[0] & 0x20) != 0 ? "���" : "Ͳ");
                ekkrInfo[6] = string.Format("{0} : {1}", "³������ ���", (OutputData[0] & 0x40) != 0 ? "���" : "Ͳ");
                ekkrInfo[7] = string.Format("{0} : {1}", "�������� ������ ���� �� ���������", (OutputData[0] & 0x80) != 0 ? "���" : "Ͳ");

                ekkrInfo[8] = "";
                ekkrInfo[9] = string.Format("{0} : {1}", "���� ������ ��� �������", (OutputData[1] & 0x02) != 0 ? "���" : "Ͳ");
                ekkrInfo[10] = "";
                ekkrInfo[11] = string.Format("{0} : {1}", "�������� �����", (OutputData[1] & 0x08) != 0 ? "���" : "Ͳ");
                ekkrInfo[12] = string.Format("{0} : {1}", "������� ������������", (OutputData[1] & 0x10) != 0 ? "���" : "Ͳ");
                ekkrInfo[13] = string.Format("{0} : {1}", "������� ���������� �������� �������", (OutputData[1] & 0x20) != 0 ? "���" : "Ͳ");
                ekkrInfo[14] = string.Format("{0} : {1}", "����� OnLine ���������", (OutputData[1] & 0x40) != 0 ? "���" : "Ͳ");
                ekkrInfo[15] = "";

                string sno = string.Empty;
                for (int i = 2; i < 21; i++)
                    sno += (char)OutputData[i];

                ekkrInfo[16] = string.Format("{0} : {1}", "������� ����� � ���� �����������", sno);

                byte[] data = new byte[3];
                int[] mas = new int[3];
                int offset = 36;

                if ((OutputData[1] & 0x10) != 0)
                {
                    data[0] = OutputData[21];
                    data[1] = OutputData[22];
                    data[2] = OutputData[23];

                    try
                    {
                        mas = Methods.GetArrayFromBCD(data);
                    }
                    catch { }

                    ekkrInfo[17] = string.Format("{0} : {1:D2}-{2:D2}-{3:D2}", "���� ��������� (��-��-��)", mas[0], mas[1], mas[2]);

                    data = new byte[2];
                    mas = new int[2];

                    data[0] = OutputData[24];
                    data[1] = OutputData[25];

                    try
                    {
                        mas = Methods.GetArrayFromBCD(data);
                    }
                    catch { }

                    ekkrInfo[18] = string.Format("{0} : {1:D2}:{2:D2}", "��� ��������� (��:��)", mas[0], mas[1]);

                    data = new byte[10];
                    for (int i = 26; i < 36; i++)
                        data[i - 26] = OutputData[i];

                    sno = Encoding.GetEncoding(866).GetString(data);

                    ekkrInfo[19] = string.Format("{0} : {1}", "Գ�������� �����", sno);

                    ekkrInfo[20] = string.Format("{0} :\r\n{1}", "�������� �������� �������", "%1% %2% %3% %4%");

                    for (int o = 1; o < 5; o++)
                    {
                        sno = string.Empty;
                        for (int i = offset; i < offset + OutputData[offset]; i++)
                            sno += (char)OutputData[i];
                        if (sno.Length != 0)
                            sno += "\r\n";
                        else
                            sno = " ";

                        ekkrInfo[20] = ekkrInfo[20].ToString().Replace("%" + o + "%", sno);

                        offset += sno.Length;
                    }
                }
                else
                    offset += 4;

                //EP-01, EP-02, EP-51
                data = new byte[5];
                data[0] = OutputData[offset];
                data[1] = OutputData[offset + 1];
                data[2] = OutputData[offset + 2];
                data[3] = OutputData[offset + 3];
                data[4] = OutputData[offset + 4];

                sno = Encoding.GetEncoding(866).GetString(data);
                ekkrInfo[21] = string.Format("{0} : {1}", "����� �� ����", sno);
            }

            return ekkrInfo;
        }//make returned info
        private DateTime GetDate(ComPort port)
        {
            lastFuncName = "GetDate";
            CMD = 1;

            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            DateTime dt = new DateTime();
            try
            {
                int[] ans = Methods.GetArrayFromBCD(OutputData);
                dt = new DateTime(ans[2] + 2000, ans[1], ans[0]);
            }
            catch { }

            return dt;
        }//ok
        private void SetDate(ComPort port, DateTime date)
        {
            lastFuncName = "SetDate";
            CMD = 2;

            //Creating data
            DataForSend = Methods.GetBCDFromArray(new int[] { date.Day, date.Month, int.Parse(date.Year.ToString().Substring(2, 2)) });
            //BCD dd = new BCD(date.Day);
            //BCD mm = new BCD(date.Month);
            //BCD yy = new BCD(int.Parse(date.Year.ToString().Substring(2, 2)));

            //Data = new byte[3];
            //Data[0] = dd.CompressedBCD[0];
            //Data[1] = mm.CompressedBCD[0];
            //Data[2] = yy.CompressedBCD[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private DateTime GetTime(ComPort port)
        {
            lastFuncName = "GetTime";
            CMD = 3;

            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            DateTime dt = new DateTime();

            try
            {
                int[] ans = Methods.GetArrayFromBCD(OutputData);
                dt = new DateTime(1990, 10, 10, ans[0], ans[1], ans[2]);
            }
            catch { }

            return dt;
        }//ok
        private void SetTime(ComPort port, DateTime time)
        {
            lastFuncName = "SetTime";
            CMD = 4;

            //Creating data
            DataForSend = Methods.GetBCDFromArray(new int[] { time.Hour, time.Minute, time.Second });
            //BCD hh = new BCD(time.Hour);
            //BCD mm = new BCD(time.Minute);
            //BCD ss = new BCD(time.Second);

            //Data = new byte[3];
            //Data[0] = hh.CompressedBCD[0];
            //Data[1] = mm.CompressedBCD[0];
            //Data[2] = ss.CompressedBCD[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCod(ComPort port, uint oldPass, byte no, uint newPass)
        {
            lastFuncName = "SetCod";
            CMD = 5;

            //Creating data
            DataForSend = new byte[2 + 1 + 2];
            byte[] p = Methods.GetByteArray(oldPass, 2);
            DataForSend[0] = p[0];
            DataForSend[1] = p[1];
            DataForSend[2] = no;//0..7-cashier,8-program mode,9-report mode
            p = Methods.GetByteArray(newPass, 2);
            DataForSend[3] = p[0];
            DataForSend[4] = p[1];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCashier(ComPort port, byte n, uint pass, string name)
        {
            lastFuncName = "SetCashier";
            CMD = 6;

            //Creating data
            DataForSend = new byte[1 + 2 + 1 + name.Length];
            byte[] p = Methods.GetByteArray(pass, 2);
            DataForSend[0] = p[0];
            DataForSend[1] = p[1];
            DataForSend[2] = n;// 0..7
            DataForSend[3] = (byte)name.Length;//0..15
            byte[] BinLine = new byte[name.Length];
            BinLine = Encoding.GetEncoding(866).GetBytes(name.Replace('�', 'i').Replace('�', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 4] = BinLine[i];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] PayMoney(ComPort port, double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id)
        {
            lastFuncName = "PayMoney";
            CMD = 8;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;

            //##Creating data
            DataForSend = new byte[3 + 1 + 4 + 1 + 1 + name.Length + 6];
            //Total
            tot *= Math.Pow(10.0, (double)doseDecimal);
            binArr = Methods.GetByteArray((int)Math.Round(tot), 3);
            DataForSend[0] = binArr[0];
            DataForSend[1] = binArr[1];
            DataForSend[2] = binArr[2];
            //Status
            oneByte = doseDecimal;
            if (notPrintOne)
                oneByte |= 0x80;
            DataForSend[3] = oneByte;
            //Price
            binArr = Methods.GetByteArray((int)Math.Round(Math.Abs(price) * 100), 4);
            DataForSend[4] = binArr[0];
            DataForSend[5] = binArr[1];
            DataForSend[6] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            DataForSend[7] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case '�': { oneByte = 0x80; break; }
                case '�': { oneByte = 0x81; break; }
                case '�': { oneByte = 0x82; break; }
                case '�': { oneByte = 0x83; break; }
                case '�': { oneByte = 0x84; break; }
                case '�': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            DataForSend[8] = oneByte;
            //Art Name length
            DataForSend[9] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(DataForSend, 10);
            //Art id
            binArr = Methods.GetByteArray(id, 6);
            DataForSend[DataForSend.Length - 6] = binArr[0];
            DataForSend[DataForSend.Length - 5] = binArr[1];
            DataForSend[DataForSend.Length - 4] = binArr[2];
            DataForSend[DataForSend.Length - 3] = binArr[3];
            DataForSend[DataForSend.Length - 2] = binArr[4];
            DataForSend[DataForSend.Length - 1] = binArr[5];
            //##End Creating data

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = Methods.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = Methods.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;/*

            return ans;
        }//ok
        private void Comment(ComPort port, string text, bool retCheque)
        {
            lastFuncName = "Comment";
            CMD = 11;

            //Creating data
            DataForSend = new byte[1 + text.Length];
            DataForSend[0] = (byte)text.Length;
            if (retCheque)
                DataForSend[0] |= 0x80;

            byte[] BinLine = new byte[0];
            BinLine = Encoding.GetEncoding(866).GetBytes(text.Replace('�', 'i').Replace('�', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 1] = BinLine[i];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplPrint(ComPort port)
        {
            lastFuncName = "CplPrint";
            CMD = 12;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineFeed(ComPort port)
        {
            lastFuncName = "LineFeed";
            CMD = 14;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ResetOrder(ComPort port)
        {
            lastFuncName = "ResetOrder";
            CMD = 15;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void Avans(ComPort port, uint suma)
        {
            lastFuncName = "Avans";
            CMD = 16;

            //Creating data
            DataForSend = new byte[4];
            DataForSend = Methods.GetByteArray(suma, DataForSend.Length);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] Sale(ComPort port, double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id, byte moneyDecimal)
        {
            lastFuncName = "Sale";
            CMD = 18;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;

            //##Creating data
            DataForSend = new byte[3 + 1 + 4 + 1 + 1 + name.Length + 6];
            //Total
            tot *= Math.Pow(10.0, (double)doseDecimal);
            binArr = Methods.GetByteArray((int)Math.Round(tot), 3);
            DataForSend[0] = binArr[0];
            DataForSend[1] = binArr[1];
            DataForSend[2] = binArr[2];
            //Status
            oneByte = doseDecimal;
            if (notPrintOne)
                oneByte |= 0x80;
            DataForSend[3] = oneByte;
            //Price
            price *= Math.Pow(10.0, (double)moneyDecimal);
            binArr = Methods.GetByteArray((int)Math.Round(Math.Abs(price)), 4);
            DataForSend[4] = binArr[0];
            DataForSend[5] = binArr[1];
            DataForSend[6] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            DataForSend[7] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case '�': { oneByte = 0x80; break; }
                case '�': { oneByte = 0x81; break; }
                case '�': { oneByte = 0x82; break; }
                case '�': { oneByte = 0x83; break; }
                case '�': { oneByte = 0x84; break; }
                case '�': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            DataForSend[8] = oneByte;
            //Art Name length
            name = name.Trim();
            if (name.LastIndexOf('�') == name.Length - 1)
                name = name.Substring(0, name.Length - 1);
            name = name.Trim();
            DataForSend[9] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(DataForSend, 10);
            //Art id
            binArr = Methods.GetByteArray(id, 6);
            DataForSend[DataForSend.Length - 6] = binArr[0];
            DataForSend[DataForSend.Length - 5] = binArr[1];
            DataForSend[DataForSend.Length - 4] = binArr[2];
            DataForSend[DataForSend.Length - 3] = binArr[3];
            DataForSend[DataForSend.Length - 2] = binArr[4];
            DataForSend[DataForSend.Length - 1] = binArr[5];
            //##End Creating data

            //Making data
            InputData = CreateInputData(CMD, DataForSend);


            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = Methods.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = Methods.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;/*

            return ans;
        }//ok
        private object[] Payment(ComPort port, byte type, bool useAddRow, double cash, bool autoclose, string addRow)
        {
            CMD = 20;

            //Creating data
            DataForSend = new byte[1 + 4 + 1 + addRow.Length];
            DataForSend[0] = type;
            if (useAddRow)
                DataForSend[0] |= 0x80;

            if ((type != 3)||(autoclose))
            {
                DataForSend[1] = 0;
                DataForSend[2] = 0;
                DataForSend[3] = 0;
                DataForSend[4] = 0x80;
            }
            else
            {
                byte[] c = Methods.GetByteArray((int)Math.Round(cash * 100), 4);
                DataForSend[1] = c[0];
                DataForSend[2] = c[1];
                DataForSend[3] = c[2];
                DataForSend[4] = c[3];
            }
            
            DataForSend[5] = (byte)addRow.Length;

            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(addRow);
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 6] = BinLine[i];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            if (OutputData != null && OutputData.Length != 0)
            {
                Array.Reverse(OutputData);
                if ((OutputData[0] & 0x80) == 0)
                    ans[0] = 0;
                else
                    ans[0] = 1;
                OutputData[0] &= 0x7F;
                Array.Reverse(OutputData);
                ans[1] = Methods.GetNumber(OutputData);
                ans[1] = Convert.ToDouble(ans[1]) / 100;
            }

            return ans;
        }//ok
        private void SetString(ComPort port, string[] lines)
        {
            lastFuncName = "SetString";
            CMD = 23;

            byte[] BinLine = new byte[0];
            byte j = 0;

            for (byte i = 0; i < lines.Length; i++)
            {
                //Creating data
                DataForSend = new byte[1 + 1 + lines[i].Length];
                DataForSend[0] = i;
                DataForSend[1] = (byte)lines[i].Length;
                BinLine = Encoding.GetEncoding(866).GetBytes(lines[i]);
                for (j = 0; j < BinLine.Length; j++)
                    DataForSend[j + 2] = BinLine[j];

                //Making data
                InputData = CreateInputData(CMD, DataForSend);

                //sending and getting data
                SendGetData(port, 20, true);

                //Next code for command
                GetNextCmdCode();
            }
        }//ok
        private void Give(ComPort port, uint suma)
        {
            lastFuncName = "Give";
            CMD = 24;

            //Creating data
            DataForSend = Methods.GetByteArray(suma, 4);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SendCustomer(ComPort port, string[] lines, bool[] show)
        {
            lastFuncName = "SendCustomer";
            CMD = 27;

            byte[] BinLine = new byte[0];
            byte j = 0;

            for (byte i = 0; i < lines.Length; i++)
            {
                if (!show[i])
                    continue;

                lines[i] = lines[i].Replace('�','i').Replace('�','I');
                BinLine = Encoding.GetEncoding(866).GetBytes(lines[i]);

                //Creating data
                DataForSend = new byte[2 + BinLine.Length];
                DataForSend[0] = i;
                DataForSend[1] = (byte)BinLine.Length;
                for (j = 0; j < BinLine.Length; j++)
                    DataForSend[j + 2] = BinLine[j];

                //Making data
                InputData = CreateInputData(CMD, DataForSend);

                //sending and getting data
                SendGetData(port, 20, true);

                //Next code for command
                GetNextCmdCode();

                //System.Threading.Thread.Sleep(100);
            }
        }//ok   
        private byte[] GetMemory(ComPort port, string adrBlock, byte pageNo, byte blockSize)
        {
            lastFuncName = "GetMemory";
            CMD = 28;

            //Creating data
            DataForSend = new byte[2 + 1 + 1];
            uint block = Convert.ToUInt32(adrBlock, 16);
            byte[] blok = Methods.GetByteArray(block, 2);
            DataForSend[0] = blok[0];
            DataForSend[1] = blok[1];
            DataForSend[2] = pageNo;
            DataForSend[3] = blockSize;

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            return OutputData;
        }//ok
        private void OpenBox(ComPort port)
        {
            lastFuncName = "OpenBox";
            CMD = 29;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintCopy(ComPort port)
        {
            lastFuncName = "PrintCopy";
            CMD = 30;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintVer(ComPort port)
        {
            lastFuncName = "PrintVer";
            CMD = 32;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private uint GetBox(ComPort port)
        {
            lastFuncName = "GetBox";
            CMD = 33;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            return (uint)Methods.GetNumber(OutputData);
        }//ok
        private void Discount(ComPort port, byte type, double value, byte digitsAfterPoint, string helpLine)
        {
            CMD = 35;

            //Creating data
            DataForSend = new byte[1 + 4 + 1 + helpLine.Length];
            DataForSend[0] = type;

            bool isDiscount = true;
            if (value < 0)
                isDiscount = false;

            byte[] disc = new byte[4];
            // v 21:59 19.11.2088 vguvaemo REVO {YURIK, VEDMED, MA4OK}
            if (type == 0 || type == 2)
            {
                disc = Methods.GetByteArray((int)Math.Abs(value * 100), 4);
                disc[3] &= (byte)0;
                disc[3] |= (byte)4;
            }
            else
            {
                if (digitsAfterPoint > 127)
                    digitsAfterPoint = 127;
                if (digitsAfterPoint > 0)
                    value *= Math.Pow(10.0, (double)digitsAfterPoint);
                disc = Methods.GetByteArray((int)Math.Abs(value), 4);
                disc[3] &= (byte)0;
                disc[3] |= (byte)digitsAfterPoint;
            }

            if (isDiscount)
                disc[3] |= (byte)0x80;//znugka
            else
                disc[3] &= (byte)0x7F;//nadbavka

            DataForSend[1] = disc[0];
            DataForSend[2] = disc[1];
            DataForSend[3] = disc[2];
            DataForSend[4] = disc[3];

            DataForSend[5] = (byte)helpLine.Length;
            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(helpLine);
            for (byte j = 0; j < BinLine.Length; j++)
                DataForSend[j + 6] = BinLine[j];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//!!!!
        private void CplOnline(ComPort port)
        {
            lastFuncName = "CplOnline";
            CMD = 36;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplInd(ComPort port)
        {
            lastFuncName = "CplInd";
            CMD = 37;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ChangeRate(ComPort port, byte rateType)
        {
            lastFuncName = "ChangeRate";
            CMD = 38;

            //Creationg data
            DataForSend = new byte[1];
            DataForSend[0] = rateType;

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineSP(ComPort port, byte lsp)
        {
            lastFuncName = "LineSP";
            CMD = 39;

            //Creating data
            DataForSend = new byte[1];
            DataForSend[0] = lsp;

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void TransPrint(ComPort port, string line, bool close)
        {
            lastFuncName = "TransPrint";
            CMD = 40;

            //Creating data
            DataForSend = new byte[1 + line.Length];
            DataForSend[0] = (byte)line.Length;
            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(line.Replace('�', 'i').Replace('�', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 1] = BinLine[i];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            //System.Threading.Thread.Sleep(200);
            if (close)
            {
                //Creating data
                DataForSend = new byte[1];
                DataForSend[0] = 0xFF;

                //Making data
                InputData = CreateInputData(CMD, DataForSend);

                //sending and getting data
                if (port.IsOpen)
                    if (port.Write(InputData))
                        if (port.Read(ref OutputData, out ReadedBytes))
                            DecodeAnswer();

                //Next code for command
                GetNextCmdCode();
            }
        }//ok
        private object[] GetArticle(ComPort port, uint id)
        {
            lastFuncName = "GetArticle";
            CMD = 41;

            //Making data
            byte[] aid = Methods.GetByteArray(id, 6);
            InputData = CreateInputData(CMD, aid);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] article = new object[0];
            if (OutputData.Length != 0)
            {
                byte nameLength = (byte)OutputData[0];
                string artName = string.Empty;
                byte doseDecimal = 0;
                double dose = 0;
                double price = 0;
                char tax = ' ';
                double rotate = 0;
                double retriveDose = 0;
                byte retriveDoseDecimal = 0;
                double retriveRotate = 0;

                artName = Encoding.GetEncoding(866).GetString(OutputData, 1, nameLength);
                //dose
                byte[] mas = new byte[3];
                mas[0] = OutputData[1 + nameLength];
                mas[1] = OutputData[1 + nameLength + 1];
                mas[2] = OutputData[1 + nameLength + 2];
                dose = Methods.GetNumber(mas) + 0.0;

                doseDecimal = OutputData[1 + nameLength + 3];

                mas = new byte[4];
                mas[0] = OutputData[1 + nameLength + 3 + 1];
                mas[1] = OutputData[1 + nameLength + 3 + 1 + 1];
                mas[2] = OutputData[1 + nameLength + 3 + 1 + 2];
                mas[3] = OutputData[1 + nameLength + 3 + 1 + 3];
                price = (double)Methods.GetNumber(mas) / 100;

                tax = (char)OutputData[1 + nameLength + 3 + 1 + 4];
                switch (System.Convert.ToString(tax, 16))
                {
                    case "80": tax = '�'; break;
                    case "81": tax = '�'; break;
                    case "82": tax = '�'; break;
                    case "83": tax = '�'; break;
                    case "84": tax = '�'; break;
                    case "85": tax = '�'; break;
                }

                mas = new byte[5];
                mas[0] = OutputData[1 + nameLength + 3 + 1 + 4 + 1];
                mas[1] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 1];
                mas[2] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 2];
                mas[3] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 3];
                mas[4] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 4];
                rotate = (double)Methods.GetNumber(mas) / 100;

                mas = new byte[3];
                mas[0] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5];
                mas[1] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 1];
                mas[2] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 2];
                retriveDose = Methods.GetNumber(mas) + 0.0;

                retriveDoseDecimal = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3];

                mas = new byte[5];
                mas[0] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1];
                mas[1] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 1];
                mas[2] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 2];
                mas[3] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 3];
                mas[4] = OutputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 4];
                retriveRotate = (double)Methods.GetNumber(mas) / 100;

                dose /= System.Math.Pow(10.0, (double)doseDecimal);
                retriveDose /= System.Math.Pow(10.0, (double)retriveDoseDecimal);

                article = new object[7];
                article[0] = artName;
                article[1] = dose;
                article[2] = price;
                article[3] = tax;
                article[4] = rotate;
                article[5] = retriveDose;
                article[6] = retriveRotate;
            }

            return article;
        }//ok
        private object[] GetDayReport(ComPort port)
        {
            lastFuncName = "GetDayReport";
            CMD = 42;

            //Creating data
            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();


            object[] rezDayInfo = new object[0];
            if (OutputData.Length == 134)
            {
                rezDayInfo = new object[10];
                byte[] arr_two = new byte[2];
                byte[] arr_five = new byte[5];
                long[] int_mas = new long[0];

                double[] sales_tax = new double[6];
                double[] sales_types = new double[4];
                double[] payment_tax = new double[6];
                double[] payment_types = new double[4];

                arr_two[0] = OutputData[0];
                arr_two[1] = OutputData[1];
                rezDayInfo[0] = Methods.GetNumberFromBCD(arr_two);

                for (int i = 0, j = 0, k = 0; i < sales_tax.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = OutputData[2 + k];
                    sales_tax[i] = ((double)Methods.GetNumber(arr_five) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < sales_types.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = OutputData[2 + (6 * 5) + k];
                    sales_types[i] = ((double)Methods.GetNumber(arr_five) / 100);
                }

                rezDayInfo[1] = new object[] { sales_tax, sales_types };

                arr_five[0] = OutputData[52];
                arr_five[1] = OutputData[53];
                arr_five[2] = OutputData[54];
                arr_five[3] = OutputData[55];
                arr_five[4] = OutputData[56];
                rezDayInfo[2] = ((double)Methods.GetNumber(arr_five) / 100);
                arr_five[0] = OutputData[57];
                arr_five[1] = OutputData[58];
                arr_five[2] = OutputData[59];
                arr_five[3] = OutputData[60];
                arr_five[4] = OutputData[61];
                rezDayInfo[3] = ((double)Methods.GetNumber(arr_five) / 100);
                arr_five[0] = OutputData[62];
                arr_five[1] = OutputData[63];
                arr_five[2] = OutputData[64];
                arr_five[3] = OutputData[65];
                arr_five[4] = OutputData[66];
                rezDayInfo[4] = ((double)Methods.GetNumber(arr_five) / 100);
                arr_two[0] = OutputData[67];
                arr_two[1] = OutputData[68];
                rezDayInfo[5] = Methods.GetNumberFromBCD(arr_two);

                for (int i = 0, j = 0, k = 0; i < payment_tax.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = OutputData[2 + 5 * (6 + 4) + 5 + 5 + 5 + 2 + k];
                    payment_tax[i] = ((double)Methods.GetNumber(arr_five) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < payment_types.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = OutputData[2 + 5 * (6 + 4) + 5 + 5 + 5 + 2 + 6 * 5 + k];
                    payment_types[i] = ((double)Methods.GetNumber(arr_five) / 100);
                }

                rezDayInfo[6] = new object[] { payment_tax, payment_types };

                arr_five[0] = OutputData[119];
                arr_five[1] = OutputData[120];
                arr_five[2] = OutputData[121];
                arr_five[3] = OutputData[122];
                arr_five[4] = OutputData[123];
                rezDayInfo[7] = ((double)Methods.GetNumber(arr_five) / 100);
                arr_five[0] = OutputData[124];
                arr_five[1] = OutputData[125];
                arr_five[2] = OutputData[126];
                arr_five[3] = OutputData[127];
                arr_five[4] = OutputData[128];
                rezDayInfo[8] = ((double)Methods.GetNumber(arr_five) / 100);
                arr_five[0] = OutputData[129];
                arr_five[1] = OutputData[130];
                arr_five[2] = OutputData[131];
                arr_five[3] = OutputData[132];
                arr_five[4] = OutputData[133];
                rezDayInfo[9] = ((double)Methods.GetNumber(arr_five) / 100);
            }

            return rezDayInfo;
        }//ok
        private object[] GetCheckSums(ComPort port)
        {
            lastFuncName = "GetCheckSums";
            CMD = 43;
            //Creating data
            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] checkSumsInfo = new object[0];

            if (OutputData.Length == 40)
            {
                double[] tax_group = new double[6];
                double[] sum_group = new double[4];
                byte[] currTaxRotate = new byte[4];
                byte[] currSumRotate = new byte[4];

                for (int i = 0, j = 0, k = 0; i < tax_group.Length; i++)
                {
                    for (j = 0; j < currTaxRotate.Length; j++, k++)
                        currTaxRotate[j] = OutputData[k];

                    tax_group[i] = ((double)Methods.GetNumber(currTaxRotate) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < sum_group.Length; i++)
                {
                    for (j = 0; j < currTaxRotate.Length; j++, k++)
                        currSumRotate[j] = OutputData[4 * 6 + k];

                    sum_group[i] = ((double)Methods.GetNumber(currSumRotate) / 100);
                }

                checkSumsInfo = new object[2];
                checkSumsInfo[0] = tax_group;
                checkSumsInfo[1] = sum_group;
            }

            return checkSumsInfo;
        }//ok
        private object[] GetTaxRates(ComPort port)
        {
            lastFuncName = "GetTaxRates";
            CMD = 44;

            //Making data
            DataForSend = new byte[0];
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] taxInfoRezult = new object[0];

            if (OutputData.Length >= (1 + 3 + 0 + 1 + 0))
            {
                byte totTax = (byte)OutputData[0];

                int[] ans = Methods.GetArrayFromBCD(new byte[] { OutputData[1], OutputData[2], OutputData[3] });
                DateTime dt = new DateTime();
                try
                {
                    dt = new DateTime(ans[2] + 2000, ans[1], ans[0]);
                }
                catch { }

                byte[] tax = new byte[2 * totTax];
                Array.Copy(OutputData, 4, tax, 0, tax.Length);
                double[] taxRates = new double[totTax];
                byte[] currRate = new byte[2];
                for (int i = 0; i < tax.Length; i += 2)
                {
                    currRate[0] = tax[i];
                    currRate[1] = tax[i + 1];
                    taxRates[i / 2] = Methods.GetNumber(currRate) / 100;
                }

                byte status = (byte)OutputData[4 + 2 * totTax];
                byte decimals = (byte)(status & 0x0F);
                byte taxType = (byte)(status & 0x10);
                byte useGetTax = (byte)(status & 0x20);

                if (useGetTax == 0)
                {
                    taxInfoRezult = new object[6];
                    taxInfoRezult[0] = totTax;
                    taxInfoRezult[1] = dt;
                    taxInfoRezult[2] = taxRates;
                    taxInfoRezult[3] = decimals;
                    taxInfoRezult[4] = taxType;
                    taxInfoRezult[5] = false;

                }
                else
                {
                    Array.Copy(OutputData, 5 + 2 * totTax, tax, 0, tax.Length);
                    double[] gtaxRates = new double[totTax];
                    for (int i = 0; i < tax.Length; i += 2)
                    {
                        currRate[0] = tax[i];
                        currRate[1] = tax[i + 1];
                        gtaxRates[i / 2] = Methods.GetNumber(currRate) / 100;
                    }

                    taxInfoRezult = new object[7];
                    taxInfoRezult[0] = totTax;
                    taxInfoRezult[1] = dt;
                    taxInfoRezult[2] = taxRates;
                    taxInfoRezult[3] = decimals;
                    taxInfoRezult[4] = taxType;
                    taxInfoRezult[5] = true;
                    taxInfoRezult[6] = gtaxRates;
                }

            }
            return taxInfoRezult;
        }//ok
        private void CplCutter(ComPort port)
        {
            lastFuncName = "CplCutter";
            CMD = 46;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        #endregion
        #region Function of Fiscalization
        private void Fiscalization(ComPort port, ushort progPass, string fn)
        {
            lastFuncName = "Fiscalization";
            CMD = 21;

            //Creating data
            byte[] pass = Methods.GetByteArray(progPass, 2);
            DataForSend = new byte[2 + 10];
            DataForSend[0] = pass[0];
            DataForSend[1] = pass[1];
            Encoding.GetEncoding(866).GetBytes(fn).CopyTo(DataForSend, 2);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetHeadLine(ComPort port, ushort progPass, string line1, string line2, string line3, string line4)
        {
            lastFuncName = "SetHeadLine";
            CMD = 25;

            //Creating data
            DataForSend = new byte[2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length + 1 + line4.Length];

            byte[] pass = Methods.GetByteArray(progPass, 2);

            DataForSend[0] = pass[0];
            DataForSend[1] = pass[1];
            DataForSend[2] = (byte)line1.Length;
            Encoding.GetEncoding(866).GetBytes(line1).CopyTo(DataForSend, 3);
            DataForSend[2 + 1 + line1.Length] = (byte)line2.Length;
            Encoding.GetEncoding(866).GetBytes(line2).CopyTo(DataForSend, 2 + 1 + line1.Length + 1);
            DataForSend[2 + 1 + line1.Length + 1 + line2.Length] = (byte)line3.Length;
            Encoding.GetEncoding(866).GetBytes(line3).CopyTo(DataForSend, 2 + 1 + line1.Length + 1 + line2.Length + 1);
            DataForSend[2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length] = (byte)line4.Length;
            Encoding.GetEncoding(866).GetBytes(line4).CopyTo(DataForSend, 2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length + 1);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetTaxRate(ComPort port, ushort progPass, byte totTax, uint[] tax, byte status, byte totGTax, uint[] gtax)
        {
            lastFuncName = "SetTaxRate";
            CMD = 25;

            //Creating data
            DataForSend = new byte[2 + 1 + 2 * totTax + 1 + 2 * totGTax];

            byte[] pass = Methods.GetByteArray(progPass, 2);
            DataForSend[0] = pass[0];
            DataForSend[1] = pass[1];
            DataForSend[2] = totTax;

            byte[] currTRate = new byte[2];
            for (int i = 0; i < 2 * totTax; i += 2)
            {
                currTRate = Methods.GetByteArray(tax[i / 2], 2);
                DataForSend[3 + i] = currTRate[0];
                DataForSend[3 + (i + 1)] = currTRate[1];
            }

            DataForSend[3 + 2 * totTax] = status;

            for (int i = 0; i < 2 * totGTax; i += 2)
            {
                currTRate = Methods.GetByteArray(gtax[i / 2], 2);
                DataForSend[3 + 2 * totTax + 1 + i] = currTRate[0];
                DataForSend[3 + 2 * totTax + 1 + (i + 1)] = currTRate[1];
            }

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ProgArt(ComPort port, ushort progPass, byte doseDecimal, double price, char pdv, string name, uint id)
        {
            lastFuncName = "ProgArt";
            CMD = 34;

            //Creating data
            byte[] binArr = null;
            byte oneByte = 0x00;

            DataForSend = new byte[2 + 1 + 4 + 1 + 1 + name.Length + 2];

            binArr = Methods.GetByteArray(progPass, 2);
            DataForSend[0] = binArr[0];
            DataForSend[1] = binArr[1];
            DataForSend[2] = doseDecimal;
            //Price
            binArr = Methods.GetByteArray((int)Math.Round(Math.Abs(price) * 100), 4);
            DataForSend[3] = binArr[0];
            DataForSend[4] = binArr[1];
            DataForSend[5] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            DataForSend[6] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case '�': { oneByte = 0x80; break; }
                case '�': { oneByte = 0x81; break; }
                case '�': { oneByte = 0x82; break; }
                case '�': { oneByte = 0x83; break; }
                case '�': { oneByte = 0x84; break; }
                case '�': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            DataForSend[7] = oneByte;
            //Art Name length
            DataForSend[8] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(DataForSend, 9);
            //Art id
            binArr = Methods.GetByteArray(id, 2);
            DataForSend[DataForSend.Length - 2] = binArr[0];
            DataForSend[DataForSend.Length - 1] = binArr[1];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void LoadBMP(ComPort port, ushort progPass, bool allow, string fpath)
        {
            lastFuncName = "LoadBMP";
            CMD = 45;

            byte[] binArr = null;
            byte[] pass = Methods.GetByteArray((uint)progPass, 2);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fpath);
            byte[] img = new byte[bmp.Width / 8 * bmp.Height];
            byte oneByte = 0x00;
            int k = 0;
            int wo = 0;

            for (int h = 0, w = 0; h < bmp.Height; h++)
                for (w = 0; w < (bmp.Width / 8 * 8); w += 8)
                {
                    for (wo = 0; wo < 8; wo++)
                    {
                        oneByte <<= 1;
                        if (bmp.GetPixel(w + wo, h).Name == "ff000000")
                            oneByte |= 1;
                    }

                    img[k] = oneByte;
                    k++;
                }
            k = img.Length / 64;
            if (img.Length % 64 != 0)
                k++;
            object[] imgBlocks = new object[k];
            long idx = 0;
            byte cs = 0;
            for (k = 0; k < imgBlocks.Length; k++)
            {
                if (idx + 64 < img.Length)
                    binArr = new byte[64];
                else
                    binArr = new byte[img.Length - idx];

                Array.Copy(img, idx, binArr, 0, binArr.Length);
                cs = (byte)(0 - Methods.SumMas(binArr));
                Array.Resize<byte>(ref binArr, binArr.Length + 1);
                binArr[binArr.Length - 1] = cs;
                imgBlocks[k] = binArr;
                idx += 64;
            }
            //Creating data
            DataForSend = new byte[2 + 1 + 2 + 2];
            DataForSend[0] = pass[0];
            DataForSend[1] = pass[1];

            if (allow)
                DataForSend[2] = (byte)1;
            else
                DataForSend[2] = (byte)0;

            binArr = Methods.GetByteArray(bmp.Width / 8 * 8, 2);
            DataForSend[3] = binArr[0];
            DataForSend[4] = binArr[1];

            binArr = Methods.GetByteArray(bmp.Height, 2);
            DataForSend[5] = binArr[0];
            DataForSend[6] = binArr[1];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data

            if (!port.IsOpen)
                port.Open();

            byte[] buffer = new byte[1];
            uint rb = 20;
            int totRd = 20;
            bool enqDetected = false;
            port.Write(InputData);
            for (k = 0; k < imgBlocks.Length; k++)
            {
                totRd = 20;
                enqDetected = false;
                buffer = new byte[1];
                while (true)
                {
                    if (totRd < 0)
                        break;
                    port.Read(ref buffer, out rb);
                    if (buffer[0] == ENQ)
                    {
                        port.PortClear();
                        enqDetected = true;
                        break;
                    }
                    if (buffer[0] == SYN)
                        continue;
                    totRd--;
                }

                if (!enqDetected)
                    break;

                port.Write((byte[])imgBlocks[k]);
            }
            port.Close();

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Function of reports
        private void ArtReport(ComPort port, uint reportPass)
        {
            lastFuncName = "ArtReport";
            CMD = 7;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ArtXReport(ComPort port, uint reportPass)
        {
            lastFuncName = "ArtXReport";
            CMD = 10;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayReport(ComPort port, uint reportPass)
        {
            lastFuncName = "DayReport";
            CMD = 9;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayClrReport(ComPort port, uint reportPass)
        {
            lastFuncName = "DayClrReport";
            CMD = 13;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            SaveArtID(0);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport(ComPort port, uint reportPass, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "PeriodicReport";
            CMD = 17;

            //Creating data
            DataForSend = new byte[2 + 3 + 3];
            byte[] p = Methods.GetByteArray(reportPass, 2);
            DataForSend[0] = p[0];
            DataForSend[1] = p[1];
            byte[] date = Methods.GetBCDFromArray(new int[] { startDate.Day, startDate.Month, int.Parse(startDate.Year.ToString().Substring(2, 2)) });
            //BCD sdd = new BCD(startDate.Day);
            //BCD sdm = new BCD(startDate.Month);
            //BCD sdy = new BCD(int.Parse(startDate.Year.ToString().Substring(2, 2)));
            //Data[2] = sdd.CompressedBCD[0];
            //Data[3] = sdm.CompressedBCD[0];
            //Data[4] = sdy.CompressedBCD[0];
            DataForSend[2] = date[0];
            DataForSend[3] = date[1];
            DataForSend[4] = date[2];
            //BCD edd = new BCD(endDate.Day);
            //BCD edm = new BCD(endDate.Month);
            //BCD edy = new BCD(int.Parse(endDate.Year.ToString().Substring(2, 2)));
            date = Methods.GetBCDFromArray(new int[] { endDate.Day, endDate.Month, int.Parse(endDate.Year.ToString().Substring(2, 2)) });
            DataForSend[5] = date[0];
            DataForSend[6] = date[1];
            DataForSend[7] = date[2];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReportShort(ComPort port, uint reportPass, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "PeriodicReportShort";
            CMD = 26;

            //Creating data
            DataForSend = new byte[2 + 3 + 3];
            byte[] p = Methods.GetByteArray(reportPass, 2);
            DataForSend[0] = p[0];
            DataForSend[1] = p[1];
            byte[] date = Methods.GetBCDFromArray(new int[] { startDate.Day, startDate.Month, int.Parse(startDate.Year.ToString().Substring(2, 2)) });
            //BCD sdd = new BCD(startDate.Day);
            //BCD sdm = new BCD(startDate.Month);
            //BCD sdy = new BCD(int.Parse(startDate.Year.ToString().Substring(2, 2)));
            //Data[2] = sdd.CompressedBCD[0];
            //Data[3] = sdm.CompressedBCD[0];
            //Data[4] = sdy.CompressedBCD[0];
            DataForSend[2] = date[0];
            DataForSend[3] = date[1];
            DataForSend[4] = date[2];
            //BCD edd = new BCD(endDate.Day);
            //BCD edm = new BCD(endDate.Month);
            //BCD edy = new BCD(int.Parse(endDate.Year.ToString().Substring(2, 2)));
            date = Methods.GetBCDFromArray(new int[] { endDate.Day, endDate.Month, int.Parse(endDate.Year.ToString().Substring(2, 2)) });
            DataForSend[5] = date[0];
            DataForSend[6] = date[1];
            DataForSend[7] = date[2];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport2(ComPort port, uint reportPass, uint startID, uint endID)
        {
            lastFuncName = "PeriodicReport2";
            CMD = 31;

            //Creating data
            DataForSend = new byte[2 + 2 + 2];
            byte[] ps = Methods.GetByteArray(reportPass, 2);
            DataForSend[0] = ps[0];
            DataForSend[1] = ps[1];
            byte[] sid = Methods.GetByteArray(startID, 2);
            DataForSend[2] = sid[0];
            DataForSend[3] = sid[1];
            byte[] eid = Methods.GetByteArray(endID, 2);
            DataForSend[4] = eid[0];
            DataForSend[5] = eid[1];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        #endregion
        */
        #endregion
        #region DayReport
        private object[] ReportXZ(ComPort port, string pwd, byte type, bool[] clearModes)
        {
            lastFuncName = "ReportXZ";
            CMD = 69;

            // Creating data
            string _data = pwd + ',' + type;
            if (clearModes[0])
                _data += 'N';
            if (clearModes[1])
                _data += 'A';

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Creating data
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(port, 20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _xzinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _xzData = new object[6];
            if (_xzinfo.Length != 0)
            {
                string[] _zxItems = _xzinfo.Split(new char[1] { ',' });
                _xzData[0] = uint.Parse(_zxItems[0]);
                _xzData[1] = double.Parse(_zxItems[1], NumberFormat) / 100;
                _xzData[2] = double.Parse(_zxItems[2], NumberFormat) / 100;
                _xzData[3] = double.Parse(_zxItems[3], NumberFormat) / 100;
                _xzData[4] = double.Parse(_zxItems[4], NumberFormat) / 100;
                _xzData[5] = double.Parse(_zxItems[5], NumberFormat) / 100;
            }

            return _xzData;
        }
        #endregion
        #region Reports
        private void ReportByTax(ComPort port, string pwd, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "ReportByTax";
            CMD = 50;

            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByNoFull(ComPort port, string pwd, uint startZNo, uint endZNo)
        {
            lastFuncName = "ReportByNoFull";
            CMD = 73;

            string _data = pwd + ',';
            _data += string.Format("{0:0000}", startZNo) + ',';
            _data += string.Format("{0:0000}", endZNo);
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByDateFull(ComPort port, string pwd, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "ReportByDateFull";
            CMD = 94;

            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByDateShort(ComPort port, string pwd, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "ReportByDateShort";
            CMD = 79;

            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByNoShort(ComPort port, string pwd, uint startZNo, uint endZNo)
        {
            lastFuncName = "ReportByNoShort";
            CMD = 95;

            string _data = pwd + ',';
            _data += string.Format("{0:0000}", startZNo) + ',';
            _data += string.Format("{0:0000}", endZNo);
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByUsers(ComPort port, string pwd)
        {
            lastFuncName = "ReportUsers";
            CMD = 105;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(pwd);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ReportByArts(ComPort port, string pwd, string mode)
        {
            lastFuncName = "ReportArts";
            CMD = 111;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(pwd + ',' + mode);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Selling
        private void PrintCopy(ComPort port, byte copies)
        {
            lastFuncName = "PrintCopy";
            CMD = 109;

            //Creating data
            if (copies > 1)
            {
                DataForSend = new byte[1];
                DataForSend[0] = (byte)(copies + 0x30);
            }
            else
                DataForSend = new byte[0];
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ResetOrder(ComPort port)
        {
            lastFuncName = "ResetOrder";
            CMD = 57;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void CloseFOrder(ComPort port)
        {
            lastFuncName = "CloseFOrder";
            CMD = 45;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            //
        }
        private ushort[] CloseNOrder(ComPort port)
        {
            lastFuncName = "CloseNOrder";
            CMD = 39;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _noinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _noinf = new ushort[4];
            if (_noinfo.Length != 0)
            {
                string[] _noItems = _noinfo.Split(new char[1] { ',' });
                _noinf[0] = ushort.Parse(_noItems[0]);
                _noinf[1] = ushort.Parse(_noItems[1]);
                _noinf[2] = ushort.Parse(_noItems[2]);
                if (_noItems.Length == 4)
                    _noinf[3] = ushort.Parse(_noItems[3]);
            }

            return _noinf;
        }
        private ushort[] OpenNOrder(ComPort port)
        {
            lastFuncName = "OpenNOrder";
            CMD = 38;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _noinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _noinf = new ushort[4];
            if (_noinfo.Length != 0)
            {
                string[] _noItems = _noinfo.Split(new char[1] { ',' });
                _noinf[0] = ushort.Parse(_noItems[0]);
                _noinf[1] = ushort.Parse(_noItems[1]);
                _noinf[2] = ushort.Parse(_noItems[2]);
                if (_noItems.Length == 4)
                    _noinf[3] = ushort.Parse(_noItems[3]);
            }
            return _noinf;
        }
        private void PrintFText(ComPort port, string ftext)
        {
            lastFuncName = "PrintFText";
            CMD = 54;

            //Creating data
            ftext = ftext.Replace('�', 'i').Replace('�', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ftext);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void PrintNText(ComPort port, string ntext)
        {
            lastFuncName = "PrintNText";
            CMD = 42;

            //Creating data
            ntext = ntext.Replace('�', 'i').Replace('�', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ntext);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }


        private double[] SubTotal(ComPort port, bool print, bool indicate, double disc, bool discmode)
        {
            return new double[0];

        }

        private object[] Total(ComPort port, string text, char pmode, double money)
        {
            return new object[0];
        }

        private ushort[] OpenFOrder(ComPort port, byte uid, string pwd, byte cdno, bool extended)
        {


            return new ushort[0];

        }

        private void SaleArtIndicate(ComPort port, char sign, uint artno, uint tot, double nprice, double disc, bool discmode)
        {

        }

        private void SaleArt(ComPort port, char sign, uint artno, uint tot, double nprice, double disc, bool discmode)
        {

        }

        #endregion
        #region Initialization
        private void SetDateTime(ComPort port, DateTime dt)
        {
            lastFuncName = "SetDateTime";
            CMD = 61;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(dt.ToString("dd-MM-yy HH:mm:ss"));

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private object[] SetTaxRate(ComPort port, string pwd, byte dpoint, bool[] userates, double[] rates)
        {
            lastFuncName = "SetTaxRate";
            CMD = 83;

            //Creating data
            string _data = pwd + ',' + dpoint.ToString() + ',';
            foreach (bool a in userates)
                _data += a ? '1' : '0';
            foreach (double r in rates)
                _data += ',' + r.ToString(NumberFormat);

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _taxdata = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _taxInfo = new object[9];
            if (_taxdata.Length != 0)
            {
                string[] _trItems = _taxdata.Split(new char[] { ',' });
                _taxInfo[0] = byte.Parse(_trItems[0]);
                _taxInfo[1] = _trItems[1][0] == '1' ? true : false;
                _taxInfo[2] = _trItems[1][1] == '1' ? true : false; ;
                _taxInfo[3] = _trItems[1][2] == '1' ? true : false; ;
                _taxInfo[4] = _trItems[1][3] == '1' ? true : false; ;

                _taxInfo[5] = double.Parse(_trItems[2], NumberFormat);
                _taxInfo[6] = double.Parse(_trItems[3], NumberFormat);
                _taxInfo[7] = double.Parse(_trItems[4], NumberFormat);
                _taxInfo[8] = double.Parse(_trItems[5], NumberFormat);
            }

            return _taxInfo;
        }
        private void SetSaleMode(ComPort port, byte mode)
        {
            lastFuncName = "SetSaleMode";
            CMD = 84;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(mode.ToString());

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }

        private char SetSerialNum(ComPort port, byte ccode, string snum)
        {
            lastFuncName = "SetSerialNum";
            CMD = 91;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ccode + ',' + snum);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }
        private char SetFixNum(ComPort port, string fnum)
        {
            lastFuncName = "SetFixNum";
            CMD = 92;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(fnum);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }

        private char SetTaxNum(ComPort port, string tnum, char ttype)
        {
            lastFuncName = "SetTaxNum";
            CMD = 98;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(tnum + ',' + ttype);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }

        private void SetUserPass(ComPort port, string opwd, string npwd, byte uid)
        {
            lastFuncName = "SetUserPwd";
            CMD = 101;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + opwd + "," + npwd);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetUserName(ComPort port, string pwd, byte uid, string name)
        {
            lastFuncName = "SetUserName";
            CMD = 102;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd + "," + name);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ResetUserData(ComPort port, string pwd, byte uid)
        {
            lastFuncName = "ResetUserData";
            CMD = 104;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private string[] SetGetArticle(ComPort port, char option, object[] param)
        {
            lastFuncName = "SetGetArticle";
            CMD = 107;

            //Creating data
            string _data = option.ToString();
            if (!Methods.IsEmpty(param))
                for (int i = 0; i < param.Length; i++)
                    _data += param[i].ToString();

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            //Perform processing recived data
            string _art = Encoding.GetEncoding(1251).GetString(OutputData);
            string[] artInfo = new string[0];

            if (_art.Length != 0)
                artInfo = _art.Split(new char[1] { ',' });

            return artInfo;
        }
        private void LoadLogo(ComPort port, string pwd, byte[][] logo)
        {
            lastFuncName = "LoadLogo";
            CMD = 115;

            byte _wCorr = 0;
            if (logo[0].Length < 54)
                _wCorr += (byte)(54 - logo[0].Length);
            byte[] _predata = new byte[0];
            for (int _r = 0; _r < logo.Length; _r++)
            {
                //Creating data
                _predata = Encoding.GetEncoding(1251).GetBytes(pwd + ',' + _r + ',');

                DataForSend = new byte[_predata.Length + logo[_r].Length + _wCorr];
                Array.Copy(_predata, 0, DataForSend, 0, _predata.Length);
                Array.Copy(logo[_r], 0, DataForSend, _predata.Length, logo[_r].Length);

                //Preparing data
                InputData = CreateInputData(CMD, DataForSend);

                //Sending and reciving data
                SendGetData(port, 20, true);

                //Next code for command
                GetNextCmdCode();
            }
        }
        private void SetAdminPass(ComPort port, string opwd, string npwd)
        {
            lastFuncName = "SetAdminPass";
            CMD = 118;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(opwd + "," + npwd);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ResetUserPass(ComPort port, string pwd, byte uid)
        {
            lastFuncName = "ResetUserPass";
            CMD = 119;

            //Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Get info
        private DateTime GetDateTime(ComPort port)
        {
            lastFuncName = "GetDateTime";
            CMD = 62;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _dtime = Encoding.GetEncoding(1251).GetString(OutputData);
            return DateTime.Parse(_dtime);
        }
        private object[] GetLastZReport(ComPort port, byte type)
        {
            lastFuncName = "GetLastZReport";
            CMD = 64;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(type.ToString());

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _zrep = new object[6];
            string _zrepInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_zrepInfo.Length != 0)
            {
                string[] _zrepItems = _zrepInfo.Split(new char[] { ',' });
                _zrep[0] = uint.Parse(_zrepItems[0]);
                _zrep[1] = double.Parse(_zrepItems[1], NumberFormat);
                _zrep[2] = double.Parse(_zrepItems[2], NumberFormat);
                _zrep[3] = double.Parse(_zrepItems[3], NumberFormat);
                _zrep[4] = double.Parse(_zrepItems[4], NumberFormat);
                _zrep[5] = double.Parse(_zrepItems[5], NumberFormat);
            }

            return _zrep;
        }
        private double[] GetSummsByDay(ComPort port, byte type)
        {
            lastFuncName = "GetSummsByDay";
            CMD = 65;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(type.ToString());

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            double[] _summ = new double[6];
            string _summInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_summInfo.Length != 0)
            {
                string[] _summItems = _summInfo.Split(new char[] { ',' });
                _summ[0] = double.Parse(_summItems[0], NumberFormat) / 100;
                _summ[1] = double.Parse(_summItems[1], NumberFormat) / 100;
                _summ[2] = double.Parse(_summItems[2], NumberFormat) / 100;
                _summ[3] = double.Parse(_summItems[3], NumberFormat) / 100;
                _summ[4] = double.Parse(_summItems[4], NumberFormat) / 100;
            }

            return _summ;
        }
        private object[] GetSumCorByDay(ComPort port)
        {
            lastFuncName = "GetSumCorByDay";
            CMD = 67;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _scorr = new object[6];
            string _corrdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_corrdata.Length != 0)
            {
                string[] _corrItems = _corrdata.Split(new char[1] { ',' });
                _scorr[0] = double.Parse(_corrItems[0]) / 100;
                _scorr[1] = double.Parse(_corrItems[1]) / 100;
                _scorr[2] = double.Parse(_corrItems[2]) / 100;
                _scorr[3] = ushort.Parse(_corrItems[3]);
                _scorr[4] = ushort.Parse(_corrItems[4]);
                _scorr[5] = ushort.Parse(_corrItems[5]);
            }

            return _scorr;
        }
        private uint GetFreeMem(ComPort port)
        {
            lastFuncName = "GetFreeMem";
            CMD = 68;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return uint.Parse(_fmem);
        }
        private byte[] GetState(ComPort port)
        {
            lastFuncName = "GetState";
            CMD = 74;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            byte[] _stat = new byte[48];
            byte _sIdx = 0;
            byte _lIdx = 0;
            byte _mask = 0x80;
            for (int _i = 0, _j = 0; _i < OutputData.Length; _i++)
            {
                for (_j = 0; _j < 8; _j++)
                {
                    if ((OutputData[_i] & _mask) != 0)
                    {
                        _stat[_lIdx] = _sIdx;
                        _lIdx++;
                    }
                    _mask >>= 1;
                    _sIdx++;
                }
                _mask = 0x80;
            }

            return _stat;
        }
        private object[] GetFixTransState(ComPort port, char param)
        {
            lastFuncName = "GetFixTransState";
            CMD = 76;

            if (param != 0)
            {
                DataForSend = new byte[1];
                DataForSend[0] = (byte)param;
            }
            else
                DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _trans = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _tInfo = new object[5];
            if (_trans.Length != 0)
            {
                string[] _tItems = _trans.Split(new char[1] { ',' });
                _tInfo[0] = byte.Parse(_tItems[0]);
                _tInfo[1] = ushort.Parse(_tItems[1]);
                _tInfo[2] = uint.Parse(_tItems[2]) / (double)100;
                if (_tItems.Length == 4)
                    _tInfo[3] = uint.Parse(_tItems[3]) / (double)100;
                if (_tItems.Length == 5)
                    _tInfo[4] = byte.Parse(_tItems[4]);
            }

            return _tInfo;
        }
        private string[] GetDiagInfo(ComPort port)
        {
            lastFuncName = "GetDiagInfo";
            CMD = 90;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _dinfo = Encoding.GetEncoding(1251).GetString(OutputData);

            return _dinfo.Split(new char[2] { ',', ' ' });
        }
        private double[] GetTaxRates(ComPort port)
        {
            lastFuncName = "GetTaxRates";
            CMD = 97;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            double[] _taxrates = new double[4];
            string _taxs = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_taxs.Length != 0)
            {
                string[] _taxItem = _taxs.Split(new char[1] { ',' });
                _taxrates[0] = double.Parse(_taxItem[0], NumberFormat); //A
                _taxrates[1] = double.Parse(_taxItem[1], NumberFormat); //B
                _taxrates[2] = double.Parse(_taxItem[2], NumberFormat); //V
                _taxrates[3] = double.Parse(_taxItem[3], NumberFormat); //G
            }

            return _taxrates;
        }
        private string[] GetTaxID(ComPort port)
        {
            lastFuncName = "GetTaxID";
            CMD = 99;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _taxid = Encoding.GetEncoding(1251).GetString(OutputData);
            return _taxid.Split(new char[1] { ',' });
        }
        private object[] GetChqInfo(ComPort port)
        {
            lastFuncName = "GetChqInfo";
            CMD = 103;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _chqinfo = new object[7];
            string _cinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_cinfo.Length != 0)
            {
                string[] _infoItem = _cinfo.Split(new char[1] { ',' });
                _chqinfo[0] = byte.Parse(_infoItem[0]);
                for (int _i = 1; _i < _infoItem.Length - 1; _i++)
                    _chqinfo[_i] = double.Parse(_infoItem[_i], NumberFormat) / 100;
                _chqinfo[6] = byte.Parse(_infoItem[6]);
            }

            return _chqinfo;
        }
        private object[] GetPaymentInfo(ComPort port)
        {
            lastFuncName = "GetPaymentInfo";
            CMD = 110;

            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _payment = new object[7];
            string _paymentInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_paymentInfo.Length != 0)
            {
                string[] _paymentItems = _paymentInfo.Split(new char[1] { ',' });
                _payment[0] = double.Parse(_paymentItems[0], NumberFormat);
                _payment[1] = double.Parse(_paymentItems[1], NumberFormat);
                _payment[2] = double.Parse(_paymentItems[2], NumberFormat);
                _payment[3] = double.Parse(_paymentItems[3], NumberFormat);
                _payment[4] = uint.Parse(_paymentItems[_paymentItems.Length - 3]);
                _payment[5] = uint.Parse(_paymentItems[_paymentItems.Length - 2]);
                _payment[6] = uint.Parse(_paymentItems[_paymentItems.Length - 1]);
            }

            return _payment;
        }
        private object[] GetUserInfo(ComPort port, byte uid)
        {
            lastFuncName = "GetUserInfo";
            CMD = 112;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid.ToString());

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _user = new object[12];
            string _userInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_userInfo.Length != 0)
            {//0, 0;0.00, 0;0.00, 0;0.00, 0;0.00, 0;0.00, gfdgfdgfd 
                string[] _userItems = _userInfo.Split(new char[2] { ',', ';' });
                _user[0] = uint.Parse(_userItems[0]);
                _user[1] = uint.Parse(_userItems[1]);
                _user[2] = double.Parse(_userItems[2], NumberFormat);
                _user[3] = uint.Parse(_userItems[3]);
                _user[4] = double.Parse(_userItems[4], NumberFormat);
                _user[5] = uint.Parse(_userItems[5]);
                _user[6] = double.Parse(_userItems[6], NumberFormat);
                _user[7] = uint.Parse(_userItems[7]);
                _user[8] = double.Parse(_userItems[8], NumberFormat);
                _user[9] = uint.Parse(_userItems[9]);
                _user[10] = double.Parse(_userItems[10], NumberFormat);
                _user[11] = _userItems[11];
            }

            return _user;
        }
        private uint GetLastFChqNo(ComPort port)
        {
            lastFuncName = "GetLastChqNo";
            CMD = 113;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _chqno = Encoding.GetEncoding(1251).GetString(OutputData);
            return uint.Parse(_chqno);
        }
        private object[] GetFixMem(ComPort port, string fno, byte type, string sparam)
        {
            lastFuncName = "GetFixMem";
            CMD = 114;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(fno + ',' + type + ((sparam.Length != 0) ? (',' + sparam) : ""));

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return new object[0];
        }
        private object[] GetRemTime(ComPort port)
        {
            lastFuncName = "GetRemTime";
            CMD = 46;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return new object[0];
        }
        #endregion
        #region Printer commads
        private void CutChq(ComPort port)
        {
            lastFuncName = "CutChq";
            CMD = 45;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void LineFeed(ComPort port, byte lines)
        {
            lastFuncName = "LineFeed";
            CMD = 44;

            DataForSend = Encoding.GetEncoding(1251).GetBytes(lines.ToString());

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Display
        private void ClrDispl(ComPort port)
        {
            lastFuncName = "ClrDispl";
            CMD = 33;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void DisplBotLine(ComPort port, string line)
        {
            lastFuncName = "DisplBotLine";
            CMD = 35;

            //Creating data
            line = line.Replace('�', 'i').Replace('�', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(line);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void DisplTopLine(ComPort port, string line)
        {
            lastFuncName = "DisplTopLine";
            CMD = 47;

            //Creating data
            line = line.Replace('�', 'i').Replace('�', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(line);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void DisplayDateTime(ComPort port)
        {
            lastFuncName = "DisplayDateTime";
            CMD = 63;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void DisplText(ComPort port, string text)
        {
            lastFuncName = "DisplText";
            CMD = 100;

            //Creating data
            text = text.Replace('�', 'i').Replace('�', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(text);

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Other
        private double[] SetGetMoney(ComPort port, double amount)
        {
            lastFuncName = "SetGetMoney";
            CMD = 70;

            string _amount = amount.ToString(NumberFormat);
            if (amount > 0)
                _amount = "+" + _amount;
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_amount);

            //Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            //
            string _rdata = Encoding.GetEncoding(1251).GetString(OutputData);
            double[] cashInfo = new double[3];
            if (_rdata.Length != 0)
            {
                string[] _explStr = _rdata.Split(new char[] { ',' });
                if (_explStr[0] != "F")
                {
                    cashInfo[0] = double.Parse(_explStr[0]) / 100;
                    cashInfo[1] = double.Parse(_explStr[1]) / 100;
                    cashInfo[2] = double.Parse(_explStr[2]) / 100;
                }
            }

            return cashInfo;
        }
        private void PrintDiagInfo(ComPort port)
        {
            lastFuncName = "PrintDiagInfo";
            CMD = 71;

            //Preparing data
            InputData = CreateInputData(CMD, new byte[0]);

            //Sending and reciving data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void OpenBox(ComPort port, byte impulse)
        {
            lastFuncName = "OpenBox";
            CMD = 106;

            if (impulse != 0)
                DataForSend = Encoding.GetEncoding(1251).GetBytes(impulse.ToString());
            else
                DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(CMD, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void Beep(ComPort port)
        {
            lastFuncName = "Beep";
            CMD = 80;

            //Making data
            InputData = CreateInputData(CMD, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #endregion

        // Driver's Application Interface (Access Methods)
        // Need for access to FP from main program
        private void FP_Sale(ComPort port, object[] param)
        {
            // Return if parameters are empty
            if (Methods.IsEmpty(param))
                return;

            // Check last executed function for error
            if (ErrorFlags["FP_Sale"])
                ResetOrder(port);

            // Local varibles
            byte operNo = (byte)param[0];
            string operPwd = param[1].ToString();
            byte pdNo = (byte)param[2];
            System.Data.DataTable dTable = (System.Data.DataTable)param[3];
            byte ppt = Convert.ToByte(param[4]);
            bool useTotDisc = (bool)param[5];
            object[] article = new object[5];
            uint nexArticleNo = 0;

            // Get last article iduint nexArticleNo = ;
            

            // Check free memory for current sale
            if (nexArticleNo + dTable.Rows.Count <= 14800)
            {
                System.Windows.Forms.MessageBox.Show("���� ����� ���'�� ��� ��������� �������\r\n�������� ������� Z-��� ��� ���������� �������",
                    Protocol_Name, System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
                return;
            }

            // Open fiscal order
            OpenFOrder(port, operNo, operPwd, pdNo, true);


            // Program and sale each articles
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                article[0] = Methods.GetDouble(dTable.Rows[i]["TOT"]);
                article[1] = ppt;
                article[2] = Methods.GetDouble(dTable.Rows[i]["PRICE"]);
                article[3] = dTable.Rows[i]["VG"];
                article[4] = dTable.Rows[i]["NAME"].ToString().Replace('�', 'i').Replace('�', 'I');
                nexArticleNo++;

                //SaleArt(port, '+', nexArticleNo, article[0], article[1], article[2], article[0]);

                if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                    ;// FP_Discount(port, (byte)0, (double)dTable.Rows[i]["DISC"], ppt, "");
            }

        }
        private void FP_PayMoney(ComPort port, object[] param)
        {
            // Return if parameters are empty
            if (Methods.IsEmpty(param))
                return;

            // Check last executed function for error
            if (ErrorFlags["FP_PayMoney"])
                ResetOrder(port);
        }
        private void FP_Payment(ComPort port, object[] param)
        {
            // Return if parameters are empty
            if (Methods.IsEmpty(param))
                return;

            // Check last executed function for error
            if (ErrorFlags["FP_Payment"])
                ResetOrder(port);
        }
        private void FP_Discount(ComPort port) { }
        private uint FP_LastChqNo(ComPort port, object[] param)
        {
            object[] fOrdInfo = GetLastZReport(port, (byte)param[0]);

            return (uint)fOrdInfo[0];
        }//ok
        private uint FP_LastZRepNo(ComPort port, object[] param)
        {
            if (Methods.IsEmpty(param))
                return 0;

            byte retriveOrder = (byte)((bool)param[0] ? 1 : 0);
            object[] zRepInfo = GetLastZReport(port, retriveOrder);

            return (uint)zRepInfo[0];
        }//ok
        private void FP_OpenBox(ComPort port)
        {
            this.OpenBox(port, 0);
        }//ok
        private void FP_SetCashier(ComPort port, object[] param)
        {
            if (Methods.IsEmpty(param))
                return;

            try
            {
                byte _pdId = (byte)param[0];
                byte _userNo = (byte)param[0];
                uint _userPwd = (uint)param[0];
                string _userId = param[0].ToString();

                CustomData["DeskNo"] = _pdId;
                CustomData["UserNo"] = _userNo;
                CustomData["UserPwd"] = _userPwd;
            }
            catch { }
        }//ok
        private void FP_SendCustomer(ComPort port, object[] param)
        {
            if (Methods.IsEmpty(param))
                return;

            try
            {
                string[] _lines = (string[])param[0];
                bool[] _show = (bool[])param[1];

                if (_show[0])
                    this.DisplTopLine(port, _lines[0]);
                if (_show[1])
                    this.DisplBotLine(port, _lines[1]);
            }
            catch { }
        }//ok

        // Base Methods
        // Implementation of them can be different for other drivers
        private void GetNextCmdCode()
        {
            SEQ++;
        }
        private byte[] CreateInputData(byte _nom, byte[] param)
        {
            byte[] mas = new byte[4 + param.Length + 6];
            byte i = 0;
            byte[] bccData = new byte[] { 0x30, 0x30, 0x30, 0x30 };
            ushort bccItem = 0x0000;
            ushort bccItemMask = 0x000F;

            mas[0] = PRE;
            mas[1] = Convert.ToByte(0x20 + param.Length + 4);
            mas[2] = (byte)(SEQ + 0x20);
            mas[3] = CMD;

            for (i = 0; i < param.Length; i++)
                mas[i + 4] = param[i];

            mas[4 + param.Length] = POS;

            bccItem = (ushort)(mas[1] + mas[2] + _nom + Methods.UIntSumMas(param) + POS);
            for (i = 0; i < bccData.Length; i++)
                bccData[i] += (byte)((bccItem >> (i * 4)) & bccItemMask);

            mas[4 + param.Length + 1] = bccData[3];
            mas[4 + param.Length + 2] = bccData[2];
            mas[4 + param.Length + 3] = bccData[1];
            mas[4 + param.Length + 4] = bccData[0];
            mas[4 + param.Length + 4 + 1] = TER;

            return mas;
        }
        private bool SendGetData(ComPort port, int totRead, bool close)
        {
            string exceptionMsg = "";

            if (!port.IsOpen)
                try
                {
                    port.Open();
                }
                catch { }

            if (port.IsOpen && port.Write(InputData))
            {
                //WinAPI.OutputDebugString("W");

                int t = totRead;
                byte[] buffer = new byte[512];
                OutputData = new byte[0];
                do
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    //WinAPI.OutputDebugString("R");
                    if (port.Read(ref buffer, out ReadedBytes))
                    {
                        Array.Resize<byte>(ref OutputData, (int)(OutputData.Length + ReadedBytes));
                        Array.Copy(buffer, 0, OutputData, OutputData.Length - ReadedBytes, ReadedBytes);
                        try
                        {
                            //WinAPI.OutputDebugString("D");
                            switch (DecodeAnswer())
                            {
                                case "busy":
                                    {
                                        //WinAPI.OutputDebugString("S");
                                        Thread.Sleep(200);
                                        break;
                                    }
                                case "true":
                                    {
                                        //WinAPI.OutputDebugString("T");
                                        ErrorFlags[lastFuncName] = false;
                                        return true;
                                    }
                                case "false":
                                    {
                                        //WinAPI.OutputDebugString("F");
                                        t--;
                                        Thread.Sleep(50);
                                        break;
                                    }
                                case "failed":
                                    {
                                        throw new Exception("������� ��������� ������� \"" + lastFuncName + "\"");
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptionMsg = "\r\n" + ex.Message;
                            break;
                        }
                    }
                    else
                        break;

                } while (t > 0);
                //WinAPI.OutputDebugString("E");
            }

            if (close)
                port.Close();

            ErrorFlags[lastFuncName] = true;
            throw new Exception("������� ������� � ����������� ��������" + " " + Protocol_Name + exceptionMsg);
        }
        private string DecodeAnswer()
        {
            // if no recived data then set false state
            if (OutputData.Length == 0)
                return "false";
            // if message wsn't accepted then set failed state            
            if (OutputData[0] == NAK)
                return "failed";
            // remove all SYN symbols
            byte symbol = 0x00;
            byte[] normalizedAnswer = (byte[])(OutputData.Clone());
            bool preDetected = false;
            bool synDetected = false;
            for (byte i = 0; i < normalizedAnswer.Length; i++)
            {
                if (OutputData[i] == PRE && !preDetected) preDetected = true;
                if (OutputData[i] == SYN && !synDetected) synDetected = true;
                // if detected only SYN symbol
                if (!preDetected && synDetected)
                {
                    symbol++;
                    continue;
                }
                break;
            }
            // if not detected first char of message thet set busy state
            if (synDetected && !preDetected)
                return "busy";
            // remove all SYN symbols
            if (symbol != 0)
            {
                OutputData = new byte[normalizedAnswer.Length - symbol];
                Array.Copy(normalizedAnswer, symbol, OutputData, 0, OutputData.Length);
            }
            // if first char is message's char then translate this message
            if (OutputData[0] == PRE)
            {
                LEN = (byte)(OutputData[1] - 0x20);

                if (LEN + 6 > OutputData.Length)
                    return "busy";

                if (SEQ == (OutputData[2] - 0x20) && CMD == OutputData[3] && SEP == OutputData[1 + LEN - 1 - 6 - 1] && POS == OutputData[LEN])
                {
                    byte[] _data = new byte[LEN - 3 - 8];
                    byte[] _status = new byte[6];
                    byte _statusMask = 0x80;
                    byte _statusItem = 0x00;
                    ushort _resBcc = 0;
                    ushort _bcc = 0x0000;
                    byte midx = 0;
                    byte i = 0;
                    byte j = 0;

                    // get data
                    Array.Copy(OutputData, 4, _data, 0, _data.Length);
                    // get status bytes
                    Array.Copy(OutputData, LEN - 6, _status, 0, _status.Length);
                    // get recived checksum
                    for (i = 0, j = 3; i < 4; i++, j--)
                        _resBcc += (ushort)((OutputData[LEN + 1 + i] - 0x30) << j * 4);
                    // calculate checksum by recived message
                    _bcc = (ushort)(LEN + 0x20 + SEQ + 0x20 + CMD + Methods.UIntSumMas(_data) + SEP + Methods.UIntSumMas(_status) + POS);
                    // compare calculated and recived checksums
                    if (_bcc != _resBcc)
                        throw new Exception("����������� ��������� ���� ���������� �����������");
                    // get status messages
                    string oper_info = string.Empty;
                    for (i = 0, j = 0, midx = 0; i < _status.Length; i++)
                    {
                        _statusItem = _status[i];
                        _statusMask = 0x80;
                        for (j = 0; j < 8; j++)
                        {
                            if ((_statusItem & _statusMask) != 0 && ((string[])CustomData["Status"])[midx].Length != 0)
                                oper_info += ((string[])CustomData["Status"])[midx] + "\r\n";
                            _statusMask >>= 1;
                            midx++;
                        }
                        j = 0;
                    }

                    OutputData = _data;
                    return "true";
                }
            }//if PRE

            return "false";
        }
    }
}