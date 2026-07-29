$response = Invoke-RestMethod -Uri "http://localhost:5122/api/store/units?RecordPerPage=100"
$response.data.items | ForEach-Object {
    [PSCustomObject]@{
        UnitId = $_.unitId
        UnitName = $_.unitName
        BaseUnit = $_.baseUnit
    }
} | Format-Table
