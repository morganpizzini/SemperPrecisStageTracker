start "API" cmd /k "cd SemperPrecisStageTracker.API && dotnet watch run"

start "Blazor" cmd /k "timeout /t 5 && cd SemperPrecisStageTracker.Blazor && dotnet watch run"