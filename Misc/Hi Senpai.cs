MySprite sprite;
MySprite text;
IMyTextSurface lcd;

public Program()
{
    // Run every 100 ticks (60 ticks is 1 second)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    string lcdName = "LCD Panel";

    // Get the LCD panel from the Terminal system 
    lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextSurface;

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

    sprite = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "LCD_Emote_Love", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X - 400, lcd.SurfaceSize.Y - 400), // Position in the middle
        Size = new Vector2(128, 128), // 128x128 size
        Color = Color.Green, // Green color
        Alignment = TextAlignment.CENTER // Center alignment
    };

    text = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = "Hii Senpai!!",
        Position = new Vector2(lcd.SurfaceSize.X - 200, lcd.SurfaceSize.Y - 400),
        RotationOrScale = 2f /* 200 % of the font's default size */,
        Color = Color.Green,
        Alignment = TextAlignment.CENTER /* Center the text on the position */,
        FontId = "White"
    };
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
        Data = "LCD_Emote_Love", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X - 400, lcd.SurfaceSize.Y - 400), // Position in the middle
        Size = new Vector2(128, 128), // 128x128 size
        Color = Color.Green, // Green color
        Alignment = TextAlignment.CENTER // Center alignment
    };

    var text = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = "Hii Senpai!!",
        Position = new Vector2(lcd.SurfaceSize.X - 200, lcd.SurfaceSize.Y - 400),
        RotationOrScale = 2f /* 200 % of the font's default size */,
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

