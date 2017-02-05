
////////////////////////////
//////////preround//////////
////////////////////////////


function serverCmdAddGuard(%client, %name) 
{
	if (!%client.isSuperAdmin)
		return;

	%targ = findclientbyname(%name);
	if (strPos($Server::PrisonEscape::Guards, %targ) >= 0) {
		messageAdmins("!!! \c5Cannot add \c3" @ %targ.name @ "\c5 to guard list - already in list!");
		return;
	}
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards SPC findclientbyname(%name));
	messageAdmins(%client, '', "\c7" @ %cl.name @ " added " @ findclientbyname(%name).name @ " to the guard list.");
	displayRoundLoadingInfo();
}

function serverCmdRemoveGuard(%client, %name)
{
	if (!%client.isSuperAdmin)
		return;

	%targ = findclientbyname(%name);
	if (strPos($Server::PrisonEscape::Guards, %targ) < 0) {
		messageAdmins("!!! \c5Cannot remove \c3" @ %targ.name @ "\c5 from guard list - not in list!");
		return;
	}

	%guard = findclientbyname(%name);
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, %guard, "");
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards);
	messageAdmins(%client, '', "\c7" @ %cl.name @ " removed " @ findclientbyname(%name).name @ " from the guard list.");
	displayRoundLoadingInfo();
}

function getGuardNames()
{
	%list = "<just:right>";
	for (%i = 0; %i < getWordCount($Server::PrisonEscape::Guards); %i++)
	{
		%list = %list @ "\c2" @ (getWord($Server::PrisonEscape::Guards, %i).name) @ " - Guard " @ (%i + 1) @ " <br>";
	}

	for (%i = getWordCount($Server::PrisonEscape::Guards); %i < 4; %i++)
	{
		%list = %list @ "\c6NONE - Guard " @ (%i + 1) @ " <br>";
	}

	return %list;
}

function displayRoundLoadingInfo() 
{
	%statisticString = $Server::PrisonEscape::statisticString;
	%guards1 = getGuardNames();
	%guards2 = getSubStr(%guards1, strPos(%guards1, "Guard 2 <br>") + 12, strLen(%guards1));
	%guards1 = "<just:right> \c6" @ getSubStr(%guards1, 0, strPos(%guards1, "Guard 2 <br>") + 12);
	%centerprintString = "<font:Arial Bold:20>" @ %guards1 @ "<just:center>\c3" @ %statisticString @ %guards2;
	centerprintAll(%centerprintString);
	bottomprintAll(generateBottomPrint(), -1, 1);
}

function generateBottomPrint() 
{
	%header = "<just:center><font:Arial Bold:34><shadowcolor:666666><shadow:0:4><color:E65714>JailBreak! <br><font:Arial Bold:26>\c7-      - <br>";
	%footer = "<shadow:0:3><color:ffffff>Please wait until the next round to play<font:Impact:1> <br>";
	return %header @ %footer;
}

function swapStatistics() 
{
	if (isEventPending($Server::PrisonEscape::statisticLoop))
		return;

	%stat = getStatistic();

	$Server::PrisonEscape::statisticString = %stat;
	$Server::PrisonEscape::currentStatistic++;
	$Server::PrisonEscape::currentStatistic %= 12;
	$Server::PrisonEscape::statisticLoop = schedule(6000, 0, swapStatistics);
	displayRoundLoadingInfo();
}