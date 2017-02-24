
////////////////////////////
//////////preround//////////
////////////////////////////


function serverCmdAddGuard(%client, %name) 
{
	if (!%client.isSuperAdmin)
		return;

	%targ = findclientbyname(%name);
	if (!isObject(%targ)) {
		messageAdmins("!!! \c5Cannot add \c3" @ %targ.name @ "\c5 to guard list - client does not exist! \c7(" @ %client.name @ ")");
		return;
	}
	if (strPos($Server::PrisonEscape::Guards, %targ) >= 0) {
		messageAdmins("!!! \c5Cannot add \c3" @ %targ.name @ "\c5 to guard list - already in list! \c7(" @ %client.name @ ")");
		return;
	}
	if (getWordCount($Server::PrisonEscape::Guards) >= 4) {
		messageAdmins("!!! \c5Cannot add \c3" @ %targ.name @ "\c5 to guard list - list is full! \c7(" @ %client.name @ ")");
		return;
	}
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards SPC findclientbyname(%name));
	messageAll('', "\c3" @ %client.name @ " added " @ findclientbyname(%name).name @ " to the guard list.");
	displayRoundLoadingInfo();
}

function serverCmdRemoveGuard(%client, %name)
{
	if (!%client.isSuperAdmin)
		return;

	%targ = findclientbyname(%name);
	if (strPos($Server::PrisonEscape::Guards, %targ) < 0 && !(%name > 0 && %name < 5)) {
		messageAdmins("!!! \c5Cannot remove \c3" @ %targ.name @ "\c5 from guard list - not in list! \c7(" @ %client.name @ ")");
		return;
	}

	if (!(%name > 0 && %name < 5)) {
		%guard = findclientbyname(%name);
		$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, %guard, "");
		$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
		$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards);
	} else {
		%guard = getWord($Server::PrisonEscape::Guards, %name-1);
		$Server::PrisonEscape::Guards = removeWord($Server::PrisonEscape::Guards, %name-1);
		$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
		$Server::PrisonEscape::Guarsd = trim($Server::PrisonEscape::Guards);
	}
	messageAll('', "\c2" @ %client.name @ " removed " @ %guard.name @ " from the guard list.");
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
	%guards1 = " \c6" @ getSubStr(%guards1, 0, strPos(%guards1, "Guard 2 <br>") + 12);
	%centerprintString = "<font:Arial Bold:20>" @ %guards1 @ "<just:center><font:Arial Bold:22>\c3" @ %statisticString @ "<font:Arial Bold:20><just:right>" @ %guards2 @ "<br><br><br>";
	centerprintAll(%centerprintString);
	bottomprintAll(generateBottomPrint(), -1, 1);
}

function generateBottomPrint() 
{
	%header = "<just:center><font:Arial Black:48><shadowcolor:555555><shadow:0:4><color:E65714>Conan's Prison Break <br><font:Arial Bold:30>\c7-      - <br>";
	%footer = "<shadow:0:3><color:ffffff>Please wait for the next round to start<font:Impact:1> <br>";
	return %header @ %footer;
}

function swapStatistics() 
{
	if (isEventPending($Server::PrisonEscape::statisticLoop))
		return;

	%stat = getStatisticToDisplay();

	$Server::PrisonEscape::statisticString = %stat;
	$Server::PrisonEscape::statisticLoop = schedule(6000, 0, swapStatistics);
	displayRoundLoadingInfo();
}