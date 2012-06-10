using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using components.Public;

namespace components.UI.Controls.CategoryNavBar
{
    public partial class CategoryNavBar : UserControl
    {
        public Hashtable CategoryDataSource { get; set; }
        private int maxPagesForCurrentCategory;
        private int itemsPerPage;
        private string currentCategoryId;
        private string currentCategoryFilter;
        private int currentPageNum;
        // ui
        public bool UseAutomaticButtonRendering { get; set; }


        public int DisplayedPage
        {
            get
            {
                return this.currentPageNum;
            }
            set
            {
                if (value == null)
                    this.currentPageNum = 0;
                else
                    this.currentPageNum = value;
                this.OnPageChanged.Invoke(this.currentPageNum, EventArgs.Empty);
            }
        }

        public string DisplayedCategoryFilter
        {
            get
            {
                return this.currentCategoryFilter;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.currentCategoryFilter = string.Empty;
                else
                    this.currentCategoryFilter = value;
                this.OnFilterChanged.Invoke(this.currentCategoryFilter, EventArgs.Empty);
            }
        }

        public string DisplayedCategoryId
        {
            get
            {
                return this.currentCategoryId;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.currentCategoryId = string.Empty;
                else
                    this.currentCategoryId = value;
                this.OnCategoryIdChanged.Invoke(this.currentCategoryId, EventArgs.Empty);
            }
        }

        public CategoryNavBar()
        {
            this.OnCategoryIdChanged += new CategoryIdChanged(CategoryNavBar_OnCategoryIdChanged);
            this.OnFilterChanged += new FilterChanged(CategoryNavBar_OnFilterChanged);
            this.OnPageChanged += new PageChanged(CategoryNavBar_OnPageChanged);
            this.OnHomeTabClicked += new HomePageClicked(CategoryNavBar_OnHomeTabClicked);
            this.OnBreadcrumbButtonClicked += new BreadcrumbButtonClicked(CategoryNavBar_OnBreadcrumbButtonClicked);
            this.OnCategoryButtonClicked += new CategoryButtonClicked(CategoryNavBar_OnCategoryButtonClicked);

            InitializeComponent();

            DisplayedCategoryFilter = string.Empty;
            DisplayedPage = 0;
            itemsPerPage = 6;
            UseAutomaticButtonRendering = true;
        }


        public CategoryNavBar(Hashtable data)
            : this()
        {
            // remove data when this form will be converted to the control object
            CategoryDataSource = data;
            // remove startup init by the reason above
            ShowCategoryItems();

        }


        public void SetAndShowNavigator(Hashtable data)
        {
            // remove data when this form will be converted to the control object
            CategoryDataSource = data;
            // remove startup init by the reason above
            ShowCategoryItems();
        }

        // home tab
        private void ShowCategoryItems()
        {
            // home state
            this.flowLayoutPanel1.Controls.Clear();
            this.flowLayoutPanel2.Controls.Clear();
            
            this.DisplayedPage = 0;

            ShowCategoryItems(string.Empty);
            this.OnHomeTabClicked.Invoke("", EventArgs.Empty);
        }

        // specific category
        private void RefreshDisplayedCategory()
        {
            ShowCategoryItems(this.currentCategoryId); 
        }

        private void ShowCategoryItems(string categoryId)
        {
            Hashtable currentCategoryEntry = null;
            if (!string.IsNullOrEmpty(categoryId))
            {
                currentCategoryEntry = (Hashtable)ApplicationConfiguration.Instance.XmlParser.GetValueByKey(this.CategoryDataSource, categoryId);
                this.DisplayedCategoryId = categoryId;
                try
                {
                    this.DisplayedCategoryFilter = currentCategoryEntry["filter"].ToString();
                }
                catch { }
            }
            // if there is no any category
            // use root category
            if (currentCategoryEntry == null)
            {
                currentCategoryEntry = (Hashtable)CategoryDataSource.Clone();
                this.DisplayedCategoryId = string.Empty;
                this.DisplayedCategoryFilter = string.Empty;
            }


            Hashtable buttonInfo = null;

            this.flowLayoutPanel2.Controls.Clear();

            this.maxPagesForCurrentCategory = (int)(currentCategoryEntry.Count / (this.itemsPerPage + 0.0));


            int printedCategory = 0;
            int addedCategory = 0;

            // with filter

            List<string> innerKeys = new List<string>();
            foreach (DictionaryEntry kde in currentCategoryEntry)
                innerKeys.Add(kde.Key.ToString());
            innerKeys.Sort();

            //foreach (DictionaryEntry de in currentCategoryEntry)
            foreach(string entryKey in innerKeys)
            {
                DictionaryEntry de = new DictionaryEntry(entryKey, currentCategoryEntry[entryKey]);

                if (printedCategory < this.itemsPerPage * this.DisplayedPage)
                {
                    if (de.Value.GetType() == typeof(Hashtable))
                        printedCategory++;
                    continue;
                }

                if (addedCategory >= this.itemsPerPage)
                    break;

                if (de.Value.GetType() == typeof(Hashtable))
                {

                    buttonInfo = (Hashtable)de.Value;
                    UI_AddCategoryButton(buttonInfo["title"].ToString(), de.Key.ToString(), categoryId, buttonInfo["level"].ToString(), buttonInfo["filter"].ToString());
                    addedCategory++;
                }

            }

            // button view refactoring
            UpdateCategoryButonsWidth();
        }


        void btn_Click(object sender, EventArgs e)
        {
            string[] clickedCategoryButton = ((Button)sender).Tag.ToString().Split(':');

            // if breadcrumb was clicked
            if (clickedCategoryButton.Length == 5 && clickedCategoryButton[4] == "B")
            {
                // remove all nav buttons after
                int idx = this.flowLayoutPanel1.Controls.IndexOf((Button)sender);
                int brButtCount = this.flowLayoutPanel1.Controls.Count;
                List<Control> toRemove = new List<Control>();
                for (int i = idx + 1; i < brButtCount; i++)
                    toRemove.Add(this.flowLayoutPanel1.Controls[i]);
                for (int i = 0; i < toRemove.Count; i++)
                    this.flowLayoutPanel1.Controls.Remove(toRemove[i]);

                this.OnBreadcrumbButtonClicked.Invoke(clickedCategoryButton, EventArgs.Empty);
            }
            else
            {
                this.OnCategoryButtonClicked.Invoke(clickedCategoryButton, EventArgs.Empty);
                UI_AddNavButton(((Button)sender).Text, clickedCategoryButton[0], clickedCategoryButton[3], clickedCategoryButton[1], clickedCategoryButton[2]);
            }
            this.DisplayedPage = 0;
            // attempt to show inner content of clicked category
            ShowCategoryItems(clickedCategoryButton[0]);

        }

        private void UI_AddCategoryButton(string title, string id, string owner, string level, string filer)
        {
            this.flowLayoutPanel2.Controls.Add(UI_CreateButton(title, id, owner, level, filer, false));
        }

        private void UI_AddNavButton(string title, string id, string owner, string level, string filer)
        {
            Button brButt = UI_CreateButton(title, id, owner, level, filer, true);
            brButt.Size = new Size(brButt.PreferredSize.Width, flowLayoutPanel1.Height - flowLayoutPanel1.Padding.Vertical);
            this.flowLayoutPanel1.Controls.Add(brButt);
        }

        private Button UI_CreateButton(string title, string id, string owner, string level, string filer, bool isnav)
        {
            Button btn = new Button();
            //btn.AutoSize = true;
            btn.Name = id;
            btn.Text = title;
            if (this.UseAutomaticButtonRendering)
                btn.AutoSize = true;
            else
                btn.Size = new Size(130, 55);
            btn.Padding = new Padding(10);
            btn.Tag = id + ":" + level + ":" + filer + ":" + owner;
            if (isnav)
                btn.Tag = btn.Tag + ":B";
            btn.Click += new EventHandler(btn_Click);
            btn.Cursor = Cursors.Hand;
            btn.UseCompatibleTextRendering = true;
            return btn;
        }

        private void button_home_Click(object sender, EventArgs e)
        {
            ShowCategoryItems();
        }

        private void button_flippers_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Tag.ToString())
            {
                case "left":
                    {
                        this.DisplayedPage--;
                        if (this.DisplayedPage < 0)
                            this.DisplayedPage = 0;
                        break;
                    }

                case "right":
                    {
                        this.DisplayedPage++;
                        if (this.DisplayedPage > this.maxPagesForCurrentCategory)
                            this.DisplayedPage = this.maxPagesForCurrentCategory;
                        break;
                    }
            }

            RefreshDisplayedCategory();

        }

        // ui methods

        public void UpdateCategoryButonsWidth()
        {
            if (!UseAutomaticButtonRendering)
                return;

            int maxBtnWidth = 0;
            int maxBtnHeight = 0;
            int widthCap = flowLayoutPanel2.Width - flowLayoutPanel2.Padding.Horizontal - flowLayoutPanel2.Margin.Horizontal;
            int availableItems = 0;

            foreach (Button catBtn in flowLayoutPanel2.Controls)
            {
                catBtn.AutoSize = true;
                if (catBtn.Width > maxBtnWidth)
                    maxBtnWidth = catBtn.Width;
                if (catBtn.Height > maxBtnHeight)
                    maxBtnHeight = catBtn.Height;
                widthCap -=  catBtn.Margin.Horizontal;
                availableItems++;
            }

            if (availableItems == 0)
                return;


            // button
            // if there are more than IPP
            bool separateInTwoRows = false;
            if (availableItems == this.itemsPerPage)
            {
                availableItems = availableItems / 2;
                separateInTwoRows = true;
            }
            bool unusualCorrect = false;
            if (availableItems > (this.itemsPerPage / 2 + 1))
            {
                unusualCorrect = true;
            }

            int widthCorr = (widthCap / availableItems);
            if (unusualCorrect)
                widthCorr = widthCap / (this.itemsPerPage / 2);


            foreach (Button catBtn in flowLayoutPanel2.Controls)
            {
                catBtn.AutoSize = false;
                catBtn.Width = widthCorr;
                if (separateInTwoRows || unusualCorrect)
                    catBtn.Height = (flowLayoutPanel2.Height - (flowLayoutPanel2.Margin.Vertical * 2)) / 2;
                else
                    catBtn.Height = flowLayoutPanel2.Height - flowLayoutPanel2.Margin.Vertical;
            }

        }


        // events
        public delegate void CategoryIdChanged(string categoryId, EventArgs e);
        public delegate void FilterChanged(string filter, EventArgs e);
        public delegate void PageChanged(int pageNumber, EventArgs e);
        public delegate void HomePageClicked(string homePageId, EventArgs e);
        public delegate void BreadcrumbButtonClicked(string[] buttonInfo, EventArgs e);
        public delegate void CategoryButtonClicked(string[] buttonInfo, EventArgs e);

        public event CategoryIdChanged OnCategoryIdChanged;
        public event FilterChanged OnFilterChanged;
        public event PageChanged OnPageChanged;
        public event HomePageClicked OnHomeTabClicked;
        public event BreadcrumbButtonClicked OnBreadcrumbButtonClicked;
        public event CategoryButtonClicked OnCategoryButtonClicked;

        // event handlers

        public void CategoryNavBar_OnCategoryIdChanged(string categoryId, EventArgs e)
        {

        }

        public void CategoryNavBar_OnFilterChanged(string filter, EventArgs e)
        {

        }

        public void CategoryNavBar_OnCategoryButtonClicked(string[] buttonInfo, EventArgs e)
        {

        }

        public void CategoryNavBar_OnBreadcrumbButtonClicked(string[] buttonInfo, EventArgs e)
        {

        }

        public void CategoryNavBar_OnHomeTabClicked(string homePageId, EventArgs e)
        {

        }

        public void CategoryNavBar_OnPageChanged(int pageNumber, EventArgs e)
        {

        }

        private void flowLayoutPanel2_SizeChanged(object sender, EventArgs e)
        {
            UpdateCategoryButonsWidth();
        }
    }
}
