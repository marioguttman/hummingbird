namespace HummingbirdCSExample {
    partial class Menu {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxPathFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSite = new System.Windows.Forms.Button();
            this.buttonSpiral = new System.Windows.Forms.Button();
            this.buttonConceptMass = new System.Windows.Forms.Button();
            this.buttonHoles = new System.Windows.Forms.Button();
            this.buttonTwistyTower = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTwistFloors = new System.Windows.Forms.TextBox();
            this.textBoxTwistHeight = new System.Windows.Forms.TextBox();
            this.textBoxTwistTwist = new System.Windows.Forms.TextBox();
            this.checkBoxPreserveId = new System.Windows.Forms.CheckBox();
            this.textBoxTwistTaper = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxConceptGridX = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxConceptGridY = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxConceptCellY = new System.Windows.Forms.TextBox();
            this.textBoxConceptCellX = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxConceptGrowX = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxConceptGrowY = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxConceptHeight = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBoxSpiralFactorA = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxSpiralFactorB = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxSpiralPoints = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSiteSizeY = new System.Windows.Forms.TextBox();
            this.textBoxSiteSizeX = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBoxSiteDivisionsX = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxSiteRandom = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBoxSiteElevTopLeft = new System.Windows.Forms.TextBox();
            this.textBoxSiteElevBotLeft = new System.Windows.Forms.TextBox();
            this.textBoxSiteElevBotRight = new System.Windows.Forms.TextBox();
            this.textBoxSiteElevTopRight = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.textBoxSiteDivisionsY = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(788, 440);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(102, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxPathFolder
            // 
            this.textBoxPathFolder.Location = new System.Drawing.Point(20, 30);
            this.textBoxPathFolder.Name = "textBoxPathFolder";
            this.textBoxPathFolder.Size = new System.Drawing.Size(650, 20);
            this.textBoxPathFolder.TabIndex = 6;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(687, 26);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(29, 26);
            this.buttonBrowse.TabIndex = 5;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Full Path to Folder for CSV Files:";
            // 
            // buttonSite
            // 
            this.buttonSite.Location = new System.Drawing.Point(22, 371);
            this.buttonSite.Name = "buttonSite";
            this.buttonSite.Size = new System.Drawing.Size(147, 23);
            this.buttonSite.TabIndex = 12;
            this.buttonSite.Text = "Site";
            this.buttonSite.UseVisualStyleBackColor = true;
            this.buttonSite.Click += new System.EventHandler(this.buttonSite_Click);
            // 
            // buttonSpiral
            // 
            this.buttonSpiral.Location = new System.Drawing.Point(22, 302);
            this.buttonSpiral.Name = "buttonSpiral";
            this.buttonSpiral.Size = new System.Drawing.Size(147, 23);
            this.buttonSpiral.TabIndex = 13;
            this.buttonSpiral.Text = "Spiral";
            this.buttonSpiral.UseVisualStyleBackColor = true;
            this.buttonSpiral.Click += new System.EventHandler(this.buttonSpiral_Click);
            // 
            // buttonConceptMass
            // 
            this.buttonConceptMass.Location = new System.Drawing.Point(22, 222);
            this.buttonConceptMass.Name = "buttonConceptMass";
            this.buttonConceptMass.Size = new System.Drawing.Size(147, 23);
            this.buttonConceptMass.TabIndex = 14;
            this.buttonConceptMass.Text = "Conceptual Mass";
            this.buttonConceptMass.UseVisualStyleBackColor = true;
            this.buttonConceptMass.Click += new System.EventHandler(this.buttonConceptMass_Click);
            // 
            // buttonHoles
            // 
            this.buttonHoles.Location = new System.Drawing.Point(22, 160);
            this.buttonHoles.Name = "buttonHoles";
            this.buttonHoles.Size = new System.Drawing.Size(147, 23);
            this.buttonHoles.TabIndex = 15;
            this.buttonHoles.Text = "Tower With Holes";
            this.buttonHoles.UseVisualStyleBackColor = true;
            this.buttonHoles.Click += new System.EventHandler(this.buttonHoles_Click);
            // 
            // buttonTwistyTower
            // 
            this.buttonTwistyTower.Location = new System.Drawing.Point(22, 99);
            this.buttonTwistyTower.Name = "buttonTwistyTower";
            this.buttonTwistyTower.Size = new System.Drawing.Size(147, 23);
            this.buttonTwistyTower.TabIndex = 16;
            this.buttonTwistyTower.Text = "TwistyTower";
            this.buttonTwistyTower.UseVisualStyleBackColor = true;
            this.buttonTwistyTower.Click += new System.EventHandler(this.buttonTwistyTower_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 246);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "( Use in Conceptual Modeler. )";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 26);
            this.label2.TabIndex = 18;
            this.label2.Text = "No. Floors:\r\n( Integer )";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(365, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 26);
            this.label4.TabIndex = 19;
            this.label4.Text = "Floor Height:\r\n( Decimal Ft. )";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(758, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 26);
            this.label5.TabIndex = 20;
            this.label5.Text = "Twist Factor:\r\n( 1 = 360 deg. )";
            // 
            // textBoxTwistFloors
            // 
            this.textBoxTwistFloors.Location = new System.Drawing.Point(284, 101);
            this.textBoxTwistFloors.Name = "textBoxTwistFloors";
            this.textBoxTwistFloors.Size = new System.Drawing.Size(52, 20);
            this.textBoxTwistFloors.TabIndex = 21;
            // 
            // textBoxTwistHeight
            // 
            this.textBoxTwistHeight.Location = new System.Drawing.Point(479, 101);
            this.textBoxTwistHeight.Name = "textBoxTwistHeight";
            this.textBoxTwistHeight.Size = new System.Drawing.Size(52, 20);
            this.textBoxTwistHeight.TabIndex = 22;
            // 
            // textBoxTwistTwist
            // 
            this.textBoxTwistTwist.Location = new System.Drawing.Point(838, 101);
            this.textBoxTwistTwist.Name = "textBoxTwistTwist";
            this.textBoxTwistTwist.Size = new System.Drawing.Size(52, 20);
            this.textBoxTwistTwist.TabIndex = 23;
            // 
            // checkBoxPreserveId
            // 
            this.checkBoxPreserveId.AutoSize = true;
            this.checkBoxPreserveId.Location = new System.Drawing.Point(21, 60);
            this.checkBoxPreserveId.Name = "checkBoxPreserveId";
            this.checkBoxPreserveId.Size = new System.Drawing.Size(228, 17);
            this.checkBoxPreserveId.TabIndex = 27;
            this.checkBoxPreserveId.Text = "Preserve Existing Revit Element ID Values.";
            this.checkBoxPreserveId.UseVisualStyleBackColor = true;
            // 
            // textBoxTwistTaper
            // 
            this.textBoxTwistTaper.Location = new System.Drawing.Point(679, 100);
            this.textBoxTwistTaper.Name = "textBoxTwistTaper";
            this.textBoxTwistTaper.Size = new System.Drawing.Size(52, 20);
            this.textBoxTwistTaper.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(565, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 26);
            this.label10.TabIndex = 28;
            this.label10.Text = "Taper Factor:\r\n( 0 < x < 1.0 )";
            // 
            // textBoxConceptGridX
            // 
            this.textBoxConceptGridX.Location = new System.Drawing.Point(284, 221);
            this.textBoxConceptGridX.Name = "textBoxConceptGridX";
            this.textBoxConceptGridX.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptGridX.TabIndex = 32;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(186, 224);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 26);
            this.label12.TabIndex = 31;
            this.label12.Text = "Grid Size:\r\n( Integer )";
            // 
            // textBoxConceptGridY
            // 
            this.textBoxConceptGridY.Location = new System.Drawing.Point(284, 246);
            this.textBoxConceptGridY.Name = "textBoxConceptGridY";
            this.textBoxConceptGridY.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptGridY.TabIndex = 34;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(261, 251);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 33;
            this.label11.Text = "Y:";
            // 
            // textBoxConceptCellY
            // 
            this.textBoxConceptCellY.Location = new System.Drawing.Point(479, 246);
            this.textBoxConceptCellY.Name = "textBoxConceptCellY";
            this.textBoxConceptCellY.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptCellY.TabIndex = 39;
            // 
            // textBoxConceptCellX
            // 
            this.textBoxConceptCellX.Location = new System.Drawing.Point(479, 219);
            this.textBoxConceptCellX.Name = "textBoxConceptCellX";
            this.textBoxConceptCellX.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptCellX.TabIndex = 37;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(461, 251);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 13);
            this.label15.TabIndex = 38;
            this.label15.Text = "Y:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(365, 223);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(72, 26);
            this.label16.TabIndex = 36;
            this.label16.Text = "Cell Size:\r\n( Decimal Ft. )";
            // 
            // textBoxConceptGrowX
            // 
            this.textBoxConceptGrowX.Location = new System.Drawing.Point(679, 220);
            this.textBoxConceptGrowX.Name = "textBoxConceptGrowX";
            this.textBoxConceptGrowX.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptGrowX.TabIndex = 42;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(565, 224);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 26);
            this.label17.TabIndex = 41;
            this.label17.Text = "Cell Grow Factor:\r\n( Real Number )";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(461, 223);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 13);
            this.label18.TabIndex = 43;
            this.label18.Text = "X:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(261, 223);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 44;
            this.label13.Text = "X:";
            // 
            // textBoxConceptGrowY
            // 
            this.textBoxConceptGrowY.Location = new System.Drawing.Point(679, 250);
            this.textBoxConceptGrowY.Name = "textBoxConceptGrowY";
            this.textBoxConceptGrowY.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptGrowY.TabIndex = 46;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(656, 223);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 13);
            this.label14.TabIndex = 48;
            this.label14.Text = "X:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(656, 251);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 13);
            this.label19.TabIndex = 47;
            this.label19.Text = "Y:";
            // 
            // textBoxConceptHeight
            // 
            this.textBoxConceptHeight.Location = new System.Drawing.Point(838, 219);
            this.textBoxConceptHeight.Name = "textBoxConceptHeight";
            this.textBoxConceptHeight.Size = new System.Drawing.Size(52, 20);
            this.textBoxConceptHeight.TabIndex = 50;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(756, 220);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(81, 26);
            this.label20.TabIndex = 49;
            this.label20.Text = "Height Factor:\r\n( Real Number )";
            // 
            // textBoxSpiralFactorA
            // 
            this.textBoxSpiralFactorA.Location = new System.Drawing.Point(284, 304);
            this.textBoxSpiralFactorA.Name = "textBoxSpiralFactorA";
            this.textBoxSpiralFactorA.Size = new System.Drawing.Size(52, 20);
            this.textBoxSpiralFactorA.TabIndex = 52;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(186, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 26);
            this.label6.TabIndex = 51;
            this.label6.Text = "Factor a:\r\n( Real Number )";
            // 
            // textBoxSpiralFactorB
            // 
            this.textBoxSpiralFactorB.Location = new System.Drawing.Point(479, 305);
            this.textBoxSpiralFactorB.Name = "textBoxSpiralFactorB";
            this.textBoxSpiralFactorB.Size = new System.Drawing.Size(52, 20);
            this.textBoxSpiralFactorB.TabIndex = 54;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(365, 307);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 26);
            this.label7.TabIndex = 53;
            this.label7.Text = "Factor b:\r\n( Real Number )";
            // 
            // textBoxSpiralPoints
            // 
            this.textBoxSpiralPoints.Location = new System.Drawing.Point(679, 304);
            this.textBoxSpiralPoints.Name = "textBoxSpiralPoints";
            this.textBoxSpiralPoints.Size = new System.Drawing.Size(52, 20);
            this.textBoxSpiralPoints.TabIndex = 56;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(565, 302);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 26);
            this.label8.TabIndex = 55;
            this.label8.Text = "Number of Points:\r\n( Integer )";
            // 
            // textBoxSiteSizeY
            // 
            this.textBoxSiteSizeY.Location = new System.Drawing.Point(284, 399);
            this.textBoxSiteSizeY.Name = "textBoxSiteSizeY";
            this.textBoxSiteSizeY.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteSizeY.TabIndex = 60;
            // 
            // textBoxSiteSizeX
            // 
            this.textBoxSiteSizeX.Location = new System.Drawing.Point(284, 368);
            this.textBoxSiteSizeX.Name = "textBoxSiteSizeX";
            this.textBoxSiteSizeX.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteSizeX.TabIndex = 58;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(186, 371);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(72, 26);
            this.label21.TabIndex = 57;
            this.label21.Text = "Size:\r\n( Decimal Ft. )";
            // 
            // textBoxSiteDivisionsX
            // 
            this.textBoxSiteDivisionsX.Location = new System.Drawing.Point(479, 368);
            this.textBoxSiteDivisionsX.Name = "textBoxSiteDivisionsX";
            this.textBoxSiteDivisionsX.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteDivisionsX.TabIndex = 62;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(365, 371);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(69, 26);
            this.label22.TabIndex = 61;
            this.label22.Text = "Subdivisions:\r\n( Integer )";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(261, 371);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(17, 13);
            this.label23.TabIndex = 64;
            this.label23.Text = "X:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(261, 403);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(17, 13);
            this.label24.TabIndex = 63;
            this.label24.Text = "Y:";
            // 
            // textBoxSiteRandom
            // 
            this.textBoxSiteRandom.Location = new System.Drawing.Point(933, 364);
            this.textBoxSiteRandom.Name = "textBoxSiteRandom";
            this.textBoxSiteRandom.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteRandom.TabIndex = 66;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(819, 362);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 26);
            this.label9.TabIndex = 65;
            this.label9.Text = "Randomization Factor:\r\n( Real Number )";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(565, 368);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(93, 26);
            this.label25.TabIndex = 67;
            this.label25.Text = "Corner Elevations:\r\n( Decimal Ft. )";
            // 
            // textBoxSiteElevTopLeft
            // 
            this.textBoxSiteElevTopLeft.Location = new System.Drawing.Point(679, 364);
            this.textBoxSiteElevTopLeft.Name = "textBoxSiteElevTopLeft";
            this.textBoxSiteElevTopLeft.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteElevTopLeft.TabIndex = 68;
            // 
            // textBoxSiteElevBotLeft
            // 
            this.textBoxSiteElevBotLeft.Location = new System.Drawing.Point(679, 394);
            this.textBoxSiteElevBotLeft.Name = "textBoxSiteElevBotLeft";
            this.textBoxSiteElevBotLeft.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteElevBotLeft.TabIndex = 70;
            // 
            // textBoxSiteElevBotRight
            // 
            this.textBoxSiteElevBotRight.Location = new System.Drawing.Point(742, 394);
            this.textBoxSiteElevBotRight.Name = "textBoxSiteElevBotRight";
            this.textBoxSiteElevBotRight.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteElevBotRight.TabIndex = 74;
            // 
            // textBoxSiteElevTopRight
            // 
            this.textBoxSiteElevTopRight.Location = new System.Drawing.Point(742, 364);
            this.textBoxSiteElevTopRight.Name = "textBoxSiteElevTopRight";
            this.textBoxSiteElevTopRight.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteElevTopRight.TabIndex = 72;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(455, 370);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(17, 13);
            this.label26.TabIndex = 76;
            this.label26.Text = "X:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(455, 402);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(17, 13);
            this.label27.TabIndex = 75;
            this.label27.Text = "Y:";
            // 
            // textBoxSiteDivisionsY
            // 
            this.textBoxSiteDivisionsY.Location = new System.Drawing.Point(479, 400);
            this.textBoxSiteDivisionsY.Name = "textBoxSiteDivisionsY";
            this.textBoxSiteDivisionsY.Size = new System.Drawing.Size(52, 20);
            this.textBoxSiteDivisionsY.TabIndex = 77;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 489);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxSiteDivisionsY);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.textBoxSiteElevBotRight);
            this.Controls.Add(this.textBoxSiteElevTopRight);
            this.Controls.Add(this.textBoxSiteElevBotLeft);
            this.Controls.Add(this.textBoxSiteElevTopLeft);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.textBoxSiteRandom);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.textBoxSiteDivisionsX);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.textBoxSiteSizeY);
            this.Controls.Add(this.textBoxSiteSizeX);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.textBoxSpiralPoints);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxSpiralFactorB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxSpiralFactorA);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxConceptHeight);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.textBoxConceptGrowY);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.textBoxConceptGrowX);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.textBoxConceptCellY);
            this.Controls.Add(this.textBoxConceptCellX);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.textBoxConceptGridY);
            this.Controls.Add(this.textBoxConceptGridX);
            this.Controls.Add(this.textBoxTwistTaper);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.checkBoxPreserveId);
            this.Controls.Add(this.textBoxTwistTwist);
            this.Controls.Add(this.textBoxTwistHeight);
            this.Controls.Add(this.textBoxTwistFloors);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonTwistyTower);
            this.Controls.Add(this.buttonHoles);
            this.Controls.Add(this.buttonConceptMass);
            this.Controls.Add(this.buttonSpiral);
            this.Controls.Add(this.buttonSite);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxPathFolder);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hummingbird C# Examples";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxPathFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSite;
        private System.Windows.Forms.Button buttonSpiral;
        private System.Windows.Forms.Button buttonConceptMass;
        private System.Windows.Forms.Button buttonHoles;
        private System.Windows.Forms.Button buttonTwistyTower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTwistFloors;
        private System.Windows.Forms.TextBox textBoxTwistHeight;
        private System.Windows.Forms.TextBox textBoxTwistTwist;
        private System.Windows.Forms.CheckBox checkBoxPreserveId;
        private System.Windows.Forms.TextBox textBoxTwistTaper;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxConceptGridX;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxConceptGridY;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxConceptCellY;
        private System.Windows.Forms.TextBox textBoxConceptCellX;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxConceptGrowX;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxConceptGrowY;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxConceptHeight;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBoxSpiralFactorA;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxSpiralFactorB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxSpiralPoints;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSiteSizeY;
        private System.Windows.Forms.TextBox textBoxSiteSizeX;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBoxSiteDivisionsX;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBoxSiteRandom;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox textBoxSiteElevTopLeft;
        private System.Windows.Forms.TextBox textBoxSiteElevBotLeft;
        private System.Windows.Forms.TextBox textBoxSiteElevBotRight;
        private System.Windows.Forms.TextBox textBoxSiteElevTopRight;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox textBoxSiteDivisionsY;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

