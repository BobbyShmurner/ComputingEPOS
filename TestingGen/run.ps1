param (
	[Switch] $Debug
)

$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Testing"
if ($Debug) {
	$path = "$PSScriptRoot/main.py $PSScriptRoot/docs/Debug"	
}

Start-Process python -ArgumentList $path -NoNewWindow -Wait && Start-Process out.docx