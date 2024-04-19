param (
	[Switch] $Debug,
	[String] $Covers 
)

if ($Xml) {
	Start-Process python -ArgumentList "$PSScriptRoot/main.py --covers $Xml" -NoNewWindow -Wait
	exit(0)
}

$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Testing"
if ($Debug) {
	$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Debug"	
}

Start-Process python -ArgumentList $path -NoNewWindow -Wait && Start-Process out.docx