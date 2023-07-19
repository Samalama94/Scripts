using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;

using System.Collections.Generic;

using Sandbox.ModAPI;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using IMyCockpit = Sandbox.ModAPI.Ingame.IMyCockpit;
using IMyTextPanel = Sandbox.ModAPI.Ingame.IMyTextPanel;
using IMyTextSurface = Sandbox.ModAPI.Ingame.IMyTextSurface;
namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.


        public Program()
        {
            // Run every 100 ticks (60 ticks is 1 second)
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main()
        {
            // The name of your LCD Panel
            string lcdName = "LCD Panel";

            // Get the LCD panel from the Terminal system 
            IMyTextSurface lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextSurface;

            if (lcd == null)
            {
                Echo("No LCD found named: " + lcdName);
                return;
            }

            // Set the LCD to use the SPRITE mode
            lcd.ContentType = ContentType.SCRIPT;
            lcd.Script = "";
            // Set the background color to Black
            lcd.ScriptBackgroundColor = Color.Black;

            // Create the frame
            var frame = lcd.DrawFrame();
            
            // Define the sprite
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "AH_PullUp", // The sprite texture
                Position = new Vector2(lcd.SurfaceSize.X-400, lcd.SurfaceSize.Y -400), // Position in the middle
                Size = new Vector2(128, 128), // 128x128 size
                Color = Color.Green, // Green color
                Alignment = TextAlignment.CENTER, // Center alignment,
                RotationOrScale = 180
            };

            var text = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Items",
                Position = new Vector2(lcd.SurfaceSize.X - 200,lcd.SurfaceSize.Y -400),
                RotationOrScale = 4f /* 400 % of the font's default size */,
                Color = Color.Green,
                Alignment = TextAlignment.CENTER /* Center the text on the position */,
                FontId = "White"
            };


            List<MySprite> spritesList = new List<MySprite>
            {
                sprite,
                text
            };
            // Add the sprite to the frame
            frame.AddRange(spritesList);
            // Draw the frame
            frame.Dispose();
            
        }

    }
}
