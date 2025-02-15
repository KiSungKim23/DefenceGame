
namespace ExcelConverter
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.convertButton = new System.Windows.Forms.Button();
            this.excelPath = new System.Windows.Forms.TextBox();
            this.clientCodePath = new System.Windows.Forms.TextBox();
            this.serverCodePath = new System.Windows.Forms.TextBox();
            this.excelPathLabel = new System.Windows.Forms.Label();
            this.clientCodePathLabel = new System.Windows.Forms.Label();
            this.serverCodePathLabel = new System.Windows.Forms.Label();
            this.PathSetButton = new System.Windows.Forms.Button();
            this.excelCountLabel = new System.Windows.Forms.Label();
            this.excelCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(604, 116);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(161, 21);
            this.convertButton.TabIndex = 0;
            this.convertButton.Text = "convertButton";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // jsonPath
            // 
            this.excelPath.Location = new System.Drawing.Point(45, 58);
            this.excelPath.Name = "excelPath";
            this.excelPath.Size = new System.Drawing.Size(525, 21);
            this.excelPath.TabIndex = 1;
            // 
            // clientCodePath
            // 
            this.clientCodePath.Location = new System.Drawing.Point(45, 117);
            this.clientCodePath.Name = "clientCodePath";
            this.clientCodePath.Size = new System.Drawing.Size(525, 21);
            this.clientCodePath.TabIndex = 3;
            // 
            // serverCodePath
            // 
            this.serverCodePath.Location = new System.Drawing.Point(45, 179);
            this.serverCodePath.Name = "serverCodePath";
            this.serverCodePath.Size = new System.Drawing.Size(525, 21);
            this.serverCodePath.TabIndex = 5;
            // 
            // excelPathLabel
            // 
            this.excelPathLabel.AutoSize = true;
            this.excelPathLabel.Location = new System.Drawing.Point(43, 34);
            this.excelPathLabel.Name = "excelPathLabel";
            this.excelPathLabel.Size = new System.Drawing.Size(65, 12);
            this.excelPathLabel.TabIndex = 6;
            this.excelPathLabel.Text = "Excel 경로";
            // 
            // clientCodePathLabel
            // 
            this.clientCodePathLabel.AutoSize = true;
            this.clientCodePathLabel.Location = new System.Drawing.Point(45, 94);
            this.clientCodePathLabel.Name = "clientCodePathLabel";
            this.clientCodePathLabel.Size = new System.Drawing.Size(95, 12);
            this.clientCodePathLabel.TabIndex = 7;
            this.clientCodePathLabel.Text = "ClientCode 경로";
            // 
            // serverCodePathLabel
            // 
            this.serverCodePathLabel.AutoSize = true;
            this.serverCodePathLabel.Location = new System.Drawing.Point(45, 157);
            this.serverCodePathLabel.Name = "serverCodePathLabel";
            this.serverCodePathLabel.Size = new System.Drawing.Size(99, 12);
            this.serverCodePathLabel.TabIndex = 8;
            this.serverCodePathLabel.Text = "ServerCode 경로";
            // 
            // PathSetButton
            // 
            this.PathSetButton.Location = new System.Drawing.Point(604, 58);
            this.PathSetButton.Name = "PathSetButton";
            this.PathSetButton.Size = new System.Drawing.Size(161, 21);
            this.PathSetButton.TabIndex = 9;
            this.PathSetButton.Text = "PathSet";
            this.PathSetButton.UseVisualStyleBackColor = true;
            this.PathSetButton.Click += new System.EventHandler(this.PathSetButton_Click);
            // 
            // excelCountLabel
            // 
            this.excelCountLabel.AutoSize = true;
            this.excelCountLabel.Location = new System.Drawing.Point(590, 296);
            this.excelCountLabel.Name = "excelCountLabel";
            this.excelCountLabel.Size = new System.Drawing.Size(81, 12);
            this.excelCountLabel.TabIndex = 10;
            this.excelCountLabel.Text = "excelCount : ";
            // 
            // excelCount
            // 
            this.excelCount.AutoSize = true;
            this.excelCount.Location = new System.Drawing.Point(707, 296);
            this.excelCount.Name = "excelCount";
            this.excelCount.Size = new System.Drawing.Size(11, 12);
            this.excelCount.TabIndex = 11;
            this.excelCount.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.excelCount);
            this.Controls.Add(this.excelCountLabel);
            this.Controls.Add(this.PathSetButton);
            this.Controls.Add(this.serverCodePathLabel);
            this.Controls.Add(this.clientCodePathLabel);
            this.Controls.Add(this.excelPathLabel);
            this.Controls.Add(this.serverCodePath);
            this.Controls.Add(this.clientCodePath);
            this.Controls.Add(this.excelPath);
            this.Controls.Add(this.convertButton);
            this.Name = "Form1";
            this.Text = "ExcelConverter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.TextBox excelPath;
        private System.Windows.Forms.TextBox clientCodePath;
        private System.Windows.Forms.TextBox serverCodePath;
        private System.Windows.Forms.Label excelPathLabel;
        private System.Windows.Forms.Label clientCodePathLabel;
        private System.Windows.Forms.Label serverCodePathLabel;
        private System.Windows.Forms.Button PathSetButton;
        private System.Windows.Forms.Label excelCountLabel;
        private System.Windows.Forms.Label excelCount;
    }
}

