using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Windows.Forms;

using System.IO;                     //for File, Directory, StreamWriter

namespace RevitDocumentation {
    public partial class Documents : System.Windows.Forms.Form {
        public Documents(string documentationFolder) {
            InitializeComponent();
            mDocumentationFolder = documentationFolder;
            FillDocumentsList();
            
        }

        private string mDocumentationFolder;

        private bool FillDocumentsList() {
            try {
                string[] documents = Directory.GetFiles(mDocumentationFolder);
                foreach (string document in documents) {
                    int position = document.LastIndexOf("\\") + 1;
                    listBoxDocuments.Items.Add(document.Substring(position));
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonDisplay_Click(object sender, EventArgs e) {
            if (listBoxDocuments.SelectedItem == null) {
                System.Windows.Forms.MessageBox.Show("Select document to display.");
                return;
            }

            try {
                string path = mDocumentationFolder + "\\" + listBoxDocuments.SelectedItem.ToString();
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
            }

            //to force the window to be full-screen maximized
            //System.Diagnostics.ProcessStartInfo PSI = new System.Diagnostics.ProcessStartInfo();
            //PSI.FileName = @"C:\junk.doc";
            //PSI.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            //System.Diagnostics.Process.Start(PSI);

        }

    }
}
