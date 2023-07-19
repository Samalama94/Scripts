

public int timesRan = 0;

public Program()
{
    // This instruction is required only if the script is run for the first time
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // Run the script every tick
}

public void Main()
{
    
    // Increase the value of the variable
    timesRan++;
    if (timesRan > 100)
    {
        timesRan= 0;
    }
    // Retrieve the first text surface (0-indexed) from the programmable block
    var surface = Me.GetSurface(0);
    surface.FontSize = 3f;
    // Clear the text from this surface
    surface.WriteText("");

    // Write the new value for timesRan
    surface.WriteText("Times Ran: " + timesRan, false);
    if (timesRan % 10 == 0)
    {
        surface.WriteText($"\nNow", true);
    }
}



/*

// this one to save between saves
  
  int timesRan;

    public Program()
    {
        if (!string.IsNullOrWhiteSpace(Storage))
        {
            timesRan = int.Parse(Storage);
        }
        else
        {
            timesRan = 0;
        }
    }
    
    public void Save()
    {
        Storage = timesRan.ToString();
    }

    public void Main(string argument)
    {
        var screen = Me.GetSurface(0);
        screen.WriteText($"Main has run {timesRan} times");
        timesRan++;
    }

    */