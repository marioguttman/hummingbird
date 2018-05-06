using System;

namespace RevitModelBuilder {

    public partial class ProgressBar : System.Windows.Forms.Form {

        public bool Cancel = false;

        public ProgressBar(string title, int maximum) {
            InitializeComponent();
            this.Text = title;
            progressBar1.Maximum = maximum;
        }

        public void SetLabel(string text) {
            labelProgressBar.Text = text;
            labelProgressBar.Refresh();
        }

        public void Increment() {
            progressBar1.Increment(1);
        }

        public void SetValue(int value) {
            if (value <= progressBar1.Maximum) progressBar1.Value = value;
            else progressBar1.Value = progressBar1.Maximum;
        }

        public void Reset() {
            progressBar1.Value = 0;
            progressBar1.Refresh();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Cancel = true;
        }
    }
   
}