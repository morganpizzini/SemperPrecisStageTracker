$directory = ".\compiled\"
$results = ".\results\"
if (Test-Path $directory) {
    Write-Host "Folder Exists"
}
else
{
    #PowerShell Create directory if not exists
    New-Item $directory -ItemType Directory
    Write-Host "Folder Created successfully"
}
if (Test-Path $results) {
    Write-Host "Folder Exists"
}
else
{
    #PowerShell Create directory if not exists
    New-Item $results -ItemType Directory
    Write-Host "Folder Created successfully"
}
Get-ChildItem -Path $directory -Include *.* -File -Recurse | foreach { $_.Delete()}
Get-ChildItem -Path $results -Include *.* -File -Recurse | foreach { $_.Delete()}
Get-ChildItem -Path .\ -Filter *.txt -File -Name| ForEach-Object {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($_)
    $lines = Get-Content ".\$name.txt"
    $langs = @()
    for ($counter=0; $counter -lt $lines.Length; $counter++){
        if($counter -eq 0){
            # create files
            New-Item "$directory$name.txt"
            $stringArray = $lines[$counter].Split(",")
            foreach ($item in $stringArray) {
                $langs += $item
                New-Item "$directory$name.$item.txt"
            }
            continue
        }

        $content = $lines[$counter].Split("=")
        $baseString = $content[0]
        $translations = $content[1].Split(",")
        #foreach ($trans in $translations) {
        for ($counter1=0; $counter1 -lt $translations.Length; $counter1++){
            $trans = $translations[$counter1]
            $compiledString = "$baseString=$trans"
            # add new line
            if($counter -eq 0){
                $compiledString = "`n$compiledString"
            }
            # set current file
            if($counter1 -eq 0){
                $currentFile = "$directory$name.txt"
            }else{
                $currentFile = "$directory$name.$item.txt"
            }
            Add-Content $currentFile $compiledString
        }

        echo $lines[$counter]
    }
}

Get-ChildItem -Path $directory -Filter *.txt -Recurse -File -Name| ForEach-Object {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($_)
    resgen "$directory$name.txt" "$results$name.resx"
}