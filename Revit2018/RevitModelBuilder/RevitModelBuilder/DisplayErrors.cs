using System;
using System.Windows.Forms;

namespace RevitModelBuilder {
    public partial class DisplayErrors : Form {
        public DisplayErrors(string message) {
            InitializeComponent();
            textBoxErrorMessage.Text = message;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

    }
}
