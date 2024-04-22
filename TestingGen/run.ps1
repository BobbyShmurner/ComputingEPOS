param (
	[Switch] $Debug,
	[String] $Covers 
)

if ($Covers) {
	Start-Process python -ArgumentList "$PSScriptRoot/main.py --covers $Covers" -NoNewWindow -Wait
	exit(0)
}

$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Testing"
if ($Debug) {
	$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Debug"	
}

Start-Process python -ArgumentList $path -NoNewWindow -Wait && Start-Process out.docx