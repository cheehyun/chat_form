namespace WinFormsApp1

{
    partial class Form1
    {

        /// <summary>

        ///  Required designer variable.

        /// </summary>

        private System.ComponentModel.IContainer components = null;



        /// <summary>

        ///  Clean up any resources being used.

        /// </summary>

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {

                components.Dispose();
            }
            base.Dispose(disposing);
        }



        #region Windows Form Designer generated code



        /// <summary>

        ///  Required method for Designer support - do not modify

        ///  the contents of this method with the code editor.

        /// </summary>

        private void InitializeComponent()
        {
            txtChatMsg = new TextBox();
            btnStart = new Button();
            lblMsg = new Label();
            SuspendLayout();
            // 
            // txtChatMsg
            // 
            txtChatMsg.Location = new Point(35, 20);
            txtChatMsg.Margin = new Padding(4, 4, 4, 4);
            txtChatMsg.Multiline = true;
            txtChatMsg.Name = "txtChatMsg";
            txtChatMsg.ScrollBars = ScrollBars.Vertical;
            txtChatMsg.Size = new Size(602, 407);
            txtChatMsg.TabIndex = 0;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("맑은 고딕", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            btnStart.Location = new Point(409, 445);
            btnStart.Margin = new Padding(4, 4, 4, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(228, 50);
            btnStart.TabIndex = 1;
            btnStart.Tag = "Stop";
            btnStart.Text = "서버 시작";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblMsg.Location = new Point(35, 445);
            lblMsg.Margin = new Padding(4, 0, 4, 0);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(197, 37);
            lblMsg.TabIndex = 2;
            lblMsg.Tag = "Stop";
            lblMsg.Text = "Server 중지 됨";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(670, 511);
            Controls.Add(lblMsg);
            Controls.Add(btnStart);
            Controls.Add(txtChatMsg);
            Margin = new Padding(4, 4, 4, 4);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion



        private TextBox txtChatMsg;

        private Button btnStart;

        private Label lblMsg;

    }
}