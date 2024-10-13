using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.JumpDelayMeter;

public class JumpDelayMeterModule : EverestModule {
    public static JumpDelayMeterModule Instance { get; private set; }

    public override Type SettingsType => typeof(JumpDelayMeterModuleSettings);
    public static JumpDelayMeterModuleSettings Settings => (JumpDelayMeterModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(JumpDelayMeterModuleSession);
    public static JumpDelayMeterModuleSession Session => (JumpDelayMeterModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(JumpDelayMeterModuleSaveData);
    public static JumpDelayMeterModuleSaveData SaveData => (JumpDelayMeterModuleSaveData) Instance._SaveData;

    private static float jumpDelay;

    public JumpDelayMeterModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(JumpDelayMeterModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(JumpDelayMeterModule), LogLevel.Info);
#endif
    }

    public override void Load(){
        On.Celeste.Player.SuperJump += modPlayerSuperJump;
        On.Celeste.SpeedrunTimerDisplay.Render += modSpeedRunTimerDisplayRender;
    }

    public override void Unload() {
        On.Celeste.Player.SuperJump -= modPlayerSuperJump;
        On.Celeste.SpeedrunTimerDisplay.Render -= modSpeedRunTimerDisplayRender;
    }
    
    private static void modPlayerSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self){
        jumpDelay = self.dashRefillCooldownTimer;
        orig(self);
    }

    private static void modSpeedRunTimerDisplayRender(On.Celeste.SpeedrunTimerDisplay.orig_Render orig, SpeedrunTimerDisplay self){
        orig(self);
        MTexture bar = GFX.Gui["tamion/jumpdelaybar"];
        MTexture pointer = GFX.Gui["tamion/pointer"];

        Vector2 barLocation = new Vector2(Celeste.Instance.Window.ClientBounds.Width / 2f,
            Celeste.Instance.Window.ClientBounds.Height * 0.95f);
        bar.DrawCentered(barLocation, Color.White, Vector2.One*8f);
        Vector2 pointerLocation = new Vector2(barLocation.X + 130f - 3000*float.Max(jumpDelay, 0f), barLocation.Y);
        pointer.DrawCentered(pointerLocation, Color.White, Vector2.One);
    }
}