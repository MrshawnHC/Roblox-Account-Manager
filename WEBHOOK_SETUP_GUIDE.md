# Discord Webhook Setup Guide for RBX Alt Manager

## 🎯 Quick Start - Copy & Paste System

This guide will help you set up Discord webhooks to receive notifications from your RBX Alt Manager.

---

## 📋 Step 1: Create a Discord Webhook URL

### In Your Discord Server:

1. **Open Discord** and go to your server
2. **Right-click** on the channel where you want notifications
3. Click **"Edit Channel"** (⚙️ icon)
4. Go to **"Integrations"** in the left sidebar
5. Click **"Webhooks"**
6. Click **"New Webhook"** button
7. Give it a name (e.g., "RBX Alt Manager")
8. **Copy the Webhook URL** - This is your unique URL!

⚠️ **IMPORTANT:** Keep your webhook URL private! Anyone with this URL can send messages to your channel.

---

## 🔧 Step 2: Configure in RBX Alt Manager

### Using the UI (Recommended):

1. Open **RBX Alt Manager**
2. Look for **"Webhook Settings"** or **"Discord Integration"** in the menu
3. **Paste your webhook URL** into the designated field
4. Set your preferred **bot username** (default: "RBX Alt Manager")
5. Enable/disable notification types:
   - ✅ Auto-log important events
   - ✅ Notify on successful operations
   - ✅ Notify on failed operations
   - ⏱️ Batch report interval (seconds)
6. Click **"Test Webhook"** to verify it works
7. Click **"Save Settings"**

### Manual Configuration (INI File):

Edit `RAMSettings.ini` and add:

```ini
[General]
DiscordWebhookURL=https://discord.com/api/webhooks/YOUR_WEBHOOK_URL_HERE
DiscordWebhookUsername=RBX Alt Manager
WebhookEnabled=true
WebhookAutoLog=true
WebhookNotifySuccess=true
WebhookNotifyFail=true
WebhookBatchInterval=60
```

---

## 💻 Step 3: Use Webhooks in Your Code

### Basic Usage - Send a Message:

```csharp
// Initialize once at startup
WebhookManager.Initialize("YOUR_WEBHOOK_URL", "My Bot Name");

// Send a simple message
await WebhookManager.QuickSendAsync("Account loaded successfully!");
```

### Advanced Usage - Custom Notifications:

```csharp
// Create a webhook instance
var webhook = new DiscordWebhook("YOUR_WEBHOOK_URL")
{
    Username = "RBX Manager",
    Enabled = true
};

// Send account notification
await webhook.SendAccountNotificationAsync(
    username: "Player123",
    action: "Successfully joined game",
    details: "Place ID: 123456789"
);

// Send log message
await webhook.SendLogAsync("INFO", "Batch operation started");

// Send batch report
await webhook.SendBatchReportAsync(
    total: 100,
    success: 95,
    failed: 5,
    batchName: "Morning Batch"
);

// Send custom embed
await webhook.SendEmbedAsync(
    title: "🎉 Event Complete!",
    description: "All accounts processed successfully",
    color: Color.Green,
    fields: new[] {
        new EmbedField { name = "Total", value = "100", inline = true },
        new EmbedField { name = "Success Rate", value = "95%", inline = true }
    }
);
```

---

## 🎨 Available Colors

Use these predefined colors for your embeds:

- `Color.Red` - Errors, failures
- `Color.Green` - Success, online status
- `Color.Blue` - Info, general notifications
- `Color.Orange` - Warnings
- `Color.Purple` - Special events
- `Color.Yellow` - Pending, processing

Or create custom colors:
```csharp
var customColor = new Color(255, 128, 0); // RGB values
```

---

## 📊 Example Notification Types

### Account Login Success:
```
✅ Account: Player123
Action: Logged In Successfully
Details: Joined server at 10:30 AM
```

### Batch Operation Report:
```
📦 Batch Operation
Batch completed with 95/100 successful
├─ Total Accounts: 100
├─ Successful: 95
├─ Failed: 5
└─ Success Rate: 95%
```

### Error Notification:
```
❌ Log: ERROR
Type: ERROR
Message: Failed to join game - Connection timeout
```

---

## ⚙️ Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `DiscordWebhookURL` | Your Discord webhook URL | Empty |
| `DiscordWebhookUsername` | Bot display name | "RBX Alt Manager" |
| `WebhookEnabled` | Enable/disable all notifications | true |
| `WebhookAutoLog` | Auto-log important events | true |
| `WebhookNotifySuccess` | Send success notifications | true |
| `WebhookNotifyFail` | Send failure notifications | true |
| `WebhookBatchInterval` | Seconds between batch reports | 60 |

---

## 🔒 Security Best Practices

1. **Never share your webhook URL publicly**
2. **Regenerate webhook** if URL is compromised (delete and create new)
3. **Use separate channels** for different notification types
4. **Enable rate limiting** in your Discord server settings
5. **Monitor webhook activity** in Discord's audit log

---

## 🛠️ Troubleshooting

### Webhook Not Working?

1. **Check URL**: Ensure the webhook URL is correct and complete
2. **Test Connection**: Use the "Test Webhook" button in the UI
3. **Permissions**: Verify the webhook has permission to send messages
4. **Channel Access**: Make sure the channel hasn't been deleted
5. **Rate Limits**: Discord limits webhooks to 30 requests/minute

### Common Errors:

- **403 Forbidden**: Webhook URL is invalid or expired
- **429 Too Many Requests**: You're sending too fast, increase intervals
- **500 Internal Error**: Discord API issue, wait and retry

---

## 📞 Support

- Join the official Discord: https://discord.gg/MsEH7smXY8
- Report issues on GitHub
- Check the README.md for more information

---

## 📝 License

This webhook integration is part of RBX Alt Manager. See LICENSE file for details.
