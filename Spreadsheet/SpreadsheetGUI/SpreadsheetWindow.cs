﻿using SSGui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {

        //Constructs the window. 
        public SpreadsheetWindow()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;
        }

        public event Action CloseEvent;
        public event Action<string> CountEvent;
        public event Action<string> FileChosenEvent;
        public event Action NewEvent;
        public event Action<SpreadsheetPanel> UpdateCell;

        /// <summary>
        /// When we edit the contents of the cell.
        /// </summary>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);
            if (value == "")
            {
                ss.SetValue(col, row, DateTime.Now.ToLocalTime().ToString("T"));
                ss.GetValue(col, row, out value);
            }

        }

        public string Message
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Title
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Content
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void DoClose()
        {
            Close();
        }

        public void OpenNew()
        {
            throw new NotImplementedException();
        }


        private void spreadsheetPanel1_DoubleClick(object sender, EventArgs e)
        {

        }

        //When we double click on the spreadsheet panel, it should fire the UpdateCell event.
        private void spreadsheetPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            //UpdateCell();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //If user presses "enter" in the editable contents box, fires the event to update all cell fields.
        private void ContentBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return || e.KeyChar == (char)Keys.Enter)
            {
                Content = ContentBox.Text;
               // UpdateCell();
            }
        }

    }
}
