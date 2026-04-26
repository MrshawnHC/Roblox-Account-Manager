using RBX_Alt_Manager.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RBX_Alt_Manager.Forms
{
    public class WebhookUI : Form
    {
        private TextBox webhookUrlTextBox;
        private TextBox usernameTextBox;
        private CheckBox enabledCheckBox;
        private Button testButton;
        private Button saveButton;
        private Button cancelButton;
        private Label statusLabel;
        private GroupBox configGroupBox;
        private FlowLayoutPanel buttonPanel;
        private RichTextBox logBox;
        private TabControl mainTabControl;
        private TabPage configTab;
        private TabPage logsTab;
        private CheckBox autoLogCheckBox;
        private CheckBox notifyOnSuccessCheckBox;
        private CheckBox notifyOnFailCheckBox;
        private NumericUpDown batchReportInterval;
        private Label intervalLabel;

        public WebhookUI()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Discord Webhook Manager - RBX Alt Manager";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            AccountManager.SetDarkBar(this.Handle);

            mainTabControl = new TabControl { Dock = DockStyle.Fill, Margin = new Padding(10) };
            configTab = new TabPage("Configuration");
            configTab.Padding = new Padding(15);
            logsTab = new TabPage("Activity Logs");
            logsTab.Padding = new Padding(15);

            configGroupBox = new GroupBox
            {
                Text = "Webhook Configuration",
                Dock = DockStyle.Top,
                Height = 280,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 10)
            };

            int yPos = 25, labelWidth = 140, inputWidth = 450, inputHeight = 28;

            Label urlLabel = new Label { Text = "Webhook URL:", Location = new Point(15, yPos), Size = new Size(labelWidth, 20), TextAlign = ContentAlignment.MiddleRight };
            configGroupBox.Controls.Add(urlLabel);

            webhookUrlTextBox = new TextBox
            {
                Location = new Point(labelWidth + 20, yPos),
                Size = new Size(inputWidth, inputHeight),
                PasswordChar = '\u25CF',
                Font = new Font("Segoe UI", 9F),
                Tag = "Enter your Discord webhook URL here"
            };
            webhookUrlTextBox.GotFocus += (s, e) => { if (webhookUrlTextBox.Tag.ToString() == "Enter your Discord webhook URL here") webhookUrlTextBox.Clear(); };
            webhookUrlTextBox.LostFocus += (s, e) => { if (string.IsNullOrEmpty(webhookUrlTextBox.Text)) webhookUrlTextBox.Text = "Enter your Discord webhook URL here"; };
            configGroupBox.Controls.Add(webhookUrlTextBox);

            yPos += 40;
            Label userLabel = new Label { Text = "Bot Username:", Location = new Point(15, yPos), Size = new Size(labelWidth, 20), TextAlign = ContentAlignment.MiddleRight };
            configGroupBox.Controls.Add(userLabel);

            usernameTextBox = new TextBox { Location = new Point(labelWidth + 20, yPos), Size = new Size(inputWidth, inputHeight), Text = "RBX Alt Manager", Font = new Font("Segoe UI", 9F) };
            configGroupBox.Controls.Add(usernameTextBox);

            yPos += 40;
            enabledCheckBox = new CheckBox { Text = "Enable Webhook Notifications", Location = new Point(15, yPos), Size = new Size(300, 25), Checked = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            configGroupBox.Controls.Add(enabledCheckBox);

            yPos += 35;
            autoLogCheckBox = new CheckBox { Text = "Auto-log important events", Location = new Point(15, yPos), Size = new Size(250, 22), Checked = true };
            configGroupBox.Controls.Add(autoLogCheckBox);

            yPos += 30;
            notifyOnSuccessCheckBox = new CheckBox { Text = "Notify on successful operations", Location = new Point(15, yPos), Size = new Size(250, 22), Checked = true };
            configGroupBox.Controls.Add(notifyOnSuccessCheckBox);

            yPos += 30;
            notifyOnFailCheckBox = new CheckBox { Text = "Notify on failed operations", Location = new Point(15, yPos), Size = new Size(250, 22), Checked = true };
            configGroupBox.Controls.Add(notifyOnFailCheckBox);

            yPos += 35;
            intervalLabel = new Label { Text = "Batch Report Interval (sec):", Location = new Point(15, yPos), Size = new Size(labelWidth, 20), TextAlign = ContentAlignment.MiddleRight };
            configGroupBox.Controls.Add(intervalLabel);

            batchReportInterval = new NumericUpDown { Location = new Point(labelWidth + 20, yPos), Size = new Size(100, inputHeight), Minimum = 0, Maximum = 3600, Value = 60, Increment = 10 };
            configGroupBox.Controls.Add(batchReportInterval);

            Panel instructionsPanel = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.FromArgb(30, 30, 30) };
            Label instructionsLabel = new Label
            {
                Text = "How to get your Webhook URL:\n1. Go to your Discord Server Settings\n2. Navigate to Integrations -> Webhooks\n3. Click 'New Webhook' and copy the URL\n4. Paste it in the field above",
                Location = new Point(15, 10),
                Size = new Size(580, 75),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8.5F)
            };
            instructionsPanel.Controls.Add(instructionsLabel);
            configGroupBox.Controls.Add(instructionsPanel);
            configTab.Controls.Add(configGroupBox);

            buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10) };
            cancelButton = new Button { Text = "Cancel", Size = new Size(100, 35), DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat };
            cancelButton.FlatAppearance.BorderSize = 0;
            buttonPanel.Controls.Add(cancelButton);

            testButton = new Button { Text = "Test Webhook", Size = new Size(130, 35), BackColor = Color.FromArgb(88, 101, 242), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            testButton.FlatAppearance.BorderSize = 0;
            testButton.Click += TestButton_Click;
            buttonPanel.Controls.Add(testButton);

            saveButton = new Button { Text = "Save Settings", Size = new Size(130, 35), BackColor = Color.FromArgb(67, 181, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            buttonPanel.Controls.Add(saveButton);
            configTab.Controls.Add(buttonPanel);

            statusLabel = new Label { Dock = DockStyle.Top, Height = 30, Text = "Status: Ready", TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.LightGreen };
            configTab.Controls.Add(statusLabel);

            logBox = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.FromArgb(20, 20, 20), ForeColor = Color.LightGray, Font = new Font("Consolas", 9F), BorderStyle = BorderStyle.None };
            logsTab.Controls.Add(logBox);

            mainTabControl.TabPages.Add(configTab);
            mainTabControl.TabPages.Add(logsTab);
            this.Controls.Add(mainTabControl);
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.FromArgb(45, 45, 45);
            this.ForeColor = Color.White;
            mainTabControl.BackColor = Color.FromArgb(45, 45, 45);
            mainTabControl.ForeColor = Color.White;
            foreach (TabPage tab in mainTabControl.TabPages) { tab.BackColor = Color.FromArgb(45, 45, 45); tab.ForeColor = Color.White; }
            configGroupBox.ForeColor = Color.White;
            configGroupBox.BackColor = Color.FromArgb(50, 50, 50);
            foreach (Control ctrl in configGroupBox.Controls)
            {
                if (ctrl is TextBox tb) { tb.BackColor = Color.FromArgb(35, 35, 35); tb.ForeColor = Color.White; tb.BorderStyle = BorderStyle.FixedSingle; }
                else if (ctrl is CheckBox cb) { cb.ForeColor = Color.White; cb.BackColor = Color.Transparent; }
                else if (ctrl is Label lbl) { lbl.ForeColor = Color.White; lbl.BackColor = Color.Transparent; }
                else if (ctrl is NumericUpDown nud) { nud.BackColor = Color.FromArgb(35, 35, 35); nud.ForeColor = Color.White; }
            }
        }

        private void LoadSettings()
        {
            string savedUrl = AccountManager.General.Get("DiscordWebhookURL");
            string savedUsername = AccountManager.General.Get("DiscordWebhookUsername");
            if (!string.IsNullOrEmpty(savedUrl) && savedUrl != "DiscordWebhookURL") { webhookUrlTextBox.Text = savedUrl; webhookUrlTextBox.PasswordChar = '\u25CF'; }
            if (!string.IsNullOrEmpty(savedUsername)) usernameTextBox.Text = savedUsername;
            enabledCheckBox.Checked = AccountManager.General.Get<bool>("WebhookEnabled");
            autoLogCheckBox.Checked = AccountManager.General.Get<bool>("WebhookAutoLog");
            notifyOnSuccessCheckBox.Checked = AccountManager.General.Get<bool>("WebhookNotifySuccess");
            notifyOnFailCheckBox.Checked = AccountManager.General.Get<bool>("WebhookNotifyFail");
            batchReportInterval.Value = (decimal)AccountManager.General.Get<int>("WebhookBatchInterval");
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            string url = webhookUrlTextBox.Text.Trim();
            if (string.IsNullOrEmpty(url) || url == "Enter your Discord webhook URL here") { statusLabel.Text = "Status: Please enter a valid webhook URL"; statusLabel.ForeColor = Color.Orange; return; }
            testButton.Enabled = false; testButton.Text = "Testing..."; statusLabel.Text = "Status: Sending test message..."; statusLabel.ForeColor = Color.Yellow;
            try
            {
                var webhook = new DiscordWebhook(url) { Username = usernameTextBox.Text, Enabled = true };
                await webhook.SendEmbedAsync("Webhook Test Successful!", "Your Discord webhook is configured correctly.", Color.Green,
                    new EmbedField { name = "Timestamp", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), inline = true },
                    new EmbedField { name = "Manager", value = "RBX Alt Manager", inline = true });
                statusLabel.Text = "Status: Test successful!"; statusLabel.ForeColor = Color.LightGreen; AppendLog("Test webhook sent successfully");
            }
            catch (Exception ex) { statusLabel.Text = "Status: Test failed"; statusLabel.ForeColor = Color.Red; AppendLog($"Test failed: {ex.Message}"); MessageBox.Show($"Failed to send test message:\n{ex.Message}", "Webhook Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { testButton.Enabled = true; testButton.Text = "Test Webhook"; }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string url = webhookUrlTextBox.Text.Trim();
            if (string.IsNullOrEmpty(url) || url == "Enter your Discord webhook URL here") { MessageBox.Show("Please enter a valid webhook URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            AccountManager.General.Set("DiscordWebhookURL", url);
            AccountManager.General.Set("DiscordWebhookUsername", usernameTextBox.Text);
            AccountManager.General.Set("WebhookEnabled", enabledCheckBox.Checked ? "true" : "false");
            AccountManager.General.Set("WebhookAutoLog", autoLogCheckBox.Checked ? "true" : "false");
            AccountManager.General.Set("WebhookNotifySuccess", notifyOnSuccessCheckBox.Checked ? "true" : "false");
            AccountManager.General.Set("WebhookNotifyFail", notifyOnFailCheckBox.Checked ? "true" : "false");
            AccountManager.General.Set("WebhookBatchInterval", ((int)batchReportInterval.Value).ToString());
            AccountManager.IniSettings.Save("RAMSettings.ini");
            WebhookManager.Initialize(url, usernameTextBox.Text);
            statusLabel.Text = "Status: Settings saved successfully!"; statusLabel.ForeColor = Color.LightGreen; AppendLog("Webhook settings saved");
            MessageBox.Show("Webhook settings saved successfully!\n\nYou can now use webhooks throughout RBX Alt Manager.", "Settings Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK; this.Close();
        }

        private void AppendLog(string message) { string timestamp = DateTime.Now.ToString("[HH:mm:ss] "); logBox.AppendText(timestamp + message + Environment.NewLine); logBox.ScrollToCaret(); }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK) { var result = MessageBox.Show("Discard changes?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (result == DialogResult.No) e.Cancel = true; }
            base.OnFormClosing(e);
        }
    }
}
