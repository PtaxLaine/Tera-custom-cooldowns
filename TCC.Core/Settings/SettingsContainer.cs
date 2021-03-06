﻿using Newtonsoft.Json;
using Nostrum;
using Nostrum.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TeraPacketParser;
using Key = System.Windows.Forms.Keys;

namespace TCC.Settings
{
    public class SettingsContainer
    {
        private string _lastLanguage;
        private bool _chatEnabled;
        private bool _enableProxy;

        #region Misc
        // Misc
        public DateTime LastRun { get; set; }
        public string LastLanguage
        {
            get => LanguageOverride != LanguageOverride.None ? LanguageOverride.GetDescription() : _lastLanguage;
            set => _lastLanguage = value;
        }
        public System.Drawing.Size LastScreenSize { get; set; }
        public DateTime StatSentTime { get; set; }
        public string StatSentVersion { get; set; }
        public LanguageOverride LanguageOverride { get; set; }
        public bool ShowTradeLfg { get; set; }
        public bool UseHotkeys { get; set; }
        public bool EthicalMode { get; set; }
        public bool HideHandles { get; set; }
        public bool HighPriority { get; set; }
        public bool ForceSoftwareRendering { get; set; }
        public bool Npcap { get; set; } //TODO: remove
        public bool ShowIngameChat { get; set; }
        public bool CheckOpcodesHash { get; set; }
        public bool CheckGuildBamWithoutOpcode { get; set; } //by HQ 20190324
        public bool BetaNotification { get; set; }
        public bool FpsAtGuardian { get; set; }
        public bool EnableProxy
        {
            get => _enableProxy || App.ToolboxMode;
            set => _enableProxy = value;
        }

        public bool DontShowFUBH { get; set; }
        public ControlShape AbnormalityShape { get; set; }
        public ControlShape SkillShape { get; set; }
        public CaptureMode CaptureMode { get; set; }
        public MentionMode MentionMode { get; set; }

        #endregion

        #region Hotkeys
        public HotKey LfgHotkey { get; set; }
        public HotKey DashboardHotkey { get; set; }
        public HotKey SettingsHotkey { get; set; }
        public HotKey SkillSettingsHotkey { get; set; }
        public HotKey ForceClickableChatHotkey { get; set; }
        public HotKey ReturnToLobbyHotkey { get; set; }
        public HotKey ToggleBoundariesHotkey { get; set; }
        public HotKey ToggleHideAllHotkey { get; set; }
        public HotKey AbnormalSettingsHotkey { get; set; }

        #endregion

        #region Webhooks
        public bool WebhookEnabledFieldBoss { get; set; }
        public string WebhookUrlFieldBoss { get; set; }
        public string WebhookMessageFieldBossSpawn { get; set; }
        public string WebhookMessageFieldBossDie { get; set; }
        public bool WebhookEnabledGuildBam { get; set; }
        public string WebhookUrlGuildBam { get; set; }
        public string WebhookMessageGuildBam { get; set; }
        public bool WebhookEnabledMentions { get; set; }
        public string WebhookUrlMentions { get; set; }
        #endregion

        public CooldownWindowSettings CooldownWindowSettings { get; set; }
        public CharacterWindowSettings CharacterWindowSettings { get; set; }
        public NpcWindowSettings NpcWindowSettings { get; set; }
        public BuffWindowSettings BuffWindowSettings { get; set; }
        public ClassWindowSettings ClassWindowSettings { get; set; }
        public GroupWindowSettings GroupWindowSettings { get; set; }
        public FlightWindowSettings FlightGaugeWindowSettings { get; set; }
        public FloatingButtonWindowSettings FloatingButtonSettings { get; set; }
        public CivilUnrestWindowSettings CivilUnrestWindowSettings { get; set; }
        public LfgWindowSettings LfgWindowSettings { get; set; }
        public NotificationAreaSettings NotificationAreaSettings { get; set; }
        public PerfMonitorSettings PerfMonitorSettings { get; set; }

        #region Chat
        public TSObservableCollection<ChatWindowSettings> ChatWindowsSettings { get; }
        public WindowSettingsBase ChatSettings { get; private set; } // added to have the EnabledChanged event
        public bool ChatEnabled
        {
            get => ChatWindowsSettings.Count > 0 ? ChatWindowsSettings[0].Enabled : _chatEnabled;
            set
            {
                ChatSettings.Enabled = value;
                if (ChatWindowsSettings.Count > 0)
                {
                    if (ChatWindowsSettings[0].Enabled == value) return;
                    ChatWindowsSettings.ToList().ForEach(x => x.Enabled = value);
                }
                else
                {
                    _chatEnabled = value;
                }
            }
        }
        public bool DisableLfgChatMessages { get; set; }
        public bool ShowChannel { get; set; }
        public bool ShowTimestamp { get; set; }
        public bool ChatTimestampSeconds { get; set; }
        public bool AnimateChatMessages { get; set; }
        public int MaxMessages { get; set; }
        public int SpamThreshold { get; set; }
        public int FontSize { get; set; }
        public int ChatScrollAmount { get; set; }
        public List<string> UserExcludedSysMsg { get; set; }
        #endregion

        #region Twitch
        [JsonIgnore]
        public string TwitchName { get; set; } //TODO: re-add this
        [JsonIgnore]
        public string TwitchToken { get; set; } //TODO: re-add this
        [JsonIgnore]
        public string TwitchChannelName { get; set; } //TODO: re-add this 
        #endregion

        public static string SettingsOverride { get; set; } = "";
        public string LastAccountNameHash { get; set; } = "";
        public bool BackgroundNotifications { get; set; }
        public bool EnablePlayerMenu { get; set; }
        public bool ShowDecimalsInCooldowns { get; set; }
        //public bool ShowConsole { get; set; }


        public SettingsContainer()
        {
            MessageFactory.ReleaseVersionChanged += OnReleaseVersionChanged;

            StatSentVersion = App.AppVersion;
            _lastLanguage = "";
            StatSentTime = DateTime.MinValue;
            LastScreenSize = FocusManager.TeraScreen.Bounds.Size;
            CooldownWindowSettings = new CooldownWindowSettings(/*.4, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CooldownWindowSettings)*/);
            CharacterWindowSettings = new CharacterWindowSettings(/*.4, 1, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CharacterWindowSettings)*/);
            NpcWindowSettings = new NpcWindowSettings(/*.4, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(NpcWindowSettings)*/);
            BuffWindowSettings = new BuffWindowSettings(/*1, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(BuffWindowSettings)*/);
            ClassWindowSettings = new ClassWindowSettings(/*.25, .6, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(ClassWindowSettings)*/);
            GroupWindowSettings = new GroupWindowSettings(/*0, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(GroupWindowSettings)*/);
            FlightGaugeWindowSettings = new FlightWindowSettings(/*0, 0, 0, 0, true, ClickThruMode.Always, 1, false, 1, false, true, false*/);
            FloatingButtonSettings = new FloatingButtonWindowSettings(/*0, 0, 0, 0, true, ClickThruMode.Never, 1, false, 1, false, true, true*/);
            CivilUnrestWindowSettings = new CivilUnrestWindowSettings(/*1, .45, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CivilUnrestWindowSettings)*/);
            ChatWindowsSettings = new TSObservableCollection<ChatWindowSettings>(App.BaseDispatcher);
            PerfMonitorSettings = new PerfMonitorSettings();
            ChatSettings = new WindowSettingsBase();
            LfgWindowSettings = new LfgWindowSettings();
            NotificationAreaSettings = new NotificationAreaSettings();

            MaxMessages = 500;
            SpamThreshold = 2;
            ShowChannel = true;
            ShowTimestamp = true;
            FontSize = 15;
            ChatScrollAmount = 1;
            LastRun = DateTime.MinValue;
            TwitchName = "";
            TwitchToken = "";
            TwitchChannelName = "";
            ShowTradeLfg = true;
            UseHotkeys = true;
            EthicalMode = false;
            LfgHotkey = new HotKey(Key.Y, ModifierKeys.Control);
            DashboardHotkey = new HotKey(Key.I, ModifierKeys.Control);
            SettingsHotkey = new HotKey(Key.O, ModifierKeys.Control);
            SkillSettingsHotkey = new HotKey(Key.K, ModifierKeys.Control);
            AbnormalSettingsHotkey = new HotKey(Key.B, ModifierKeys.Control);
            ReturnToLobbyHotkey = new HotKey(Key.R, ModifierKeys.Control | ModifierKeys.Alt);
            ForceClickableChatHotkey = new HotKey(Key.C, ModifierKeys.Control | ModifierKeys.Alt);
            ToggleBoundariesHotkey = new HotKey(Key.H, ModifierKeys.Control | ModifierKeys.Alt);
            ToggleHideAllHotkey = new HotKey(Key.Z, ModifierKeys.Control | ModifierKeys.Alt);
            LanguageOverride = LanguageOverride.None;
            HideHandles = false;
            HighPriority = false;
            ForceSoftwareRendering = true;
            AbnormalityShape = ControlShape.Round;
            SkillShape = ControlShape.Round;
            MentionMode = MentionMode.All;
            Npcap = true;
            CheckOpcodesHash = true;
            CheckGuildBamWithoutOpcode = false;
            UserExcludedSysMsg = new List<string>();
            BetaNotification = true;
            FpsAtGuardian = true;
            EnableProxy = true;
            DisableLfgChatMessages = true;
            WebhookEnabledFieldBoss = false;
            WebhookUrlFieldBoss = "";
            WebhookMessageFieldBossSpawn = "@here {bossName} spawned in {regionName}!";
            WebhookMessageFieldBossDie = "{bossName} is dead.";
            WebhookEnabledGuildBam = false;
            WebhookUrlGuildBam = "";
            WebhookMessageGuildBam = "@here Guild BAM will spawn soon!";
            WebhookEnabledMentions = false;
            WebhookUrlMentions = "";
            ShowDecimalsInCooldowns = true;
        }

        private void OnReleaseVersionChanged(int v)
        {
#if TERA_X64
            var major = v / 100;
            if (major >= 97)
            {
                var settings = new List<WindowSettingsBase>
                {
                    CooldownWindowSettings,
                    CharacterWindowSettings,
                    NpcWindowSettings,
                    BuffWindowSettings,
                    ClassWindowSettings,
                    GroupWindowSettings,
                    FlightGaugeWindowSettings,
                    FloatingButtonSettings,
                    CivilUnrestWindowSettings,
                    NotificationAreaSettings
                }.Concat(ChatWindowsSettings).Where(s => s.ClickThruMode == ClickThruMode.GameDriven);

                if (settings.Any())
                {
                    foreach (var ws in settings)
                    {
                        ws.ClickThruMode = ClickThruMode.Always;
                    }

                    TccMessageBox.Show("TCC", SR.ForcingGameDrivenClickThruOff, MessageBoxButton.OK);
                }
            }
#endif
        }

        public static SettingsContainer Load()
        {
            var settingsPath = SettingsOverride == ""
                ? Path.Combine(App.BasePath, SettingsGlobals.SettingsFileName)
                : SettingsOverride;
            return new JsonSettingsReader().LoadSettings(settingsPath);
        }
        public void Save()
        {
            var toRemove = ChatWindowsSettings.Where(s => s.Tabs.Count == 0).ToList();
            toRemove.ForEach(s => ChatWindowsSettings.Remove(s));
            new JsonSettingsWriter().Save();
        }

    }
}
