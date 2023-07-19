// File: C:\Users\tsell\Desktop\Scripts\Dock SIgn.cs

public Program()
{
    // Run every 100 ticks (60 ticks is 1 second)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main()
{
    DisplayItemSign();
    DisplayDockSign();
    DisplayUnloadSign();

    Me.GetSurface(0).WriteText("Dock Item Sign");
}

public void DisplayItemSign()
{
    // The name of your LCD Panel
    string lcdName = "Items Sign";

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
    MySprite sprite = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "AH_PullUp", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X / 2, lcd.SurfaceSize.Y - 400), // Position in the middle
        Size = new Vector2(128, 128), // 128x128 size
        Color = Color.Green, // Green color
        Alignment = TextAlignment.CENTER, // Center alignment,
        RotationOrScale = 3.14f
    };

    MySprite text = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = "Items",
        Position = new Vector2(lcd.SurfaceSize.X / 2, lcd.SurfaceSize.Y - 300),
        RotationOrScale = 4f /* 200 % of the font's default size */,
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

public void DisplayUnloadSign()
{
    IMyTextSurface lcd = GridTerminalSystem.GetBlockWithName("Unload Sign") as IMyTextSurface;
    if (lcd == null)
    {
        Echo("No Unload sign found");
        return;
    }
    // Set the LCD to use the SPRITE mode
    lcd.ContentType = ContentType.SCRIPT;
    lcd.Script = "";
    // Set the background color to Black
    lcd.ScriptBackgroundColor = Color.Black;

    var frame = lcd.DrawFrame();

    MySprite sprite = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "AH_PullUp", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X - 400, lcd.SurfaceSize.Y - 400), // Position in the middle
        Size = new Vector2(128, 128), // 128x128 size
        Color = Color.Green, // Green color
        Alignment = TextAlignment.CENTER, // Center alignment,
        RotationOrScale = 3.7f
    };
    MySprite text = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = "Unload",
        Position = new Vector2(lcd.SurfaceSize.X / 2, lcd.SurfaceSize.Y - 300),
        RotationOrScale = 4f /* 200 % of the font's default size */,
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

public void DisplayDockSign()
{
    List<IMyTextSurface> DockScreens = new List<IMyTextSurface> {
        GridTerminalSystem.GetBlockWithName("Dock 1 Screen") as IMyTextSurface,
        GridTerminalSystem.GetBlockWithName("Dock 2 Screen") as IMyTextSurface,
        GridTerminalSystem.GetBlockWithName("Dock 3 Screen") as IMyTextSurface,
        GridTerminalSystem.GetBlockWithName("Dock 4 Screen") as IMyTextSurface,
        GridTerminalSystem.GetBlockWithName("Dock 5 Screen") as IMyTextSurface
    };

    foreach (var screen in DockScreens)
    {
        if (screen == null)
        {
            Echo($"Null screen found in the DockScreens list: {DockScreens.IndexOf(screen) + 1}");
            continue;
        }

        screen.FontSize = 5f;
        screen.WriteText($"Dock {DockScreens.IndexOf(screen) + 1}");
    }
}
