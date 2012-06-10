using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;


namespace FPService.Protocol.IKSE260T
{
    sealed class IKSE260T : Protocol
    {
        //Service symbols
        private byte NOM = 0x00;
        private byte COD = 0x00;
        private byte DLE = 0x10;
        private byte STX = 0x02;
        private byte ETX = 0x03;
        private byte ACK = 0x06;
        private byte NAK = 0x15;
        private byte SYN = 0x16;
        private byte ENQ = 0x05;
        private byte CS;
        
        //Other
        private byte state = 0;
        private string[] stateMsg = new string[8]{
            "Принтер не готовий",
            "Нема паперу",
            "Помилка або переповнення фіскальної памяті",
            "Направильна дата або помилка годинника",
            "Помилка індикатора",
            "Перевищення тривалості зміни",
            "Зниження робочої напруги живлення",
            "Функція не існує або заборонена в даному режимі"
        };
        private byte rezult = 0;
        private string[] rezultMsg = new string[47]{
            "",
            "Помилка принтера",
            "Закінчився папір",
            "",
            "Збій фіскальної памяті",
            "",
            "Зниження напруги живлення",
            "",
            "Фіскальна память переповнена",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Команда заборонена в даному режимі",
            "",
            "",
            "Помилка програмування логотипа",
            "Неправильна довжина рядка",
            "Неправильний пароль",
            "Неіснуючий номер (пароля, рядка)",
            "Податкова група не існує або не встановлена",
            "Тип оплати не існує",
            "Недопустимі коди символів",
            "Перебільшення кількості податків або значення податкової ставки",
            "Від'ємний продаж більше суми попередніх продаж чека",
            "Помилка опису артикула. Обнуліть чек та зробіть Z-звіт",
            "",
            "Помилка формату дати/часу",
            "Перебільшення реєстрацій у чеці",
            "Перебільшення розрядності розрахованої вартості",
            "Переповнення регістра денного обігу",
            "Переповнення регістра оплат",
            "Видана сума більша, ніж у грошовій скринці",
            "Дата передує даті останнього Z-звіту",
            "Продажі заборонені (відкрито чек виплат)",
            "Виплати заборонені (відкрито чек продажу)",
            "Команда заборонена (чек не відкрито)",
            "",
            "Команда заборонена до Z-звіту",
            "Команда заборонена (не було чеків)",
            "Здача з цієї оплати заборонена",
            "Команда заборонена (чек відкрито)",
            "Знижки/націнки заборонені (не було продаж у відкритому чеці)",
            "Команда заборонена після початку оплат"
        };
        private byte reserv = 0;
        private string[] reservMsg = new string[5]{
            "Стан аварії",
            "Чек виплат",
            "Принтер зареєстрований",
            "Відкрита зміна",
            "Відкритий чек"
        };
       
        public IKSE260T()
        {
            Protocol_Name = "IKC-E260T";
            FP_Panel = new Tree(Protocol_Name);
            ProtocolPublicFunctions = ((Tree)FP_Panel).PublicFunctions;
            
        }

        internal override object[] CallFunction(string name, string description, ComPort port, object[] param)
        {
            object[] value = new object[0];
            OutputData = new byte[0];
            ReadedBytes = 0;

            if (description == string.Empty)
                description = ((Tree)FP_Panel).GetDescription(name);

            switch (name)
            {
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
                            System.Windows.Forms.MessageBox.Show("Неохідно зробити Z-звіт для наступного продажу", 
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

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                article[0] = Methods.GetDouble(dTable.Rows[i]["TOT"]);
                                article[2] = Methods.GetDouble(dTable.Rows[i]["PRICE"]);
                                article[3] = dTable.Rows[i]["VG"];
                                article[4] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
                                articleNewID++;
                                PayMoney(port, (double)article[0], dose_ppt, false, (double)article[2],
                                    article[3].ToString()[0], article[4].ToString(), articleNewID, money_ppt);
                                if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                                    Discount(port, (byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, "");
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
                        string _status = string.Format("{0} {1}", "Друк у нефіскальному режимі", (mem[0] & 0x08) != 0 ? "дозволений" : "не дозволений");
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
                            System.Windows.Forms.MessageBox.Show("Неохідно зробити Z-звіт для наступного продажу", 
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
                                    Sale(port, (double)article[0], (byte)article[1], s.dontPrintOne, 
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
                                article[4] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
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
                                        info += "Здача : ";
                                    else
                                        info += "Залишок : ";

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
                        string _status = string.Format("В сейфі : {0}", (double)(copecks / 100) + (double)(copecks % 100) / 100);
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
                        string _status = string.Format("{0} {1}", "Режим OnLine", (mem[0] & 0x40) != 0 ? "увімкнено" : "вимкнено");
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
                        string _status = string.Format("{0} {1}", "Видача суми на індикатор", (mem[0] & 0x80) != 0 ? "не активна" : "активна");
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
                            info += "Назва товару" + " : ";
                            info += artInfo[0].ToString() + "\r\n";
                            info += "Кількість" + " : ";
                            info += artInfo[1].ToString() + "\r\n";
                            info += "Ціна" + " : ";
                            info += artInfo[2].ToString() + "\r\n";
                            info += "Податкова група" + " : ";
                            info += artInfo[3].ToString() + "\r\n";
                            info += "Сума обігу" + " : ";
                            info += artInfo[4].ToString() + "\r\n";
                            info += "\r\n\r\n";
                            info += "Зворотня операція" + "\r\n";
                            info += "Кількість" + " : ";
                            info += artInfo[5].ToString() + "\r\n";
                            info += "Сума обігу" + " : ";
                            info += artInfo[6].ToString() + "\r\n";
                        }
                        else
                            info = "Інформація про то товар відсутня";

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
                            string[] payTypes = new string[] { "Картка", "Кредит", "Чек", "Готівка" };

                            double[] sales_group = (double[])((object[])value[1])[0];
                            double[] sales_forms = (double[])((object[])value[1])[1];

                            double[] pays_group = (double[])((object[])value[6])[0];
                            double[] pays_forms = (double[])((object[])value[6])[1];

                            dayRepInfoLine += "Лічильник чеків продаж : " + value[0];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Лічильник продаж по податковим групам і формам оплати";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < sales_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), sales_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < sales_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], sales_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна націнка по продажам : " + value[2];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна знижка по продажам : " + value[3];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна сума службового внесення : " + value[4];
                            dayRepInfoLine += "\r\n\r\n";
                            dayRepInfoLine += "Лічильник чеків виплат : " + value[5];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Лічильник виплат по податковим групам і формам оплати";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < pays_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), pays_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < pays_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], pays_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна націнка по виплатам : " + value[7];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна знижка по виплатам : " + value[8];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна сума службової видачі : " + value[9];

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
                            checkSumsInfoLine += "Лічильник обігів по податковим групам";
                            checkSumsInfoLine += "\r\n";
                            double[] tax = (double[])value[0];
                            for (i = 0; i < tax.Length; i++)
                                checkSumsInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), tax[i]);
                            checkSumsInfoLine += "\r\n";
                            double[] sum = (double[])value[1];
                            checkSumsInfoLine += "Лічильник сум сплат по формам оплат";
                            checkSumsInfoLine += "\r\n";
                            string[] payTypes = new string[] { "Картка", "Кредит", "Чек", "Готівка" };
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
                            taxInfoLine += "К-сть податкових ставок : " + taxData[0];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "Дата встановлення : " + ((DateTime)taxData[1]).ToLongDateString();
                            taxInfoLine += "\r\n\r\n";
                            taxInfoLine += "Ставки ПДВ";
                            taxInfoLine += "\r\n";
                            double[] tax = (double[])taxData[2];
                            for (int i = 0; i < tax.Length; i++)
                                taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'А') + i), tax[i]);
                            taxInfoLine += "\r\n";
                            taxInfoLine += "К-сть десяткових розрядів грошових сум : " + taxData[3];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "Тип ПДВ : " + ((taxData[4].ToString() == "0") ? "Включний" : "Додатній");
                            if ((bool)taxData[5])
                            {
                                taxInfoLine += "\r\n\r\n";
                                taxInfoLine += "Ставки зборів";
                                taxInfoLine += "\r\n";
                                tax = (double[])taxData[6];
                                for (int i = 0; i < tax.Length; i++)
                                    taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'А') + i), tax[i]);
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
                        string _status = string.Format("{0} {1}", "Обріжчик", (mem[0] & 0x08) == 0 ? "активний" : "не активний");
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
                                article[3] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
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

                case "FP_GetLastZNo":
                    {
                        value = new object[1] { FP_LastZRepNo(port) };
                        break;
                    }
            }
            return value;
        }

        //Driver
        #region Functions of registration
        private object[] SendStatus(ComPort port)
        {
            lastFuncName = "SendStatus";
            NOM = 0;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);
            
            //Next code for command
            GetNextCmdCode();

            object[] ekkrInfo = null;
            if (OutputData != null && OutputData.Length != 0)
            {
                Array.Resize<object>(ref ekkrInfo, 16 + 1 + 1 + 1 + 1 + 1 + 1);

                ekkrInfo[0] = "";
                ekkrInfo[1] = string.Format("{0} : {1}", "Режим реєстрації оплат в чеці", (OutputData[0] & 0x02) != 0 ? "ТАК" : "НІ");
                ekkrInfo[2] = string.Format("{0} : {1}", "Відкрита грошова скринька", (OutputData[0] & 0x04) != 0 ? "ТАК" : "НІ");
                ekkrInfo[3] = string.Format("{0} : {1}", "Чек", (OutputData[0] & 0x08) == 0 ? "ПРОДАЖУ" : "ВИПЛАТИ");
                ekkrInfo[4] = string.Format("{0} : {1}", "ПДВ", (OutputData[0] & 0x10) == 0 ? "ВКЛЮЧНИЙ" : "ДОДАТНІЙ");
                ekkrInfo[5] = string.Format("{0} : {1}", "Були продажі\\виплати (зміна відкрита)", (OutputData[0] & 0x20) != 0 ? "ТАК" : "НІ");
                ekkrInfo[6] = string.Format("{0} : {1}", "Відкрито чек", (OutputData[0] & 0x40) != 0 ? "ТАК" : "НІ");
                ekkrInfo[7] = string.Format("{0} : {1}", "Заборона видачі суми на індикатор", (OutputData[0] & 0x80) != 0 ? "ТАК" : "НІ");

                ekkrInfo[8] = "";
                ekkrInfo[9] = string.Format("{0} : {1}", "Були введені нові податки", (OutputData[1] & 0x02) != 0 ? "ТАК" : "НІ");
                ekkrInfo[10] = "";
                ekkrInfo[11] = string.Format("{0} : {1}", "Заборона друку", (OutputData[1] & 0x08) != 0 ? "ТАК" : "НІ");
                ekkrInfo[12] = string.Format("{0} : {1}", "Принтер фіскалізовано", (OutputData[1] & 0x10) != 0 ? "ТАК" : "НІ");
                ekkrInfo[13] = string.Format("{0} : {1}", "Аварійне завершення останньої команди", (OutputData[1] & 0x20) != 0 ? "ТАК" : "НІ");
                ekkrInfo[14] = string.Format("{0} : {1}", "Режим OnLine реєстрацій", (OutputData[1] & 0x40) != 0 ? "ТАК" : "НІ");
                ekkrInfo[15] = "";

                string sno = string.Empty;
                for (int i = 2; i < 21; i++)
                    sno += (char)OutputData[i];

                ekkrInfo[16] = string.Format("{0} : {1}", "Серійний номер і дата виробництва", sno);

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

                    ekkrInfo[17] = string.Format("{0} : {1:D2}-{2:D2}-{3:D2}", "Дата реєстрації (ДД-ММ-РР)", mas[0], mas[1], mas[2]);

                    data = new byte[2];
                    mas = new int[2];

                    data[0] = OutputData[24];
                    data[1] = OutputData[25];

                    try
                    {
                        mas = Methods.GetArrayFromBCD(data);
                    }
                    catch { }

                    ekkrInfo[18] = string.Format("{0} : {1:D2}:{2:D2}", "Час реєстрації (ЧЧ:ММ)", mas[0], mas[1]);

                    data = new byte[10];
                    for (int i = 26; i < 36; i++)
                        data[i - 26] = OutputData[i];

                    sno = Encoding.GetEncoding(866).GetString(data);

                    ekkrInfo[19] = string.Format("{0} : {1}", "Фіскальний номер", sno);

                    ekkrInfo[20] = string.Format("{0} :\r\n{1}", "Атрибути платника податків", "%1% %2% %3% %4%");

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
                ekkrInfo[21] = string.Format("{0} : {1}", "Версія ПЗ ЕККР", sno);
            }

            return ekkrInfo;
        }//make returned info
        private DateTime GetDate(ComPort port)
        {
            lastFuncName = "GetDate";
            NOM = 1;

            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 2;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private DateTime GetTime(ComPort port)
        {
            lastFuncName = "GetTime";
            NOM = 3;

            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 4;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCod(ComPort port, uint oldPass, byte no, uint newPass)
        {
            lastFuncName = "SetCod";
            NOM = 5;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCashier(ComPort port, byte n, uint pass, string name)
        {
            lastFuncName = "SetCashier";
            NOM = 6;

            //Creating data
            DataForSend = new byte[1 + 2 + 1 + name.Length];
            byte[] p = Methods.GetByteArray(pass, 2);
            DataForSend[0] = p[0];
            DataForSend[1] = p[1];
            DataForSend[2] = n;// 0..7
            DataForSend[3] = (byte)name.Length;//0..15
            byte[] BinLine = new byte[name.Length];
            BinLine = Encoding.GetEncoding(866).GetBytes(name.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 4] = BinLine[i];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] PayMoney(ComPort port, double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id, byte moneyDecimal)
        {
            lastFuncName = "PayMoney";
            NOM = 8;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;

            byte[] unusedChars = {
                (byte)0xF8,
                (byte)0xF9,
                (byte)0xFA,
                (byte)0xFB,
                (byte)0xFC
            };

            string clnName = string.Empty;
            byte[] tmpNameBytes = Encoding.GetEncoding(866).GetBytes(name);
            List<byte> clnNameBytes = new List<byte>();
            foreach (byte nc in tmpNameBytes)
            {
                if (Array.IndexOf(unusedChars, nc) >= 0)
                    continue;
                if ((0x20 <= nc && nc <= 0xAF) || (0xE0 <= nc && nc <= 0xF7))
                    clnNameBytes.Add(nc);
            }
            clnName = Encoding.GetEncoding(866).GetString(clnNameBytes.ToArray());

            name = clnName;

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
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            DataForSend[8] = oneByte;
            //Art Name length
            /*
            name = name.Trim();
            if (name.LastIndexOf('№') == name.Length - 1)
                name = name.Substring(0, name.Length - 1);
            */
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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = Methods.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = Methods.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;*/

            return ans;
        }//ok
        private void Comment(ComPort port, string text, bool retCheque)
        {
            lastFuncName = "Comment";
            NOM = 11;

            //Creating data
            DataForSend = new byte[1 + text.Length];
            DataForSend[0] = (byte)text.Length;
            if (retCheque)
                DataForSend[0] |= 0x80;

            byte[] BinLine = new byte[0];
            BinLine = Encoding.GetEncoding(866).GetBytes(text.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 1] = BinLine[i];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplPrint(ComPort port)
        {
            lastFuncName = "CplPrint";
            NOM = 12;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineFeed(ComPort port)
        {
            lastFuncName = "LineFeed";
            NOM = 14;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ResetOrder(ComPort port)
        {
            lastFuncName = "ResetOrder";
            NOM = 15;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void Avans(ComPort port, uint suma)
        {
            lastFuncName = "Avans";
            NOM = 16;

            //Creating data
            DataForSend = new byte[4];
            DataForSend = Methods.GetByteArray(suma, DataForSend.Length);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] Sale(ComPort port, double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id, byte moneyDecimal)
        {
            lastFuncName = "Sale";
            NOM = 18;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;
            //

            byte[] unusedChars = {
                (byte)0xF8,
                (byte)0xF9,
                (byte)0xFA,
                (byte)0xFB,
                (byte)0xFC
            };

            string clnName = string.Empty;
            byte[] tmpNameBytes = Encoding.GetEncoding(866).GetBytes(name);
            List<byte> clnNameBytes = new List<byte>();
            foreach (byte nc in tmpNameBytes)
            {
                if (Array.IndexOf(unusedChars, nc) >= 0)
                    continue;
                if ((0x20 <= nc && nc <= 0xAF) || (0xE0 <= nc && nc <= 0xF7))
                    clnNameBytes.Add(nc);
            }
            clnName = Encoding.GetEncoding(866).GetString(clnNameBytes.ToArray());
            // !!!!!!!!!!! CHAR TABLE TEST !!!!!!!!!!!!!!
            /*
            string testName = string.Empty;
            int page = 3;
            int maxTestNameLength = 50;
            List<byte> testNameByte = new List<byte>();
            for (byte testn = 0x20; testn < 0xF8; testn++)
                if ((0x20 <= testn && testn <= 0xAF) || (0xE0 <= testn && testn <= 0xF7))
                    testNameByte.Add(testn);
            byte[] testGenName = new byte[maxTestNameLength];
            byte[] testGenNameFull = testNameByte.ToArray();
            //List<byte> testGenName = new List<byte>();
            Array.Copy(testNameByte.ToArray(), (page * maxTestNameLength), testGenName, 0, maxTestNameLength);
            /*
            for (int nTmp = 0, iTmp = (page * maxTestNameLength); nTmp < maxTestNameLength; iTmp++, nTmp++)
            {
                testGenName[nTmp] = (byte)testGenNameFull[iTmp];
            }
            *
            testName = Encoding.GetEncoding(866).GetString(testGenName);
            */
            name = clnName;
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
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            DataForSend[8] = oneByte;
            //Art Name length
            /*
            if (name.Replace("№", ""))
                name = name.Substring(0, name.Length - 1);
            */
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
            InputData = CreateInputData(NOM, DataForSend);


            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = Methods.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = Methods.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;*/

            return ans;
        }//ok
        private object[] Payment(ComPort port, byte type, bool useAddRow, double cash, bool autoclose, string addRow)
        {
            NOM = 20;

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
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 23;

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
                InputData = CreateInputData(NOM, DataForSend);

                //sending and getting data
                SendGetData(port, 20, true);

                //Next code for command
                GetNextCmdCode();
            }
        }//ok
        private void Give(ComPort port, uint suma)
        {
            lastFuncName = "Give";
            NOM = 24;

            //Creating data
            DataForSend = Methods.GetByteArray(suma, 4);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SendCustomer(ComPort port, string[] lines, bool[] show)
        {
            lastFuncName = "SendCustomer";
            NOM = 27;

            byte[] BinLine = new byte[0];
            byte j = 0;

            for (byte i = 0; i < lines.Length; i++)
            {
                if (!show[i])
                    continue;

                lines[i] = lines[i].Replace('і','i').Replace('І','I');
                BinLine = Encoding.GetEncoding(866).GetBytes(lines[i]);

                //Creating data
                DataForSend = new byte[2 + BinLine.Length];
                DataForSend[0] = i;
                DataForSend[1] = (byte)BinLine.Length;
                for (j = 0; j < BinLine.Length; j++)
                    DataForSend[j + 2] = BinLine[j];

                //Making data
                InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 28;

            //Creating data
            DataForSend = new byte[2 + 1 + 1];
            uint block = Convert.ToUInt32(adrBlock, 16);
            byte[] blok = Methods.GetByteArray(block, 2);
            DataForSend[0] = blok[0];
            DataForSend[1] = blok[1];
            DataForSend[2] = pageNo;
            DataForSend[3] = blockSize;

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            return OutputData;
        }//ok
        private void OpenBox(ComPort port)
        {
            lastFuncName = "OpenBox";
            NOM = 29;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintCopy(ComPort port)
        {
            lastFuncName = "PrintCopy";
            NOM = 30;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintVer(ComPort port)
        {
            lastFuncName = "PrintVer";
            NOM = 32;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private uint GetBox(ComPort port)
        {
            lastFuncName = "GetBox";
            NOM = 33;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();

            return (uint)Methods.GetNumber(OutputData);
        }//ok
        private void Discount(ComPort port, byte type, double value, byte digitsAfterPoint, string helpLine)
        {
            NOM = 35;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//!!!!
        private void CplOnline(ComPort port)
        {
            lastFuncName = "CplOnline";
            NOM = 36;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplInd(ComPort port)
        {
            lastFuncName = "CplInd";
            NOM = 37;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ChangeRate(ComPort port, byte rateType)
        {
            lastFuncName = "ChangeRate";
            NOM = 38;

            //Creationg data
            DataForSend = new byte[1];
            DataForSend[0] = rateType;

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineSP(ComPort port, byte lsp)
        {
            lastFuncName = "LineSP";
            NOM = 39;

            //Creating data
            DataForSend = new byte[1];
            DataForSend[0] = lsp;

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void TransPrint(ComPort port, string line, bool close)
        {
            lastFuncName = "TransPrint";
            NOM = 40;

            //Creating data
            DataForSend = new byte[1 + line.Length];
            DataForSend[0] = (byte)line.Length;
            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(line.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                DataForSend[i + 1] = BinLine[i];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

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
                InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 41;

            //Making data
            byte[] aid = Methods.GetByteArray(id, 6);
            InputData = CreateInputData(NOM, aid);

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
                    case "80": tax = 'А'; break;
                    case "81": tax = 'Б'; break;
                    case "82": tax = 'В'; break;
                    case "83": tax = 'Г'; break;
                    case "84": tax = 'Д'; break;
                    case "85": tax = 'Е'; break;
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
            NOM = 42;

            //Creating data
            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 43;
            //Creating data
            DataForSend = new byte[0];

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 44;

            //Making data
            DataForSend = new byte[0];
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 46;

            //Making data
            InputData = CreateInputData(NOM, new byte[0]);

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
            NOM = 21;

            //Creating data
            byte[] pass = Methods.GetByteArray(progPass, 2);
            DataForSend = new byte[2 + 10];
            DataForSend[0] = pass[0];
            DataForSend[1] = pass[1];
            Encoding.GetEncoding(866).GetBytes(fn).CopyTo(DataForSend, 2);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetHeadLine(ComPort port, ushort progPass, string line1, string line2, string line3, string line4)
        {
            lastFuncName = "SetHeadLine";
            NOM = 25;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetTaxRate(ComPort port, ushort progPass, byte totTax, uint[] tax, byte status, byte totGTax, uint[] gtax)
        {
            lastFuncName = "SetTaxRate";
            NOM = 25;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ProgArt(ComPort port, ushort progPass, byte doseDecimal, double price, char pdv, string name, uint id)
        {
            lastFuncName = "ProgArt";
            NOM = 34;

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
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void LoadBMP(ComPort port, ushort progPass, bool allow, string fpath)
        {
            lastFuncName = "LoadBMP";
            NOM = 45;

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
            InputData = CreateInputData(NOM, DataForSend);

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
            NOM = 7;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ArtXReport(ComPort port, uint reportPass)
        {
            lastFuncName = "ArtXReport";
            NOM = 10;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayReport(ComPort port, uint reportPass)
        {
            lastFuncName = "DayReport";
            NOM = 9;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayClrReport(ComPort port, uint reportPass)
        {
            lastFuncName = "DayClrReport";
            NOM = 13;

            //Creating data
            DataForSend = Methods.GetByteArray(reportPass, 2);

            //Making data
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            SaveArtID(0);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport(ComPort port, uint reportPass, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "PeriodicReport";
            NOM = 17;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReportShort(ComPort port, uint reportPass, DateTime startDate, DateTime endDate)
        {
            lastFuncName = "PeriodicReportShort";
            NOM = 26;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport2(ComPort port, uint reportPass, uint startID, uint endID)
        {
            lastFuncName = "PeriodicReport2";
            NOM = 31;

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
            InputData = CreateInputData(NOM, DataForSend);

            //sending and getting data
            SendGetData(port, 20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        #endregion

        //Methods
        private void GetNextCmdCode()
        {
            //if (COD == 0xFF)
            //    COD = 0x01;
            //else
                COD++;
        }
        private byte[] CreateInputData(byte _nom, byte[] param)
        {
            byte[] mas = new byte[4 + param.Length + 3];
            byte i = 0;

            mas[0] = DLE;
            mas[1] = STX;
            mas[2] = COD;
            mas[3] = _nom;

            CS = (byte)(0 - (COD + Methods.SumMas(param) + _nom));

            for (i = 0; i < param.Length; i++)
                mas[i + 4] = param[i];

            mas[4 + param.Length] = CS;
            mas[4 + param.Length + 1] = DLE;
            mas[4 + param.Length + 2] = ETX;

            byte[] validData = new byte[0];

            for (i = 0; i < mas.Length; i++)
            {
                if (mas[i] == DLE && i != 0 && i != mas.Length - 2)
                {
                    Array.Resize<byte>(ref validData, validData.Length + 1);
                    validData[validData.Length - 1] = DLE;
                }

                Array.Resize<byte>(ref validData, validData.Length + 1);
                validData[validData.Length - 1] = mas[i];
            }

            return validData;
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
                                        Errors[lastFuncName] = false;
                                        return true;
                                    }
                                case "false":
                                    {
                                        //WinAPI.OutputDebugString("F");
                                        t--;
                                        Thread.Sleep(50);
                                        break;
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

            Errors[lastFuncName] = true;
            throw new Exception("Помилка читання з фіскального принтера" + " " + Protocol_Name + exceptionMsg);
        }
        private string DecodeAnswer()
        {
            if (OutputData.Length == 0)
                return "false";

            if (OutputData[0] == ACK)
            {
                byte i = 0;
                byte symbol = 0x00;

                byte[] normalizedAnswer = new byte[0];

                bool dleDetected = false;
                bool synDetected = false;

                //remove all SYN symbols
                for (i = 0; i < OutputData.Length; i++)
                {
                    if (OutputData[i] == DLE && !dleDetected) dleDetected = true;
                    if (OutputData[i] == SYN && !synDetected) synDetected = true;

                    if (!dleDetected && synDetected)
                    {
                        symbol++;
                        continue;
                    }

                    Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                    normalizedAnswer[normalizedAnswer.Length - 1] = OutputData[i];
                }

                if (synDetected && !dleDetected)
                    return "busy";                    

                OutputData = (byte[])normalizedAnswer.Clone();
                normalizedAnswer = new byte[0];

                //remove all dolbyDLE symbols
                for (i = 0; i < (OutputData.Length - 1); i++)
                {
                    Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                    normalizedAnswer[normalizedAnswer.Length - 1] = OutputData[i];
                    if (OutputData[i] == DLE && OutputData[i + 1] == DLE) i++;
                }
                Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                normalizedAnswer[normalizedAnswer.Length - 1] = OutputData[i];

                if (normalizedAnswer[normalizedAnswer.Length - 1] == ETX && normalizedAnswer[normalizedAnswer.Length - 2] == DLE)
                {
                    i = (byte)(normalizedAnswer.Length - 3);
                    CS = normalizedAnswer[i];
                }

                i--;

                //normalizedAnswer = (byte[])answer.Clone();
                OutputData = (byte[])normalizedAnswer.Clone();
                normalizedAnswer = new byte[0];

                symbol = 0x00;
                while (true)
                {
                    try
                    {
                        Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                        normalizedAnswer[normalizedAnswer.Length - 1] = OutputData[i];
                        symbol = Methods.SumMas(normalizedAnswer);
                        i--;
                        if ((byte)(symbol + CS) == 0 && OutputData[i] == STX && OutputData[i - 1] == DLE)
                            break;
                    }
                    catch { return "false"; }
                }

                Array.Reverse(normalizedAnswer);

                if (normalizedAnswer.Length == 0)
                    return "false";

                if (normalizedAnswer[0] == COD && normalizedAnswer[1] == NOM)
                {
                    state = normalizedAnswer[2];
                    rezult = normalizedAnswer[3];
                    reserv = normalizedAnswer[4];

                    //read state
                    i = 0;
                    string oper_info = string.Empty;
                    for (byte mask = 1; i < 8; mask *= 2, i++)
                        if ((state & mask) == 1)
                            oper_info += stateMsg[i] + "\r\n";

                    //read rezult
                    oper_info += rezultMsg[rezult];

                    if (oper_info.Length != 0)
                        throw new Exception(oper_info);


                    //read reserv state
                    /*
                    if (reserv < reservMsg.Length && reservMsg[reserv] != "")
                        System.Windows.Forms.MessageBox.Show(reservMsg[reserv], Protocol_Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    */
                    Array.Reverse(normalizedAnswer);
                    Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length - 5);
                    Array.Reverse(normalizedAnswer);

                    OutputData = normalizedAnswer;
                    return "true";
                }
            }//if ACK

            return "false";
        }
        //Temp
        private uint LoadArtID(ComPort port)
        {
            uint aid = 0;
            byte[] mem = GetMemory(port, "0065", (byte)16, (byte)2);
            uint fxAid = (uint)Methods.GetNumber(mem);
            uint nextAID = 0;
            try
            {
                /*
                System.IO.StreamReader sRd = new System.IO.StreamReader("aid.fxm", Encoding.Default);
                aid = uint.Parse(sRd.ReadLine());
                sRd.Close();
                sRd.Dispose();
                */
            }
            catch { }

            if (aid < fxAid)
                nextAID = fxAid;

            //nextAID += 10;

            return nextAID;
        }
        private void SaveArtID(object aid)
        {
            try
            {
                System.IO.StreamWriter sRw = new System.IO.StreamWriter("aid.fxm", false, Encoding.Default);
                sRw.Write(aid);
                sRw.Close();
                sRw.Dispose();
            }
            catch { }
        }

        private uint FP_LastZRepNo(ComPort port)
        {
            byte[] _mem = GetMemory(port, "0037", (byte)16, (byte)2);
            uint _zn = (uint)Methods.GetNumberFromBCD(_mem, 10);
            return _zn;
        }
    }
}

/* Strcuture of memory
Внутренняя память. Страница 0.
     16h    -  количество налоговых ставок
     29h    -  байт конфигурации 1
        BIT  1      -  режим оплат в чеке
        BIT  3      -  чек выплат
        BIT  4      -  НДС вложенный \ НДС наложенный
        BIT  5      -  открытая смена
        BIT  6      -  открытый чек
        BIT  7      -  игнорирование вывода на индикатор
     2Ah    -  байт конфигурации 2
        BIT  1      -  введены новые налоги
        BIT  4      -  аппарат зарегистрирован
        BIT  5      -  состояние ошибки (пред. команда не завершена)
        BIT  6      -  режим OnLine

Внешняя память. Страница 16.
     0000h         3        дата регистрации ЭККР
     0003h         2        время регистрации ЭККР
     0005h         10       регистрационный номер ЭККР
     000Fh                  имя кассира 
     001Fh         24       серийный номер, дата и время производства
     0037h         2        текущий номер Z-отчета
     0063h         1        счетчик сброса памяти (инициализаций)
     0065h         2        счетчик артикулов запрограммированных по ходу продаж
     0067h         2        счетчик предварительно запрограммированных артикулов
     0069h         3        указатель конца списка артикулов
     008Bh         3        дата начала смены
     008Eh         2        время начала смены
     0090h         3        указатель ошибки в фискальной памяти
     0096h                  заголовок чека 4 строки
     00FCh         3        указатель конца записей в фискальной памяти

     0100h        4*6       суммы продаж по налогам в чеке
     0118h        4*4       суммы оплат по видам в чеке
     014Ch         3        дата последней регистрации налогов
     0150h  2*NT+1+2*NT     налоговые ставки по 2 байта + статус + ставки сборов по 2 байта
     0169h         3        дата последнего дневного отчета
     2F00h         5*6      суммы налогов по налоговым группам для наложенного НДС
     2F60h         5        суммарная наценка на сумму по продажам
     2F65h         5        суммарная скидка на сумму по продажам
     2F6Ah         5        суммарная наценка на сумму по выплатам
     2F6Fh         5        суммарная скидка на сумму по выплатам
     2F74h         2        количество аннулирований чеков продаж
     2F76h         5        сумма по аннулированным чекам продаж
     2F7Bh         2        количество аннулирований чеков выплат
     2F7Dh         5        сумма по аннулированным чекам выплат
     2F82h         2        количество отмен в чеках продаж
     2F84h         5        сумма отмен в чеках продаж
     2F89h         2        количество отмен в чеках выплат
     2F8Bh         5        сумма отмен в чеках выплат

     3000h         1        код последней незавершенной команды 
     3002h         1        номер скорости обмена по RS-232
     3003h        2*10      пароли кассиров, программирования и отчетов
     3017h         1        количество налоговых ставок
     301Ah         1        флаги состояния при включении ЭККР
        BIT  0       1      режим OnLine
        BIT  1       1      игнорирование вывода на индикатор суммы
        BIT  2       1      печатать пользовательский логотип
        BIT  3       1      запрет обрезчика бумаги

     301Bh         2        счетчик чеков продаж
     301Dh    5*(6+4)       счетчики сумм продаж  по налоговым группам и формам оплат 
     3050h         5        сменная наценка по продажам
     3055h         5        сменная скидка по продажам
     305Ah         5        сменная сумма аванса
     3060h                  первая строка рекламы
     3085h                  последняя строка рекламы
     30ABh         2        счетчик чеков выплат
     30ADh    5*(6+4)       счетчики сумм выплат по налоговым группам и формам оплат
     30DFh        5         сменная наценка по выплатам
     30E4h        5         сменная скидка по выплатам
     30E9h        5         сменная сумма выдано
     30EFh        1         счетчик регистраций (продажи, выплаты, комментарии и оплаты) в
                            текущем чеке 
*/