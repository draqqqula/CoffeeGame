
using System;
using System.IO;
try
{
    using var game = new CoffeeProject.CoffeeGame();
    game.Run();
}
catch (Exception ex)
{
    File.WriteAllText("error_log.txt", $"{ex.Message}, " +
        $"{ex.StackTrace}");
    throw;
}