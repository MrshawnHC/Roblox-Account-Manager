using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RBX_Alt_Manager.Classes
{
    /// <summary>
    /// Discord Webhook Integration for RBX Alt Manager
    /// Easily send notifications, logs, and account events to Discord
    /// </summary>
    public class DiscordWebhook
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        /// <summary>
        /// Webhook URL from Discord Server Settings
        /// </summary>
        public string WebhookUrl { get; set; }
        
        /// <summary>
        /// Username displayed in Discord (default: RBX Alt Manager)
        /// </summary>
        public string Username { get; set; } = "RBX Alt Manager";
        
        /// <summary>
        /// Avatar URL for the webhook bot
        /// </summary>
        public string AvatarUrl { get; set; } = "https://i.imgur.com/default.png";
        
        /// <summary>
        /// Enable/Disable webhook sending
        /// </summary>
        public bool Enabled { get; set; } = true;

        public DiscordWebhook(string webhookUrl)
        {
            WebhookUrl = webhookUrl;
        }

        /// <summary>
        /// Send a simple message to Discord
        /// </summary>
        public async Task SendMessageAsync(string content)
        {
            if (!Enabled || string.IsNullOrEmpty(WebhookUrl))
                return;

            var payload = new
            {
                username = Username,
                avatar_url = AvatarUrl,
                content = content
            };

            await SendPayloadAsync(payload);
        }

        /// <summary>
        /// Send an embed message with rich formatting
        /// </summary>
        public async Task SendEmbedAsync(string title, string description, Color color, params EmbedField[] fields)
        {
            if (!Enabled || string.IsNullOrEmpty(WebhookUrl))
                return;

            var embed = new
            {
                title = title,
                description = description,
                color = ((int)color.R << 16) | ((int)color.G << 8) | (int)color.B,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                fields = fields
            };

            var payload = new
            {
                username = Username,
                avatar_url = AvatarUrl,
                embeds = new[] { embed }
            };

            await SendPayloadAsync(payload);
        }

        /// <summary>
        /// Send account-related notification
        /// </summary>
        public async Task SendAccountNotificationAsync(string username, string action, string details = "")
        {
            Color color = action.ToLower().Contains("success") || action.ToLower().Contains("online") ? Color.Green :
                         action.ToLower().Contains("error") || action.ToLower().Contains("fail") ? Color.Red : Color.Blue;

            var fields = new[]
            {
                new EmbedField { name = "Username", value = username, inline = true },
                new EmbedField { name = "Action", value = action, inline = true },
                new EmbedField { name = "Details", value = string.IsNullOrEmpty(details) ? "N/A" : details, inline = false }
            };

            await SendEmbedAsync($"Account: {username}", $"{action}", color, fields);
        }

        /// <summary>
        /// Send log message
        /// </summary>
        public async Task SendLogAsync(string logType, string message)
        {
            Color color = logType.ToLower() switch
            {
                "error" => Color.Red,
                "warning" => Color.Orange,
                "success" => Color.Green,
                _ => Color.Blue
            };

            var fields = new[]
            {
                new EmbedField { name = "Type", value = logType, inline = true },
                new EmbedField { name = "Message", value = message, inline = false }
            };

            await SendEmbedAsync($"Log: {logType}", message, color, fields);
        }

        /// <summary>
        /// Send batch status report
        /// </summary>
        public async Task SendBatchReportAsync(int total, int success, int failed, string batchName = "Batch Operation")
        {
            var fields = new[]
            {
                new EmbedField { name = "Total Accounts", value = total.ToString(), inline = true },
                new EmbedField { name = "Successful", value = success.ToString(), inline = true },
                new EmbedField { name = "Failed", value = failed.ToString(), inline = true },
                new EmbedField { name = "Success Rate", value = $"{(total > 0 ? (success * 100 / total) : 0)}%", inline = false }
            };

            Color color = failed == 0 ? Color.Green : failed < total / 2 ? Color.Orange : Color.Red;

            await SendEmbedAsync(batchName, $"Batch completed with {success}/{total} successful", color, fields);
        }

        private async Task SendPayloadAsync(object payload)
        {
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(WebhookUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    Program.Logger.Error($"Discord Webhook failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error($"Discord Webhook Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Color struct for Discord embeds
    /// </summary>
    public struct Color
    {
        public byte R, G, B;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static Color Red => new Color(255, 69, 58);
        public static Color Green => new Color(75, 219, 106);
        public static Color Blue => new Color(59, 142, 234);
        public static Color Orange => new Color(255, 149, 0);
        public static Color Purple => new Color(175, 82, 222);
        public static Color Yellow => new Color(255, 204, 0);
    }

    /// <summary>
    /// Embed field for Discord messages
    /// </summary>
    public class EmbedField
    {
        public string name { get; set; }
        public string value { get; set; }
        public bool inline { get; set; } = false;
    }

    /// <summary>
    /// Static manager for easy webhook access throughout the application
    /// </summary>
    public static class WebhookManager
    {
        private static DiscordWebhook _webhook;
        private static readonly object _lock = new object();

        public static DiscordWebhook Instance
        {
            get
            {
                if (_webhook == null)
                {
                    lock (_lock)
                    {
                        if (_webhook == null)
                        {
                            string url = AccountManager.General.Get("DiscordWebhookURL");
                            _webhook = new DiscordWebhook(url)
                            {
                                Enabled = !string.IsNullOrEmpty(url),
                                Username = AccountManager.General.Get("DiscordWebhookUsername") ?? "RBX Alt Manager"
                            };
                        }
                    }
                }
                return _webhook;
            }
        }

        public static void Initialize(string webhookUrl, string username = "RBX Alt Manager")
        {
            lock (_lock)
            {
                _webhook = new DiscordWebhook(webhookUrl)
                {
                    Enabled = !string.IsNullOrEmpty(webhookUrl),
                    Username = username
                };
            }
        }

        public static async Task QuickSendAsync(string message)
        {
            if (Instance.Enabled)
                await Instance.SendMessageAsync(message);
        }
    }
}
