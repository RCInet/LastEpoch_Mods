using System;
using Il2CppRewired;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Walks the active joystick's IGamepadTemplate to check button states
    internal static class KeybindRewired
    {
        private static readonly (string Name, Func<IGamepadTemplate, IControllerTemplateButton> Get)[] Buttons =
            new (string, Func<IGamepadTemplate, IControllerTemplateButton>)[]
            {
                ("A", t => t.a),
                ("B", t => t.b),
                ("X", t => t.x),
                ("Y", t => t.y),
                ("LB", t => t.leftBumper),
                ("RB", t => t.rightBumper),
                ("Back", t => t.back),
                ("Start", t => t.start),
                ("Guide", t => t.guide),
                ("LStick", t => t.leftStick?.press),
                ("RStick", t => t.rightStick?.press),
                ("DPadUp", t => t.dPad?.up),
                ("DPadDown", t => t.dPad?.down),
                ("DPadLeft", t => t.dPad?.left),
                ("DPadRight", t => t.dPad?.right),
            };

        public static int ButtonCount => Buttons.Length;
        public static string ButtonNameAt(int index) => Buttons[index].Name;

        public static IGamepadTemplate ActiveTemplate()
        {
            try
            {
                var rp = Refs_Manager.epoch_input_manager?.rewiredPlayer;
                if (rp == null || rp.IsNullOrDestroyed()) return null;
                var joystick = rp.controllers.GetLastActiveController(ControllerType.Joystick);
                if (joystick == null) return null;
                var template = joystick.GetTemplate<GamepadTemplate>();
                if (template == null) return null;
                return template.Cast<IGamepadTemplate>();
            }
            catch { return null; }
        }

        public static bool ButtonHeldAt(IGamepadTemplate template, int index)
        {
            if (template == null) return false;
            try
            {
                var btn = Buttons[index].Get(template);
                return btn != null && btn.value;
            }
            catch { return false; }
        }

        public static bool ButtonHeldByName(string elementName)
        {
            var template = ActiveTemplate();
            if (template == null) return false;
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Buttons[i].Name == elementName)
                    return ButtonHeldAt(template, i);
            }
            return false;
        }
    }
}
