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
        bool OverrideSetting;
        bool Validation;

        public event OverrideHandler ToggleOverride;
        public event ValidationHandler ToggleValidation;
        public event SoundHandler ToggleSound;

        public Setting(bool overrideSetting, bool validation, bool sound)
        {
            InitializeComponent();
            OverrideSetting = overrideSetting;
            Validation = validation;
            Sound = sound;

            this.PlayerSlot.Text = Utilities.player1SlotBase.ToString("X");
            this.PlayerOffset.Text = Utilities.playerOffset.ToString("X");
            this.Slot21Offset.Text = Utilities.Slot21Offset.ToString("X");
            this.HomeOffset.Text = Utilities.HomeOffset.ToString("X");
            this.ReactionOffset.Text = Utilities.ReactionOffset.ToString("X");
            this.Villager.Text = Utilities.VillagerAddress.ToString("X");
            this.VillagerOffset.Text = Utilities.VillagerSize.ToString("X");
            this.VillagerHouse.Text = Utilities.VillagerHouseAddress.ToString("X");
            this.VillagerHouseOffset.Text = Utilities.VillagerHouseSize.ToString("X");
            this.VillagerHouseBuffer.Text = Utilities.VillagerHouseBufferDiff.ToString("X");
            this.RecyclingBin.Text = Utilities.MasterRecyclingBase.ToString("X");
            this.Turnip.Text = Utilities.TurnipPurchasePriceAddr.ToString("X");
            this.Stamina.Text = Utilities.precedingZeros(Utilities.staminaAddress.ToString("X"), 8);
            this.WalkSpeed.Text = Utilities.precedingZeros(Utilities.wSpeedAddress.ToString("X"), 8);
            this.AnimationSpeed.Text = Utilities.precedingZeros(Utilities.aSpeedAddress.ToString("X"), 8);
            this.Collision.Text = Utilities.precedingZeros(Utilities.CollisionAddress.ToString("X"), 8);
            this.FreezeTime.Text = Utilities.precedingZeros(Utilities.freezeTimeAddress.ToString("X"), 8);
            this.ReadTime.Text = Utilities.precedingZeros(Utilities.readTimeAddress.ToString("X"), 8);
            this.WeatherSeed.Text = Utilities.weatherSeed.ToString("X");
            this.MapZero.Text = Utilities.mapZero.ToString("X");
        }

        public void UpdateToggle(bool OpenForm)
        {
            if (OpenForm)
                startup = true;

            if (OverrideSetting)
            {
                this.addresses.Enabled = true;

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

            startup = false;
        }

        public void OverrideAddresses()
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            this.PlayerSlot.Text = Config.AppSettings.Settings["PlayerSlot"].Value;
            this.PlayerOffset.Text = Config.AppSettings.Settings["PlayerOffset"].Value;
            this.Slot21Offset.Text = Config.AppSettings.Settings["Slot21Offset"].Value;
            this.HomeOffset.Text = Config.AppSettings.Settings["HomeOffset"].Value;
            this.ReactionOffset.Text = Config.AppSettings.Settings["ReactionOffset"].Value;
            this.Villager.Text = Config.AppSettings.Settings["Villager"].Value;
            this.VillagerOffset.Text = Config.AppSettings.Settings["VillagerOffset"].Value;
            this.VillagerHouse.Text = Config.AppSettings.Settings["VillagerHouse"].Value;
            this.VillagerHouseOffset.Text = Config.AppSettings.Settings["VillagerHouseOffset"].Value;
            this.VillagerHouseBuffer.Text = Config.AppSettings.Settings["VillagerHouseBuffer"].Value;
            this.RecyclingBin.Text = Config.AppSettings.Settings["RecyclingBin"].Value;
            this.Turnip.Text = Config.AppSettings.Settings["Turnip"].Value;
            this.Stamina.Text = Config.AppSettings.Settings["Stamina"].Value;
            this.WalkSpeed.Text = Config.AppSettings.Settings["WalkSpeed"].Value;
            this.AnimationSpeed.Text = Config.AppSettings.Settings["AnimationSpeed"].Value;
            this.Collision.Text = Config.AppSettings.Settings["Collision"].Value;
            this.FreezeTime.Text = Config.AppSettings.Settings["FreezeTime"].Value;
            this.ReadTime.Text = Config.AppSettings.Settings["ReadTime"].Value;
            this.WeatherSeed.Text = Config.AppSettings.Settings["WeatherSeed"].Value;
            this.MapZero.Text = Config.AppSettings.Settings["MapZero"].Value;


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

            Utilities.overrideAddresses(ConfigValue);
        }

        private static void ResetAddresses()
        {
            Application.Restart();
        }

        private void HexKeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == (char)Keys.Back || (c == 'v' && Control.ModifierKeys == Keys.Control)))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);
        }

        private void BoxValidating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Text = box.Text.ToUpper();
            char[] allowedChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            foreach (char character in box.Text.ToUpper().ToArray())
            {
                if (!allowedChars.Contains(character))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("'{0}' is not a hexadecimal character!", character));
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

            Config.AppSettings.Settings["PlayerSlot"].Value = this.PlayerSlot.Text;
            Config.AppSettings.Settings["PlayerOffset"].Value = this.PlayerOffset.Text;
            Config.AppSettings.Settings["Slot21Offset"].Value = this.Slot21Offset.Text;
            Config.AppSettings.Settings["HomeOffset"].Value = this.HomeOffset.Text;
            Config.AppSettings.Settings["ReactionOffset"].Value = this.ReactionOffset.Text;
            Config.AppSettings.Settings["Villager"].Value = this.Villager.Text;
            Config.AppSettings.Settings["VillagerOffset"].Value = this.VillagerOffset.Text;
            Config.AppSettings.Settings["VillagerHouse"].Value = this.VillagerHouse.Text;
            Config.AppSettings.Settings["VillagerHouseOffset"].Value = this.VillagerHouseOffset.Text;
            Config.AppSettings.Settings["VillagerHouseBuffer"].Value = this.VillagerHouseBuffer.Text;
            Config.AppSettings.Settings["RecyclingBin"].Value = this.RecyclingBin.Text;
            Config.AppSettings.Settings["Turnip"].Value = this.Turnip.Text;
            Config.AppSettings.Settings["Stamina"].Value = this.Stamina.Text;
            Config.AppSettings.Settings["WalkSpeed"].Value = this.WalkSpeed.Text;
            Config.AppSettings.Settings["AnimationSpeed"].Value = this.AnimationSpeed.Text;
            Config.AppSettings.Settings["Collision"].Value = this.Collision.Text;
            Config.AppSettings.Settings["FreezeTime"].Value = this.FreezeTime.Text;
            Config.AppSettings.Settings["ReadTime"].Value = this.ReadTime.Text;
            Config.AppSettings.Settings["WeatherSeed"].Value = this.WeatherSeed.Text;
            Config.AppSettings.Settings["MapZero"].Value = this.MapZero.Text;

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

            Utilities.overrideAddresses(ConfigValue);

            if (Sound)
                System.Media.SystemSounds.Asterisk.Play();

            this.Close();
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
                    this.addresses.Enabled = false;
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
                    this.addresses.Enabled = true;
                    OverrideAddresses();
                }
            }

            this.ToggleOverride();

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
                    this.ToggleValidation();
                }
            }
            else
            {
                Validation = true;
                Config.AppSettings.Settings["validation"].Value = "true";
                Config.Save(ConfigurationSaveMode.Minimal);
                ValidationLabel.ForeColor = Color.White;
                this.ToggleValidation();
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
                this.ToggleSound(false);
            }
            else
            {
                Sound = true;
                Config.AppSettings.Settings["sound"].Value = "true";
                Config.Save(ConfigurationSaveMode.Minimal);
                Sound = true;
                this.ToggleSound(false);
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
