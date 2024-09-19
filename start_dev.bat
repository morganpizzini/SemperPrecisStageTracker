start "API" cmd /k "cd SemperPrecisStageTracker.API && dotnet watch run --no-hot-reload --quiet --no-interactive"

start "Blazor" cmd /k "timeout /t 5 /nobreak && cd SemperPrecisStageTracker.Blazor && dotnet watch run --no-hot-reload --quiet --no-interactive --pathbase=/app"