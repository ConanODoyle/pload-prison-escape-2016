$Server::PrisonEscape::Guards = "";

function serverCmdAddGuard(%client, %name) 
{
	if (!%client.isSuperAdmin)
		return;

	$Server::PrisonEscape::Guards = $Server::PrisonEscape::Guards @ findclientbyname(%name);
	messageClient(%client, '', "\c7Added " @ %findclientbyname(%name).name @ " to the guard list.");
}

function serverCmdRemoveGuard(%client, %name)
{
	if (!%client.isSuperAdmin)
		return;

	%guard = findclientbyname(%name);
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, %guard, "");
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards);
}

function getGuardNames()
{
	%list = "<just:right>";
	for (%i = 0; %i < getWordCount($Server::PrisonEscape::Guards); %i++)
	{
		%list = %list @ getWord($Server::PrisonEscape::Guards, %i).name @ " - Guard " @ (%i + 1) @ " <br>");
	}

	for (%i = getWordCount($Server::PrisonEscape::Guards); %i < 4; %i++)
	{
		%list = %list @ "NONE - Guard " @ (%i + 1) @ " <br>");
	}

	return %list;
}

function generateCenterPrintTextGuards() 
{
	%statisticString = $Server::PrisonEscape::statisticString;
	%guards1 = getGuardNames();
	%guards2 = getSubStr(%guards1, strPos(%guards1, "Guard 2 <br>") + 12, strLen(%guards1));
	%guards1 = getSubStr(%guards1, 0, strPos(%guards1, "Guard 2 <br>") + 11);
	echo(%guards1);
	echo(%guards2);
	%centerprintString = "<font:Arial Bold:20>\c6" @ %guards1 @ "<just:center>" @ %statisticString @ " <just:right>" @ %guards2;
	centerprintAll(%centerprintString);
}

function generateBottomPrint() 
{
	%header = "<just:center><font:Arial Bold:24><shadow:4:4>\c6<shadowcolor:rickroll>JailBreak! <br><font:Arial Bold:20>-      - <br>";
	%footer = "<shadow:2:2>Please wait until the next round to play";
}