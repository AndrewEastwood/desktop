using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace mdcore.Lib
{
    public static class CleanLib
    {
        public static void ClearCheckedBox(ref CheckedListBox checkedListBox1)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
            checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, false);
        }
    }
}
