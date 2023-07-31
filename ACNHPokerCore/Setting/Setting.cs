using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Setting : Form
    {
        bool startup = true;

        bool Sound;
        bool CaptureSetting;
        bool OverrideSetting;
        bool Validation;

        public event OverrideHandler ToggleOverride;
        public event ValidationHandler ToggleValidation;
        public event SoundHandler ToggleSound;
        public event CaptureHandler ToggleCapture;


        public Setting(bool overrideSetting, bool validation, bool sound, bool capturesetting)
        {
            InitializeComponent();
            OverrideSetting = overrideSetting;
            Validation = validation;
            Sound = sound;
            CaptureSetting = capturesetting;

            PlayerSlot.Text = Utilities.player1SlotBase.ToString("X");
            PlayerOffset.Text = Utilities.playerOffset.ToString("X");
            Slot21Offset.Text = Utilities.Slot21Offset.ToString("X");
            HomeOffset.Text = Utilities.HomeOffset.ToString("X");
            ReactionOffset.Text = Utilities.ReactionOffset.ToString("X");
            Villager.Text = Utilities.VillagerAddress.ToString("X");
            VillagerOffset.Text = Utilities.VillagerSize.ToString("X");
            VillagerHouse.Text = Utilities.VillagerHouseAddress.ToString("X");
            VillagerHouseOffset.Text = Utilities.VillagerHouseSize.ToString("X");
            VillagerHouseBuffer.Text = Utilities.VillagerHouseBufferDiff.ToString("X");
            RecyclingBin.Text = Utilities.MasterRecyclingBase.ToString("X");
            Turnip.Text = Utilities.TurnipPurchasePriceAddr.ToString("X");
            Stamina.Text = Utilities.PrecedingZeros(Utilities.staminaAddress.ToString("X"), 8);
            WalkSpeed.Text = Utilities.PrecedingZeros(Utilities.wSpeedAddress.ToString("X"), 8);
            AnimationSpeed.Text = Utilities.PrecedingZeros(Utilities.aSpeedAddress.ToString("X"), 8);
            Collision.Text = Utilities.PrecedingZeros(Utilities.CollisionAddress.ToString("X"), 8);
            FreezeTime.Text = Utilities.PrecedingZeros(Utilities.freezeTimeAddress.ToString("X"), 8);
            ReadTime.Text = Utilities.PrecedingZeros(Utilities.readTimeAddress.ToString("X"), 8);
            WeatherSeed.Text = Utilities.weatherSeed.ToString("X");
            MapZero.Text = Utilities.mapZero.ToString("X");
        }

        public void UpdateToggle(bool OpenForm)
        {
            if (OpenForm)
                startup = true;

            if (OverrideSetting)
            {
                addresses.Enabled = true;

                AddressOverrideLabel.ForeColor = Color.Red;
                AddressOverrideToggle.Checked = true;
            }
            else
            {
                AddressOverrideToggle.Checked = false;
            }

            if (Validation)
            {
                ValidationToggle.Checked = true;
            }
            else
            {
                ValidationLabel.ForeColor = Color.Red;
                ValidationToggle.Checked = false;
            }

            if (Sound)
            {
                SoundToggle.Checked = true;
            }
            else
            {
                SoundToggle.Checked = false;
            }

            if (CaptureSetting)
            {
                CaptureToggle.Checked = true;
            }
            else
            {
                CaptureToggle.Checked = false;
            }

            startup = false;
        }

        public void OverrideAddresses()
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            PlayerSlot.Text = Config.AppSettings.Settings["PlayerSlot"].Value;
            PlayerOffset.Text = Config.AppSettings.Settings["PlayerOffset"].Value;
            Slot21Offset.Text = Config.AppSettings.Settings["Slot21Offset"].Value;
            HomeOffset.Text = Config.AppSettings.Settings["HomeOffset"].Value;
            ReactionOffset.Text = Config.AppSettings.Settings["ReactionOffset"].Value;
            Villager.Text = Config.AppSettings.Settings["Villager"].Value;
            VillagerOffset.Text = Config.AppSettings.Settings["VillagerOffset"].Value;
            VillagerHouse.Text = Config.AppSettings.Settings["VillagerHouse"].Value;
            VillagerHouseOffset.Text = Config.AppSettings.Settings["VillagerHouseOffset"].Value;
            VillagerHouseBuffer.Text = Config.AppSettings.Settings["VillagerHouseBuffer"].Value;
            RecyclingBin.Text = Config.AppSettings.Settings["RecyclingBin"].Value;
            Turnip.Text = Config.AppSettings.Settings["Turnip"].Value;
            Stamina.Text = Config.AppSettings.Settings["Stamina"].Value;
            WalkSpeed.Text = Config.AppSettings.Settings["WalkSpeed"].Value;
            AnimationSpeed.Text = Config.AppSettings.Settings["AnimationSpeed"].Value;
            Collision.Text = Config.AppSettings.Settings["Collision"].Value;
            FreezeTime.Text = Config.AppSettings.Settings["FreezeTime"].Value;
            ReadTime.Text = Config.AppSettings.Settings["ReadTime"].Value;
            WeatherSeed.Text = Config.AppSettings.Settings["WeatherSeed"].Value;
            MapZero.Text = Config.AppSettings.Settings["MapZero"].Value;


            if (Utilities.player1SlotBase != Convert.ToUInt32(PlayerSlot.Text, 16))
                PlayerSlot.ForeColor = Color.Red;
            if (Utilities.playerOffset != Convert.ToUInt32(PlayerOffset.Text, 16))
                PlayerOffset.ForeColor = Color.Red;
            if (Utilities.Slot21Offset != Convert.ToUInt32(Slot21Offset.Text, 16))
                Slot21Offset.ForeColor = Color.Red;
            if (Utilities.HomeOffset != Convert.ToUInt32(HomeOffset.Text, 16))
                HomeOffset.ForeColor = Color.Red;
            if (Utilities.ReactionOffset != Convert.ToUInt32(ReactionOffset.Text, 16))
                ReactionOffset.ForeColor = Color.Red;

            if (Utilities.VillagerAddress != Convert.ToUInt32(Villager.Text, 16))
                Villager.ForeColor = Color.Red;
            if (Utilities.VillagerSize != Convert.ToUInt32(VillagerOffset.Text, 16))
                VillagerOffset.ForeColor = Color.Red;
            if (Utilities.VillagerHouseAddress != Convert.ToUInt32(VillagerHouse.Text, 16))
                VillagerHouse.ForeColor = Color.Red;
            if (Utilities.VillagerHouseSize != Convert.ToUInt32(VillagerHouseOffset.Text, 16))
                VillagerHouseOffset.ForeColor = Color.Red;
            if (Utilities.VillagerHouseBufferDiff != Convert.ToUInt32(VillagerHouseBuffer.Text, 16))
                VillagerHouseBuffer.ForeColor = Color.Red;

            if (Utilities.MasterRecyclingBase != Convert.ToUInt32(RecyclingBin.Text, 16))
                RecyclingBin.ForeColor = Color.Red;
            if (Utilities.TurnipPurchasePriceAddr != Convert.ToUInt32(Turnip.Text, 16))
                Turnip.ForeColor = Color.Red;

            if (Utilities.staminaAddress != Convert.ToUInt32(Stamina.Text, 16))
                Stamina.ForeColor = Color.Red;
            if (Utilities.wSpeedAddress != Convert.ToUInt32(WalkSpeed.Text, 16))
                WalkSpeed.ForeColor = Color.Red;
            if (Utilities.aSpeedAddress != Convert.ToUInt32(AnimationSpeed.Text, 16))
                AnimationSpeed.ForeColor = Color.Red;
            if (Utilities.CollisionAddress != Convert.ToUInt32(Collision.Text, 16))
                Collision.ForeColor = Color.Red;

            if (Utilities.freezeTimeAddress != Convert.ToUInt32(FreezeTime.Text, 16))
                FreezeTime.ForeColor = Color.Red;
            if (Utilities.readTimeAddress != Convert.ToUInt32(ReadTime.Text, 16))
                ReadTime.ForeColor = Color.Red;

            if (Utilities.weatherSeed != Convert.ToUInt32(WeatherSeed.Text, 16))
                WeatherSeed.ForeColor = Color.Red;
            if (Utilities.mapZero != Convert.ToUInt32(MapZero.Text, 16))
                MapZero.ForeColor = Color.Red;

            Dictionary<string, UInt32> ConfigValue = new()
            {
                { "PlayerSlot", Convert.ToUInt32(PlayerSlot.Text, 16) },
                { "PlayerOffset", Convert.ToUInt32(PlayerOffset.Text, 16) },
                { "Slot21Offset", Convert.ToUInt32(Slot21Offset.Text, 16) },
                { "HomeOffset", Convert.ToUInt32(HomeOffset.Text, 16) },
                { "ReactionOffset", Convert.ToUInt32(ReactionOffset.Text, 16) },
                { "Villager", Convert.ToUInt32(Villager.Text, 16) },
                { "VillagerOffset", Convert.ToUInt32(VillagerOffset.Text, 16) },
                { "VillagerHouse", Convert.ToUInt32(VillagerHouse.Text, 16) },
                { "VillagerHouseOffset", Convert.ToUInt32(VillagerHouseOffset.Text, 16) },
                { "VillagerHouseBuffer", Convert.ToUInt32(VillagerHouseBuffer.Text, 16) },
                { "RecyclingBin", Convert.ToUInt32(RecyclingBin.Text, 16) },
                { "Turnip", Convert.ToUInt32(Turnip.Text, 16) },
                { "Stamina", Convert.ToUInt32(Stamina.Text, 16) },
                { "WalkSpeed", Convert.ToUInt32(WalkSpeed.Text, 16) },
                { "AnimationSpeed", Convert.ToUInt32(AnimationSpeed.Text, 16) },
                { "Collision", Convert.ToUInt32(Collision.Text, 16) },
                { "FreezeTime", Convert.ToUInt32(FreezeTime.Text, 16) },
                { "ReadTime", Convert.ToUInt32(ReadTime.Text, 16) },
                { "WeatherSeed", Convert.ToUInt32(WeatherSeed.Text, 16) },
                { "MapZero", Convert.ToUInt32(MapZero.Text, 16) }
            };

            Utilities.OverrideAddresses(ConfigValue);
        }

        private static void ResetAddresses()
        {
            Application.Restart();
        }

        private void HexKeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == (char)Keys.Back || (c == 'v' && ModifierKeys == Keys.Control)))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);
        }

        private void BoxValidating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Text = box.Text.ToUpper();
            char[] allowedChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            foreach (char character in box.Text.ToUpper().ToArray())
            {
                if (!allowedChars.Contains(character))
                {
                    MessageBox.Show(string.Format("'{0}' is not a hexadecimal character!", character));
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void BoxTextChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            TextBox box = (TextBox)sender;
            box.ForeColor = Color.Red;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            Dictionary<string, UInt32> ConfigValue = new();

            Config.AppSettings.Settings["PlayerSlot"].Value = PlayerSlot.Text;
            Config.AppSettings.Settings["PlayerOffset"].Value = PlayerOffset.Text;
            Config.AppSettings.Settings["Slot21Offset"].Value = Slot21Offset.Text;
            Config.AppSettings.Settings["HomeOffset"].Value = HomeOffset.Text;
            Config.AppSettings.Settings["ReactionOffset"].Value = ReactionOffset.Text;
            Config.AppSettings.Settings["Villager"].Value = Villager.Text;
            Config.AppSettings.Settings["VillagerOffset"].Value = VillagerOffset.Text;
            Config.AppSettings.Settings["VillagerHouse"].Value = VillagerHouse.Text;
            Config.AppSettings.Settings["VillagerHouseOffset"].Value = VillagerHouseOffset.Text;
            Config.AppSettings.Settings["VillagerHouseBuffer"].Value = VillagerHouseBuffer.Text;
            Config.AppSettings.Settings["RecyclingBin"].Value = RecyclingBin.Text;
            Config.AppSettings.Settings["Turnip"].Value = Turnip.Text;
            Config.AppSettings.Settings["Stamina"].Value = Stamina.Text;
            Config.AppSettings.Settings["WalkSpeed"].Value = WalkSpeed.Text;
            Config.AppSettings.Settings["AnimationSpeed"].Value = AnimationSpeed.Text;
            Config.AppSettings.Settings["Collision"].Value = Collision.Text;
            Config.AppSettings.Settings["FreezeTime"].Value = FreezeTime.Text;
            Config.AppSettings.Settings["ReadTime"].Value = ReadTime.Text;
            Config.AppSettings.Settings["WeatherSeed"].Value = WeatherSeed.Text;
            Config.AppSettings.Settings["MapZero"].Value = MapZero.Text;

            Config.Save(ConfigurationSaveMode.Minimal);

            ConfigValue.Add("PlayerSlot", Convert.ToUInt32(PlayerSlot.Text, 16));
            ConfigValue.Add("PlayerOffset", Convert.ToUInt32(PlayerOffset.Text, 16));
            ConfigValue.Add("Slot21Offset", Convert.ToUInt32(Slot21Offset.Text, 16));
            ConfigValue.Add("HomeOffset", Convert.ToUInt32(HomeOffset.Text, 16));
            ConfigValue.Add("ReactionOffset", Convert.ToUInt32(ReactionOffset.Text, 16));
            ConfigValue.Add("Villager", Convert.ToUInt32(Villager.Text, 16));
            ConfigValue.Add("VillagerOffset", Convert.ToUInt32(VillagerOffset.Text, 16));
            ConfigValue.Add("VillagerHouse", Convert.ToUInt32(VillagerHouse.Text, 16));
            ConfigValue.Add("VillagerHouseOffset", Convert.ToUInt32(VillagerHouseOffset.Text, 16));
            ConfigValue.Add("VillagerHouseBuffer", Convert.ToUInt32(VillagerHouseBuffer.Text, 16));
            ConfigValue.Add("RecyclingBin", Convert.ToUInt32(RecyclingBin.Text, 16));
            ConfigValue.Add("Turnip", Convert.ToUInt32(Turnip.Text, 16));
            ConfigValue.Add("Stamina", Convert.ToUInt32(Stamina.Text, 16));
            ConfigValue.Add("WalkSpeed", Convert.ToUInt32(WalkSpeed.Text, 16));
            ConfigValue.Add("AnimationSpeed", Convert.ToUInt32(AnimationSpeed.Text, 16));
            ConfigValue.Add("Collision", Convert.ToUInt32(Collision.Text, 16));
            ConfigValue.Add("FreezeTime", Convert.ToUInt32(FreezeTime.Text, 16));
            ConfigValue.Add("ReadTime", Convert.ToUInt32(ReadTime.Text, 16));
            ConfigValue.Add("WeatherSeed", Convert.ToUInt32(WeatherSeed.Text, 16));
            ConfigValue.Add("MapZero", Convert.ToUInt32(MapZero.Text, 16));

            Utilities.OverrideAddresses(ConfigValue);

            if (Sound)
                System.Media.SystemSounds.Asterisk.Play();

            Close();
        }

        private void ImageBtn_Click(object sender, EventArgs e)
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["ForcedImageDownload"].Value = "true";
            Config.Save(ConfigurationSaveMode.Minimal);
            Application.Restart();
        }

        private void AddressOverrideToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (Config.AppSettings.Settings["override"].Value == "true")
            {
                DialogResult dialogResult = MyMessageBox.Show("The application will restart and reset all addresses!\n\nAre you sure you want to disable address override?", "Disable Override", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    OverrideSetting = false;
                    Config.AppSettings.Settings["override"].Value = "false";
                    Config.Save(ConfigurationSaveMode.Minimal);
                    addresses.Enabled = false;
                    ResetAddresses();
                }
            }
            else
            {
                DialogResult dialogResult = MyMessageBox.Show("Please make sure you have acquired the correct addresses for your game version!\n\nAre you sure you want to enable address override?", "Enable Override", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    OverrideSetting = true;
                    Config.AppSettings.Settings["override"].Value = "true";
                    Config.Save(ConfigurationSaveMode.Minimal);
                    AddressOverrideLabel.ForeColor = Color.Red;
                    addresses.Enabled = true;
                    OverrideAddresses();
                }
            }

            ToggleOverride?.Invoke();

            if (Sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ValidationToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (Config.AppSettings.Settings["validation"].Value == "true")
            {
                DialogResult dialogResult = MyMessageBox.Show("Validation is meant to prevent save file corruption!\nPlease only do so if you know what you are doing.\n\nAre you sure you want to disable validation?", "Disable Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    Validation = false;
                    Config.AppSettings.Settings["validation"].Value = "false";
                    Config.Save(ConfigurationSaveMode.Minimal);
                    ValidationLabel.ForeColor = Color.Red;
                    ToggleValidation?.Invoke();
                }
            }
            else
            {
                Validation = true;
                Config.AppSettings.Settings["validation"].Value = "true";
                Config.Save(ConfigurationSaveMode.Minimal);
                ValidationLabel.ForeColor = Color.White;
                ToggleValidation?.Invoke();
            }
        }

        private void SoundToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (Config.AppSettings.Settings["sound"].Value == "true")
            {
                Sound = false;
                Config.AppSettings.Settings["sound"].Value = "false";
                Config.Save(ConfigurationSaveMode.Minimal);
                Sound = false;
                ToggleSound?.Invoke(false);
            }
            else
            {
                Sound = true;
                Config.AppSettings.Settings["sound"].Value = "true";
                Config.Save(ConfigurationSaveMode.Minimal);
                Sound = true;
                ToggleSound?.Invoke(false);
            }
            if (Sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
        private void CaptureToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (Config.AppSettings.Settings["capture"].Value == "true")
            {
                CaptureSetting = false;
                Config.AppSettings.Settings["capture"].Value = "false";
                Config.Save(ConfigurationSaveMode.Minimal);
                CaptureSetting = false;
                this.ToggleCapture(false);
            }
            else
            {
                CaptureSetting = true;
                Config.AppSettings.Settings["capture"].Value = "true";
                Config.Save(ConfigurationSaveMode.Minimal);
                CaptureSetting = true;
                this.ToggleCapture(true);
            }
            if (Sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
        private void Setting_Load(object sender, EventArgs e)
        {
            UpdateToggle(true);
        }
    }
}
