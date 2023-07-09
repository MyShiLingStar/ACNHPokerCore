namespace ACNHPokerCore
{
    partial class Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setting));
            this.PlayerSlotLabel = new System.Windows.Forms.Label();
            this.PlayerSlot = new System.Windows.Forms.TextBox();
            this.Slot21Offset = new System.Windows.Forms.TextBox();
            this.HomeOffset = new System.Windows.Forms.TextBox();
            this.ReactionOffset = new System.Windows.Forms.TextBox();
            this.PlayerOffset = new System.Windows.Forms.TextBox();
            this.Villager = new System.Windows.Forms.TextBox();
            this.VillagerOffset = new System.Windows.Forms.TextBox();
            this.PlayerOffsetLabel = new System.Windows.Forms.Label();
            this.Slot21OffsetLabel = new System.Windows.Forms.Label();
            this.HomeOffsetLabel = new System.Windows.Forms.Label();
            this.ReactionOffsetLabel = new System.Windows.Forms.Label();
            this.VillagerLabel = new System.Windows.Forms.Label();
            this.VillagerOffsetLabel = new System.Windows.Forms.Label();
            this.VillagerHouse = new System.Windows.Forms.TextBox();
            this.VillagerHouseLabel = new System.Windows.Forms.Label();
            this.VillagerHouseOffsetLabel = new System.Windows.Forms.Label();
            this.VillagerHouseOffset = new System.Windows.Forms.TextBox();
            this.VillagerHouseBufferLabel = new System.Windows.Forms.Label();
            this.VillagerHouseBuffer = new System.Windows.Forms.TextBox();
            this.StaminaLabel = new System.Windows.Forms.Label();
            this.Stamina = new System.Windows.Forms.TextBox();
            this.WalkSpeedLabel = new System.Windows.Forms.Label();
            this.AnimationSpeedLabel = new System.Windows.Forms.Label();
            this.WalkSpeed = new System.Windows.Forms.TextBox();
            this.AnimationSpeed = new System.Windows.Forms.TextBox();
            this.CollisionLabel = new System.Windows.Forms.Label();
            this.Collision = new System.Windows.Forms.TextBox();
            this.FreezeTimeLabel = new System.Windows.Forms.Label();
            this.FreezeTime = new System.Windows.Forms.TextBox();
            this.ReadTimeLabel = new System.Windows.Forms.Label();
            this.ReadTime = new System.Windows.Forms.TextBox();
            this.RecyclingBinLabel = new System.Windows.Forms.Label();
            this.TurnipLabel = new System.Windows.Forms.Label();
            this.RecyclingBin = new System.Windows.Forms.TextBox();
            this.Turnip = new System.Windows.Forms.TextBox();
            this.WeatherSeedLabel = new System.Windows.Forms.Label();
            this.WeatherSeed = new System.Windows.Forms.TextBox();
            this.addresses = new System.Windows.Forms.Panel();
            this.MapZeroLabel = new System.Windows.Forms.Label();
            this.MapZero = new System.Windows.Forms.TextBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.ImageBtn = new System.Windows.Forms.Button();
            this.AddressOverrideToggle = new JCS.ToggleSwitch();
            this.AddressOverrideLabel = new System.Windows.Forms.Label();
            this.ValidationLabel = new System.Windows.Forms.Label();
            this.ValidationToggle = new JCS.ToggleSwitch();
            this.SoundToggle = new JCS.ToggleSwitch();
            this.SoundLabel = new System.Windows.Forms.Label();
            this.CaptureToggle = new JCS.ToggleSwitch();
            this.CaptureLabel = new System.Windows.Forms.Label();
            this.addresses.SuspendLayout();
            this.SuspendLayout();
            // 
            // PlayerSlotLabel
            // 
            this.PlayerSlotLabel.AutoSize = true;
            this.PlayerSlotLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PlayerSlotLabel.ForeColor = System.Drawing.Color.White;
            this.PlayerSlotLabel.Location = new System.Drawing.Point(8, 11);
            this.PlayerSlotLabel.Name = "PlayerSlotLabel";
            this.PlayerSlotLabel.Size = new System.Drawing.Size(153, 19);
            this.PlayerSlotLabel.TabIndex = 37;
            this.PlayerSlotLabel.Text = "Player 1 Item Slot 1";
            // 
            // PlayerSlot
            // 
            this.PlayerSlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.PlayerSlot.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlayerSlot.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PlayerSlot.ForeColor = System.Drawing.Color.White;
            this.PlayerSlot.Location = new System.Drawing.Point(306, 4);
            this.PlayerSlot.MaxLength = 8;
            this.PlayerSlot.Name = "PlayerSlot";
            this.PlayerSlot.Size = new System.Drawing.Size(180, 31);
            this.PlayerSlot.TabIndex = 38;
            this.PlayerSlot.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PlayerSlot.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.PlayerSlot.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // Slot21Offset
            // 
            this.Slot21Offset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.Slot21Offset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Slot21Offset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Slot21Offset.ForeColor = System.Drawing.Color.White;
            this.Slot21Offset.Location = new System.Drawing.Point(306, 78);
            this.Slot21Offset.MaxLength = 8;
            this.Slot21Offset.Name = "Slot21Offset";
            this.Slot21Offset.Size = new System.Drawing.Size(180, 31);
            this.Slot21Offset.TabIndex = 39;
            this.Slot21Offset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Slot21Offset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.Slot21Offset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // HomeOffset
            // 
            this.HomeOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.HomeOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HomeOffset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HomeOffset.ForeColor = System.Drawing.Color.White;
            this.HomeOffset.Location = new System.Drawing.Point(306, 115);
            this.HomeOffset.MaxLength = 8;
            this.HomeOffset.Name = "HomeOffset";
            this.HomeOffset.Size = new System.Drawing.Size(180, 31);
            this.HomeOffset.TabIndex = 40;
            this.HomeOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HomeOffset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.HomeOffset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // ReactionOffset
            // 
            this.ReactionOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.ReactionOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ReactionOffset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ReactionOffset.ForeColor = System.Drawing.Color.White;
            this.ReactionOffset.Location = new System.Drawing.Point(306, 152);
            this.ReactionOffset.MaxLength = 8;
            this.ReactionOffset.Name = "ReactionOffset";
            this.ReactionOffset.Size = new System.Drawing.Size(180, 31);
            this.ReactionOffset.TabIndex = 41;
            this.ReactionOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReactionOffset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.ReactionOffset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // PlayerOffset
            // 
            this.PlayerOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.PlayerOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlayerOffset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PlayerOffset.ForeColor = System.Drawing.Color.White;
            this.PlayerOffset.Location = new System.Drawing.Point(306, 41);
            this.PlayerOffset.MaxLength = 8;
            this.PlayerOffset.Name = "PlayerOffset";
            this.PlayerOffset.Size = new System.Drawing.Size(180, 31);
            this.PlayerOffset.TabIndex = 42;
            this.PlayerOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PlayerOffset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.PlayerOffset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // Villager
            // 
            this.Villager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.Villager.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Villager.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Villager.ForeColor = System.Drawing.Color.White;
            this.Villager.Location = new System.Drawing.Point(306, 198);
            this.Villager.MaxLength = 8;
            this.Villager.Name = "Villager";
            this.Villager.Size = new System.Drawing.Size(180, 31);
            this.Villager.TabIndex = 43;
            this.Villager.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Villager.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.Villager.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // VillagerOffset
            // 
            this.VillagerOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VillagerOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VillagerOffset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerOffset.ForeColor = System.Drawing.Color.White;
            this.VillagerOffset.Location = new System.Drawing.Point(306, 235);
            this.VillagerOffset.MaxLength = 8;
            this.VillagerOffset.Name = "VillagerOffset";
            this.VillagerOffset.Size = new System.Drawing.Size(180, 31);
            this.VillagerOffset.TabIndex = 44;
            this.VillagerOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VillagerOffset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.VillagerOffset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // PlayerOffsetLabel
            // 
            this.PlayerOffsetLabel.AutoSize = true;
            this.PlayerOffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PlayerOffsetLabel.ForeColor = System.Drawing.Color.White;
            this.PlayerOffsetLabel.Location = new System.Drawing.Point(8, 45);
            this.PlayerOffsetLabel.Name = "PlayerOffsetLabel";
            this.PlayerOffsetLabel.Size = new System.Drawing.Size(153, 19);
            this.PlayerOffsetLabel.TabIndex = 45;
            this.PlayerOffsetLabel.Text = "Player Offset (Size)";
            // 
            // Slot21OffsetLabel
            // 
            this.Slot21OffsetLabel.AutoSize = true;
            this.Slot21OffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Slot21OffsetLabel.ForeColor = System.Drawing.Color.White;
            this.Slot21OffsetLabel.Location = new System.Drawing.Point(8, 82);
            this.Slot21OffsetLabel.Name = "Slot21OffsetLabel";
            this.Slot21OffsetLabel.Size = new System.Drawing.Size(211, 19);
            this.Slot21OffsetLabel.TabIndex = 46;
            this.Slot21OffsetLabel.Text = "Player 1 Item Slot 21 Offset";
            // 
            // HomeOffsetLabel
            // 
            this.HomeOffsetLabel.AutoSize = true;
            this.HomeOffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HomeOffsetLabel.ForeColor = System.Drawing.Color.White;
            this.HomeOffsetLabel.Location = new System.Drawing.Point(8, 119);
            this.HomeOffsetLabel.Name = "HomeOffsetLabel";
            this.HomeOffsetLabel.Size = new System.Drawing.Size(232, 19);
            this.HomeOffsetLabel.TabIndex = 47;
            this.HomeOffsetLabel.Text = "Player 1 Home Storage Offset";
            // 
            // ReactionOffsetLabel
            // 
            this.ReactionOffsetLabel.AutoSize = true;
            this.ReactionOffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ReactionOffsetLabel.ForeColor = System.Drawing.Color.White;
            this.ReactionOffsetLabel.Location = new System.Drawing.Point(8, 156);
            this.ReactionOffsetLabel.Name = "ReactionOffsetLabel";
            this.ReactionOffsetLabel.Size = new System.Drawing.Size(242, 19);
            this.ReactionOffsetLabel.TabIndex = 48;
            this.ReactionOffsetLabel.Text = "Player 1 Reaction Wheel Offset";
            this.ReactionOffsetLabel.TextChanged += new System.EventHandler(this.BoxTextChanged);
            // 
            // VillagerLabel
            // 
            this.VillagerLabel.AutoSize = true;
            this.VillagerLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerLabel.ForeColor = System.Drawing.Color.White;
            this.VillagerLabel.Location = new System.Drawing.Point(8, 202);
            this.VillagerLabel.Name = "VillagerLabel";
            this.VillagerLabel.Size = new System.Drawing.Size(79, 19);
            this.VillagerLabel.TabIndex = 49;
            this.VillagerLabel.Text = "Villager 1";
            // 
            // VillagerOffsetLabel
            // 
            this.VillagerOffsetLabel.AutoSize = true;
            this.VillagerOffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerOffsetLabel.ForeColor = System.Drawing.Color.White;
            this.VillagerOffsetLabel.Location = new System.Drawing.Point(8, 239);
            this.VillagerOffsetLabel.Name = "VillagerOffsetLabel";
            this.VillagerOffsetLabel.Size = new System.Drawing.Size(162, 19);
            this.VillagerOffsetLabel.TabIndex = 50;
            this.VillagerOffsetLabel.Text = "Villager Offest (Size)";
            // 
            // VillagerHouse
            // 
            this.VillagerHouse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VillagerHouse.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VillagerHouse.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouse.ForeColor = System.Drawing.Color.White;
            this.VillagerHouse.Location = new System.Drawing.Point(306, 272);
            this.VillagerHouse.MaxLength = 8;
            this.VillagerHouse.Name = "VillagerHouse";
            this.VillagerHouse.Size = new System.Drawing.Size(180, 31);
            this.VillagerHouse.TabIndex = 51;
            this.VillagerHouse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VillagerHouse.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.VillagerHouse.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // VillagerHouseLabel
            // 
            this.VillagerHouseLabel.AutoSize = true;
            this.VillagerHouseLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouseLabel.ForeColor = System.Drawing.Color.White;
            this.VillagerHouseLabel.Location = new System.Drawing.Point(8, 276);
            this.VillagerHouseLabel.Name = "VillagerHouseLabel";
            this.VillagerHouseLabel.Size = new System.Drawing.Size(133, 19);
            this.VillagerHouseLabel.TabIndex = 52;
            this.VillagerHouseLabel.Text = "Villager 1 House";
            // 
            // VillagerHouseOffsetLabel
            // 
            this.VillagerHouseOffsetLabel.AutoSize = true;
            this.VillagerHouseOffsetLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouseOffsetLabel.ForeColor = System.Drawing.Color.White;
            this.VillagerHouseOffsetLabel.Location = new System.Drawing.Point(8, 313);
            this.VillagerHouseOffsetLabel.Name = "VillagerHouseOffsetLabel";
            this.VillagerHouseOffsetLabel.Size = new System.Drawing.Size(216, 19);
            this.VillagerHouseOffsetLabel.TabIndex = 53;
            this.VillagerHouseOffsetLabel.Text = "Villager House Offest (Size)";
            // 
            // VillagerHouseOffset
            // 
            this.VillagerHouseOffset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VillagerHouseOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VillagerHouseOffset.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouseOffset.ForeColor = System.Drawing.Color.White;
            this.VillagerHouseOffset.Location = new System.Drawing.Point(306, 309);
            this.VillagerHouseOffset.MaxLength = 8;
            this.VillagerHouseOffset.Name = "VillagerHouseOffset";
            this.VillagerHouseOffset.Size = new System.Drawing.Size(180, 31);
            this.VillagerHouseOffset.TabIndex = 54;
            this.VillagerHouseOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VillagerHouseOffset.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.VillagerHouseOffset.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // VillagerHouseBufferLabel
            // 
            this.VillagerHouseBufferLabel.AutoSize = true;
            this.VillagerHouseBufferLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouseBufferLabel.ForeColor = System.Drawing.Color.White;
            this.VillagerHouseBufferLabel.Location = new System.Drawing.Point(8, 350);
            this.VillagerHouseBufferLabel.Name = "VillagerHouseBufferLabel";
            this.VillagerHouseBufferLabel.Size = new System.Drawing.Size(220, 19);
            this.VillagerHouseBufferLabel.TabIndex = 55;
            this.VillagerHouseBufferLabel.Text = "Villager House Buffer Offest";
            // 
            // VillagerHouseBuffer
            // 
            this.VillagerHouseBuffer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VillagerHouseBuffer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VillagerHouseBuffer.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VillagerHouseBuffer.ForeColor = System.Drawing.Color.White;
            this.VillagerHouseBuffer.Location = new System.Drawing.Point(306, 346);
            this.VillagerHouseBuffer.MaxLength = 8;
            this.VillagerHouseBuffer.Name = "VillagerHouseBuffer";
            this.VillagerHouseBuffer.Size = new System.Drawing.Size(180, 31);
            this.VillagerHouseBuffer.TabIndex = 56;
            this.VillagerHouseBuffer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VillagerHouseBuffer.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.VillagerHouseBuffer.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // StaminaLabel
            // 
            this.StaminaLabel.AutoSize = true;
            this.StaminaLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StaminaLabel.ForeColor = System.Drawing.Color.White;
            this.StaminaLabel.Location = new System.Drawing.Point(597, 11);
            this.StaminaLabel.Name = "StaminaLabel";
            this.StaminaLabel.Size = new System.Drawing.Size(71, 19);
            this.StaminaLabel.TabIndex = 57;
            this.StaminaLabel.Text = "Stamina";
            // 
            // Stamina
            // 
            this.Stamina.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.Stamina.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Stamina.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Stamina.ForeColor = System.Drawing.Color.White;
            this.Stamina.Location = new System.Drawing.Point(774, 4);
            this.Stamina.MaxLength = 8;
            this.Stamina.Name = "Stamina";
            this.Stamina.Size = new System.Drawing.Size(180, 31);
            this.Stamina.TabIndex = 58;
            this.Stamina.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Stamina.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.Stamina.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // WalkSpeedLabel
            // 
            this.WalkSpeedLabel.AutoSize = true;
            this.WalkSpeedLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WalkSpeedLabel.ForeColor = System.Drawing.Color.White;
            this.WalkSpeedLabel.Location = new System.Drawing.Point(597, 48);
            this.WalkSpeedLabel.Name = "WalkSpeedLabel";
            this.WalkSpeedLabel.Size = new System.Drawing.Size(98, 19);
            this.WalkSpeedLabel.TabIndex = 59;
            this.WalkSpeedLabel.Text = "Walk Speed";
            // 
            // AnimationSpeedLabel
            // 
            this.AnimationSpeedLabel.AutoSize = true;
            this.AnimationSpeedLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AnimationSpeedLabel.ForeColor = System.Drawing.Color.White;
            this.AnimationSpeedLabel.Location = new System.Drawing.Point(597, 85);
            this.AnimationSpeedLabel.Name = "AnimationSpeedLabel";
            this.AnimationSpeedLabel.Size = new System.Drawing.Size(139, 19);
            this.AnimationSpeedLabel.TabIndex = 60;
            this.AnimationSpeedLabel.Text = "Animation Speed";
            // 
            // WalkSpeed
            // 
            this.WalkSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.WalkSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WalkSpeed.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WalkSpeed.ForeColor = System.Drawing.Color.White;
            this.WalkSpeed.Location = new System.Drawing.Point(774, 41);
            this.WalkSpeed.MaxLength = 8;
            this.WalkSpeed.Name = "WalkSpeed";
            this.WalkSpeed.Size = new System.Drawing.Size(180, 31);
            this.WalkSpeed.TabIndex = 61;
            this.WalkSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WalkSpeed.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.WalkSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // AnimationSpeed
            // 
            this.AnimationSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.AnimationSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AnimationSpeed.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AnimationSpeed.ForeColor = System.Drawing.Color.White;
            this.AnimationSpeed.Location = new System.Drawing.Point(774, 78);
            this.AnimationSpeed.MaxLength = 8;
            this.AnimationSpeed.Name = "AnimationSpeed";
            this.AnimationSpeed.Size = new System.Drawing.Size(180, 31);
            this.AnimationSpeed.TabIndex = 62;
            this.AnimationSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AnimationSpeed.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.AnimationSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // CollisionLabel
            // 
            this.CollisionLabel.AutoSize = true;
            this.CollisionLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CollisionLabel.ForeColor = System.Drawing.Color.White;
            this.CollisionLabel.Location = new System.Drawing.Point(597, 122);
            this.CollisionLabel.Name = "CollisionLabel";
            this.CollisionLabel.Size = new System.Drawing.Size(76, 19);
            this.CollisionLabel.TabIndex = 63;
            this.CollisionLabel.Text = "Collision";
            // 
            // Collision
            // 
            this.Collision.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.Collision.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Collision.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Collision.ForeColor = System.Drawing.Color.White;
            this.Collision.Location = new System.Drawing.Point(774, 115);
            this.Collision.MaxLength = 8;
            this.Collision.Name = "Collision";
            this.Collision.Size = new System.Drawing.Size(180, 31);
            this.Collision.TabIndex = 64;
            this.Collision.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Collision.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.Collision.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // FreezeTimeLabel
            // 
            this.FreezeTimeLabel.AutoSize = true;
            this.FreezeTimeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeTimeLabel.ForeColor = System.Drawing.Color.White;
            this.FreezeTimeLabel.Location = new System.Drawing.Point(597, 176);
            this.FreezeTimeLabel.Name = "FreezeTimeLabel";
            this.FreezeTimeLabel.Size = new System.Drawing.Size(102, 19);
            this.FreezeTimeLabel.TabIndex = 65;
            this.FreezeTimeLabel.Text = "Freeze Time";
            // 
            // FreezeTime
            // 
            this.FreezeTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.FreezeTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FreezeTime.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeTime.ForeColor = System.Drawing.Color.White;
            this.FreezeTime.Location = new System.Drawing.Point(774, 169);
            this.FreezeTime.MaxLength = 8;
            this.FreezeTime.Name = "FreezeTime";
            this.FreezeTime.Size = new System.Drawing.Size(180, 31);
            this.FreezeTime.TabIndex = 66;
            this.FreezeTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FreezeTime.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.FreezeTime.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // ReadTimeLabel
            // 
            this.ReadTimeLabel.AutoSize = true;
            this.ReadTimeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ReadTimeLabel.ForeColor = System.Drawing.Color.White;
            this.ReadTimeLabel.Location = new System.Drawing.Point(597, 213);
            this.ReadTimeLabel.Name = "ReadTimeLabel";
            this.ReadTimeLabel.Size = new System.Drawing.Size(90, 19);
            this.ReadTimeLabel.TabIndex = 67;
            this.ReadTimeLabel.Text = "Read Time";
            // 
            // ReadTime
            // 
            this.ReadTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.ReadTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ReadTime.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ReadTime.ForeColor = System.Drawing.Color.White;
            this.ReadTime.Location = new System.Drawing.Point(774, 206);
            this.ReadTime.MaxLength = 8;
            this.ReadTime.Name = "ReadTime";
            this.ReadTime.Size = new System.Drawing.Size(180, 31);
            this.ReadTime.TabIndex = 68;
            this.ReadTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReadTime.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.ReadTime.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // RecyclingBinLabel
            // 
            this.RecyclingBinLabel.AutoSize = true;
            this.RecyclingBinLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RecyclingBinLabel.ForeColor = System.Drawing.Color.White;
            this.RecyclingBinLabel.Location = new System.Drawing.Point(8, 395);
            this.RecyclingBinLabel.Name = "RecyclingBinLabel";
            this.RecyclingBinLabel.Size = new System.Drawing.Size(115, 19);
            this.RecyclingBinLabel.TabIndex = 71;
            this.RecyclingBinLabel.Text = "Recycling Bin";
            // 
            // TurnipLabel
            // 
            this.TurnipLabel.AutoSize = true;
            this.TurnipLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TurnipLabel.ForeColor = System.Drawing.Color.White;
            this.TurnipLabel.Location = new System.Drawing.Point(8, 432);
            this.TurnipLabel.Name = "TurnipLabel";
            this.TurnipLabel.Size = new System.Drawing.Size(178, 19);
            this.TurnipLabel.TabIndex = 72;
            this.TurnipLabel.Text = "Turnip Purchase Price";
            // 
            // RecyclingBin
            // 
            this.RecyclingBin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.RecyclingBin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RecyclingBin.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RecyclingBin.ForeColor = System.Drawing.Color.White;
            this.RecyclingBin.Location = new System.Drawing.Point(306, 391);
            this.RecyclingBin.MaxLength = 8;
            this.RecyclingBin.Name = "RecyclingBin";
            this.RecyclingBin.Size = new System.Drawing.Size(180, 31);
            this.RecyclingBin.TabIndex = 73;
            this.RecyclingBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RecyclingBin.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.RecyclingBin.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // Turnip
            // 
            this.Turnip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.Turnip.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Turnip.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Turnip.ForeColor = System.Drawing.Color.White;
            this.Turnip.Location = new System.Drawing.Point(306, 428);
            this.Turnip.MaxLength = 8;
            this.Turnip.Name = "Turnip";
            this.Turnip.Size = new System.Drawing.Size(180, 31);
            this.Turnip.TabIndex = 74;
            this.Turnip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Turnip.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.Turnip.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // WeatherSeedLabel
            // 
            this.WeatherSeedLabel.AutoSize = true;
            this.WeatherSeedLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WeatherSeedLabel.ForeColor = System.Drawing.Color.White;
            this.WeatherSeedLabel.Location = new System.Drawing.Point(599, 316);
            this.WeatherSeedLabel.Name = "WeatherSeedLabel";
            this.WeatherSeedLabel.Size = new System.Drawing.Size(115, 19);
            this.WeatherSeedLabel.TabIndex = 77;
            this.WeatherSeedLabel.Text = "Weather Seed";
            // 
            // WeatherSeed
            // 
            this.WeatherSeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.WeatherSeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WeatherSeed.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WeatherSeed.ForeColor = System.Drawing.Color.White;
            this.WeatherSeed.Location = new System.Drawing.Point(776, 309);
            this.WeatherSeed.MaxLength = 8;
            this.WeatherSeed.Name = "WeatherSeed";
            this.WeatherSeed.Size = new System.Drawing.Size(180, 31);
            this.WeatherSeed.TabIndex = 78;
            this.WeatherSeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WeatherSeed.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.WeatherSeed.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // addresses
            // 
            this.addresses.Controls.Add(this.MapZeroLabel);
            this.addresses.Controls.Add(this.MapZero);
            this.addresses.Controls.Add(this.SaveBtn);
            this.addresses.Controls.Add(this.PlayerSlotLabel);
            this.addresses.Controls.Add(this.PlayerSlot);
            this.addresses.Controls.Add(this.Slot21Offset);
            this.addresses.Controls.Add(this.WeatherSeed);
            this.addresses.Controls.Add(this.HomeOffset);
            this.addresses.Controls.Add(this.WeatherSeedLabel);
            this.addresses.Controls.Add(this.ReactionOffset);
            this.addresses.Controls.Add(this.PlayerOffset);
            this.addresses.Controls.Add(this.Villager);
            this.addresses.Controls.Add(this.Turnip);
            this.addresses.Controls.Add(this.VillagerOffset);
            this.addresses.Controls.Add(this.RecyclingBin);
            this.addresses.Controls.Add(this.PlayerOffsetLabel);
            this.addresses.Controls.Add(this.TurnipLabel);
            this.addresses.Controls.Add(this.Slot21OffsetLabel);
            this.addresses.Controls.Add(this.RecyclingBinLabel);
            this.addresses.Controls.Add(this.HomeOffsetLabel);
            this.addresses.Controls.Add(this.ReactionOffsetLabel);
            this.addresses.Controls.Add(this.VillagerLabel);
            this.addresses.Controls.Add(this.ReadTime);
            this.addresses.Controls.Add(this.VillagerOffsetLabel);
            this.addresses.Controls.Add(this.ReadTimeLabel);
            this.addresses.Controls.Add(this.VillagerHouse);
            this.addresses.Controls.Add(this.FreezeTime);
            this.addresses.Controls.Add(this.VillagerHouseLabel);
            this.addresses.Controls.Add(this.FreezeTimeLabel);
            this.addresses.Controls.Add(this.VillagerHouseOffsetLabel);
            this.addresses.Controls.Add(this.Collision);
            this.addresses.Controls.Add(this.VillagerHouseOffset);
            this.addresses.Controls.Add(this.CollisionLabel);
            this.addresses.Controls.Add(this.VillagerHouseBufferLabel);
            this.addresses.Controls.Add(this.AnimationSpeed);
            this.addresses.Controls.Add(this.VillagerHouseBuffer);
            this.addresses.Controls.Add(this.WalkSpeed);
            this.addresses.Controls.Add(this.StaminaLabel);
            this.addresses.Controls.Add(this.AnimationSpeedLabel);
            this.addresses.Controls.Add(this.Stamina);
            this.addresses.Controls.Add(this.WalkSpeedLabel);
            this.addresses.Enabled = false;
            this.addresses.Location = new System.Drawing.Point(0, 40);
            this.addresses.Name = "addresses";
            this.addresses.Size = new System.Drawing.Size(968, 466);
            this.addresses.TabIndex = 98;
            // 
            // MapZeroLabel
            // 
            this.MapZeroLabel.AutoSize = true;
            this.MapZeroLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MapZeroLabel.ForeColor = System.Drawing.Color.White;
            this.MapZeroLabel.Location = new System.Drawing.Point(599, 353);
            this.MapZeroLabel.Name = "MapZeroLabel";
            this.MapZeroLabel.Size = new System.Drawing.Size(79, 19);
            this.MapZeroLabel.TabIndex = 101;
            this.MapZeroLabel.Text = "Map Zero";
            // 
            // MapZero
            // 
            this.MapZero.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.MapZero.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MapZero.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MapZero.ForeColor = System.Drawing.Color.White;
            this.MapZero.Location = new System.Drawing.Point(776, 346);
            this.MapZero.MaxLength = 8;
            this.MapZero.Name = "MapZero";
            this.MapZero.Size = new System.Drawing.Size(180, 31);
            this.MapZero.TabIndex = 100;
            this.MapZero.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MapZero.TextChanged += new System.EventHandler(this.BoxTextChanged);
            this.MapZero.Validating += new System.ComponentModel.CancelEventHandler(this.BoxValidating);
            // 
            // SaveBtn
            // 
            this.SaveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.SaveBtn.FlatAppearance.BorderSize = 0;
            this.SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveBtn.ForeColor = System.Drawing.Color.White;
            this.SaveBtn.Location = new System.Drawing.Point(774, 423);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(180, 30);
            this.SaveBtn.TabIndex = 99;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = false;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // ImageBtn
            // 
            this.ImageBtn.BackColor = System.Drawing.Color.Orange;
            this.ImageBtn.FlatAppearance.BorderSize = 0;
            this.ImageBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ImageBtn.ForeColor = System.Drawing.Color.White;
            this.ImageBtn.Location = new System.Drawing.Point(780, 5);
            this.ImageBtn.Name = "ImageBtn";
            this.ImageBtn.Size = new System.Drawing.Size(180, 30);
            this.ImageBtn.TabIndex = 100;
            this.ImageBtn.Text = "Download Image";
            this.ImageBtn.UseVisualStyleBackColor = false;
            this.ImageBtn.Click += new System.EventHandler(this.ImageBtn_Click);
            // 
            // AddressOverrideToggle
            // 
            this.AddressOverrideToggle.Location = new System.Drawing.Point(143, 9);
            this.AddressOverrideToggle.Name = "AddressOverrideToggle";
            this.AddressOverrideToggle.OffFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AddressOverrideToggle.OffForeColor = System.Drawing.Color.White;
            this.AddressOverrideToggle.OffText = "Off";
            this.AddressOverrideToggle.OnFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AddressOverrideToggle.OnForeColor = System.Drawing.Color.White;
            this.AddressOverrideToggle.OnText = "On";
            this.AddressOverrideToggle.Size = new System.Drawing.Size(59, 19);
            this.AddressOverrideToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.AddressOverrideToggle.TabIndex = 101;
            this.AddressOverrideToggle.UseAnimation = false;
            this.AddressOverrideToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.AddressOverrideToggle_CheckedChanged);
            // 
            // AddressOverrideLabel
            // 
            this.AddressOverrideLabel.AutoSize = true;
            this.AddressOverrideLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AddressOverrideLabel.ForeColor = System.Drawing.Color.White;
            this.AddressOverrideLabel.Location = new System.Drawing.Point(10, 11);
            this.AddressOverrideLabel.Name = "AddressOverrideLabel";
            this.AddressOverrideLabel.Size = new System.Drawing.Size(132, 16);
            this.AddressOverrideLabel.TabIndex = 102;
            this.AddressOverrideLabel.Text = "Address Override";
            // 
            // ValidationLabel
            // 
            this.ValidationLabel.AutoSize = true;
            this.ValidationLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ValidationLabel.ForeColor = System.Drawing.Color.White;
            this.ValidationLabel.Location = new System.Drawing.Point(238, 11);
            this.ValidationLabel.Name = "ValidationLabel";
            this.ValidationLabel.Size = new System.Drawing.Size(75, 16);
            this.ValidationLabel.TabIndex = 104;
            this.ValidationLabel.Text = "Validation";
            // 
            // ValidationToggle
            // 
            this.ValidationToggle.Location = new System.Drawing.Point(316, 9);
            this.ValidationToggle.Name = "ValidationToggle";
            this.ValidationToggle.OffFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ValidationToggle.OffForeColor = System.Drawing.Color.White;
            this.ValidationToggle.OffText = "Off";
            this.ValidationToggle.OnFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ValidationToggle.OnForeColor = System.Drawing.Color.White;
            this.ValidationToggle.OnText = "On";
            this.ValidationToggle.Size = new System.Drawing.Size(59, 19);
            this.ValidationToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.ValidationToggle.TabIndex = 103;
            this.ValidationToggle.UseAnimation = false;
            this.ValidationToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.ValidationToggle_CheckedChanged);
            // 
            // SoundToggle
            // 
            this.SoundToggle.Location = new System.Drawing.Point(469, 9);
            this.SoundToggle.Name = "SoundToggle";
            this.SoundToggle.OffFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SoundToggle.OffForeColor = System.Drawing.Color.White;
            this.SoundToggle.OffText = "Off";
            this.SoundToggle.OnFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SoundToggle.OnForeColor = System.Drawing.Color.White;
            this.SoundToggle.OnText = "On";
            this.SoundToggle.Size = new System.Drawing.Size(59, 19);
            this.SoundToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.SoundToggle.TabIndex = 105;
            this.SoundToggle.UseAnimation = false;
            this.SoundToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.SoundToggle_CheckedChanged);
            // 
            // SoundLabel
            // 
            this.SoundLabel.AutoSize = true;
            this.SoundLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SoundLabel.ForeColor = System.Drawing.Color.White;
            this.SoundLabel.Location = new System.Drawing.Point(414, 11);
            this.SoundLabel.Name = "SoundLabel";
            this.SoundLabel.Size = new System.Drawing.Size(52, 16);
            this.SoundLabel.TabIndex = 106;
            this.SoundLabel.Text = "Sound";
            // 
            // CaptureToggle
            // 
            this.CaptureToggle.Location = new System.Drawing.Point(628, 9);
            this.CaptureToggle.Name = "CaptureToggle";
            this.CaptureToggle.OffFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CaptureToggle.OffForeColor = System.Drawing.Color.White;
            this.CaptureToggle.OffText = "Off";
            this.CaptureToggle.OnFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CaptureToggle.OnForeColor = System.Drawing.Color.White;
            this.CaptureToggle.OnText = "On";
            this.CaptureToggle.Size = new System.Drawing.Size(59, 19);
            this.CaptureToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.CaptureToggle.TabIndex = 107;
            this.CaptureToggle.UseAnimation = false;
            this.CaptureToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.CaptureToggle_CheckedChanged);
            // 
            // CaptureLabel
            // 
            this.CaptureLabel.AutoSize = true;
            this.CaptureLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CaptureLabel.ForeColor = System.Drawing.Color.White;
            this.CaptureLabel.Location = new System.Drawing.Point(561, 11);
            this.CaptureLabel.Name = "CaptureLabel";
            this.CaptureLabel.Size = new System.Drawing.Size(63, 16);
            this.CaptureLabel.TabIndex = 108;
            this.CaptureLabel.Text = "Capture";
            // 
            // Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(964, 506);
            this.Controls.Add(this.CaptureLabel);
            this.Controls.Add(this.CaptureToggle);
            this.Controls.Add(this.SoundToggle);
            this.Controls.Add(this.SoundLabel);
            this.Controls.Add(this.ValidationToggle);
            this.Controls.Add(this.AddressOverrideToggle);
            this.Controls.Add(this.ValidationLabel);
            this.Controls.Add(this.AddressOverrideLabel);
            this.Controls.Add(this.ImageBtn);
            this.Controls.Add(this.addresses);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(980, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(980, 545);
            this.Name = "Setting";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.Setting_Load);
            this.addresses.ResumeLayout(false);
            this.addresses.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PlayerSlotLabel;
        private System.Windows.Forms.TextBox PlayerSlot;
        private System.Windows.Forms.TextBox Slot21Offset;
        private System.Windows.Forms.TextBox HomeOffset;
        private System.Windows.Forms.TextBox ReactionOffset;
        private System.Windows.Forms.TextBox PlayerOffset;
        private System.Windows.Forms.TextBox Villager;
        private System.Windows.Forms.TextBox VillagerOffset;
        private System.Windows.Forms.Label PlayerOffsetLabel;
        private System.Windows.Forms.Label Slot21OffsetLabel;
        private System.Windows.Forms.Label HomeOffsetLabel;
        private System.Windows.Forms.Label ReactionOffsetLabel;
        private System.Windows.Forms.Label VillagerLabel;
        private System.Windows.Forms.Label VillagerOffsetLabel;
        private System.Windows.Forms.TextBox VillagerHouse;
        private System.Windows.Forms.Label VillagerHouseLabel;
        private System.Windows.Forms.Label VillagerHouseOffsetLabel;
        private System.Windows.Forms.TextBox VillagerHouseOffset;
        private System.Windows.Forms.Label VillagerHouseBufferLabel;
        private System.Windows.Forms.TextBox VillagerHouseBuffer;
        private System.Windows.Forms.Label StaminaLabel;
        private System.Windows.Forms.TextBox Stamina;
        private System.Windows.Forms.Label WalkSpeedLabel;
        private System.Windows.Forms.Label AnimationSpeedLabel;
        private System.Windows.Forms.TextBox WalkSpeed;
        private System.Windows.Forms.TextBox AnimationSpeed;
        private System.Windows.Forms.Label CollisionLabel;
        private System.Windows.Forms.TextBox Collision;
        private System.Windows.Forms.Label FreezeTimeLabel;
        private System.Windows.Forms.TextBox FreezeTime;
        private System.Windows.Forms.Label ReadTimeLabel;
        private System.Windows.Forms.TextBox ReadTime;
        private System.Windows.Forms.Label RecyclingBinLabel;
        private System.Windows.Forms.Label TurnipLabel;
        private System.Windows.Forms.TextBox RecyclingBin;
        private System.Windows.Forms.TextBox Turnip;
        private System.Windows.Forms.Label WeatherSeedLabel;
        private System.Windows.Forms.TextBox WeatherSeed;
        private System.Windows.Forms.Panel addresses;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button ImageBtn;
        private System.Windows.Forms.Label MapZeroLabel;
        private System.Windows.Forms.TextBox MapZero;
        private JCS.ToggleSwitch AddressOverrideToggle;
        private System.Windows.Forms.Label AddressOverrideLabel;
        private System.Windows.Forms.Label ValidationLabel;
        private JCS.ToggleSwitch ValidationToggle;
        private JCS.ToggleSwitch SoundToggle;
        private System.Windows.Forms.Label SoundLabel;
        private JCS.ToggleSwitch CaptureToggle;
        private System.Windows.Forms.Label CaptureLabel;
    }
}