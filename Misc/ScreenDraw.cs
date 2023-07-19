

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

    // Set the background color to Black
    lcd.BackgroundColor = Color.Red;

    // Create the frame
    var frame = lcd.DrawFrame();

    // Define the sprite
    var sprite = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "LCD_Emote_Love", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X / 2, lcd.SurfaceSize.Y / 2), // Position in the middle
        Size = new Vector2(128, 128), // 128x128 size
        Color = Color.Green, // Green color
        Alignment = TextAlignment.CENTER // Center alignment
    };
    var background = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple", // The sprite texture
        Position = new Vector2(lcd.SurfaceSize.X / 2, lcd.SurfaceSize.Y / 2), // Position in the middle
        Size = new Vector2(1024, 1024), // 1024x1024 size
        Color = Color.Black, // Green color
        Alignment = TextAlignment.CENTER // Center alignment
    };



    // Add the sprite to the frame
    frame.Add(background);
    frame.Add(sprite);

    // Draw the frame
    frame.Dispose();
}