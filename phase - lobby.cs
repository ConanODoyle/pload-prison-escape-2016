
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
	// %guards1 = getGuardNames();
	// %guards2 = getSubStr(%guards1, strPos(%guards1, "Guard 2 <br>") + 12, strLen(%guards1));
	// %guards1 = " \c6" @ getSubStr(%guards1, 0, strPos(%guards1, "Guard 2 <br>") + 12);
	// %centerprintString = "<font:Arial Bold:20>" @ %guards1 @ "<just:center><font:Arial Bold:22>\c3" @ %statisticString @ "<font:Arial Bold:20><just:right>" @ %guards2 @ "<br><br><br>";
	// centerprintAll(%centerprintString);

	if (!isObject($PrisonEscape::TextGroup)) {
		$PrisonEscape::TextGroup = new ScriptGroup(TextGroup) { };
	}

	%line0 = 	"<color:ffffff>        ROUND " @ $Statistics::round;
	if (getStatistic("Winner") !$= "Guards") {
		%line1 =	"<color:ff8724>     PRISONERS WIN";
	} else {
		%line1 =	"<color:8AD88D>        GUARDS WIN";
	}
	%line2 =	" ";
	%line3 = 	"<color:8AD88D>MVP Guard<color:ffffff>: " @ $bestGuardname;
	%line4 =	"<color:ff8724>MVP Prisoner<color:ffffff>: " @ $bestPrisonername;
	%line5 =	"<color:ffff00>Trays Used<color:ffffff>: " @ getStatistic("TraysPickedUp") + 0;
	%line6 =	"<color:ffff00>Buckets Used<color:ffffff>: " @ getStatistic("BucketsPickedUp") + 0;
	%line7 =	"<color:ffff00>Bricks Destroyed<color:ffffff>: " @ getStatistic("BricksDestroyed") + 0;
	%line8 =	"<color:ffff00>Prisoners Killed<color:ffffff>: " @ getStatistic("Deaths") + 0;
	%line9 =	" ";
	%line10 =	" <color:aaaaaa>---------------------";
	%line11 =	"<color:ffff00>Guard 1";
	%line12 =	"<color:ffffff>" @ getWord($Server::PrisonEscape::Guards, 0).name;
	%line13 =	"<color:ffff00>Guard 2";
	%line14 =	"<color:ffffff>" @ getWord($Server::PrisonEscape::Guards, 1).name;
	%line15 =	"<color:ffff00>Guard 3";
	%line16 =	"<color:ffffff>" @ getWord($Server::PrisonEscape::Guards, 2).name;
	%line17 =	"<color:ffff00>Guard 4";
	%line18 =	"<color:ffffff>" @ getWord($Server::PrisonEscape::Guards, 3).name;

	%str = %line0;
	for (%i = 1; %i < 19; %i++) {
		%str = %str TAB %line[%i];
	}

	displayTextOnBoard(_board1.getPosition(), 2, %str, 0);
	bottomprintAll(generateBottomPrint(), -1, 1);
}

function createTextSignLinePPE(%text, %pos, %rot, %size, %group, %justify, %lineNum) {
	if(%text $= "" || %pos $= "") 	{
		return;
	}

	%text = strlwr(%text);

	%vec[0] = "0.5 0 0";
	%vec[1] = "0 -0.5 0";
	%vec[2] = "-0.5 0 0";
	%vec[3] = "0 0.5 0";

	%len = strlen(%text);
	%physLen = %len;

	for(%i = 0; %i < %len; %i++) {
		%l = getSubStr(%text, %i, 1);

		%restAfter = getSubStr(%text, %i, strlen(%text) - %i);

		if(strpos( %restAfter, "<color:" ) == 0) {
			%physLen -= 14;
		}
	}

	switch(%justify) {
		case 1:
			%pos = VectorSub(%pos, VectorScale( %vec[ %rot ], textSignScaleToUnits(%size) * (%physLen-1) ));
		case 2:
			%pos = VectorSub(%pos, VectorScale( %vec[ %rot ], textSignScaleToUnits(%size) * %physLen * 2 ));
	}

	%axis[0] = "1 0 0 0";
	%axis[1] = "0 0 -1 270";
	%axis[2] = "0 0 -1 180";
	%axis[3] = "0 0 -1 90";

	%axis = %axis[ %rot ];

	%s = textSignScaleToUnits(%size);

	for(%i = 0; %i < %len; %i++) {
		%l = getSubStr(%text, %i, 1);

		%restAfter = getSubStr(%text, %i, strlen(%text) - %i);

		if(strpos( %restAfter, "<color:" ) == 0) {
			%hex = getSubStr( %restAfter, 7, 6 );

			$_currentTextSignColor = strupr(%hex);

			%i += 13;
			continue;
		}

		%id = strpos($TextSign::PosArray, %l);

		if(%id >= 0) {		
			%shape = new StaticShape(Char @ %lineNum) {
				datablock = "TextSignShape";
				position = %pos;
				rotation = %axis;
				scale = %s SPC %s SPC %s;
			};

			%shape.setNodeColor("ALL", getColorF($_currentTextSignColor));
			
			%shape.hideNode("ALL");
			%shape.unhideNode("c" @ %id);
			
			%group.add(%shape);
		}

		%pos = VectorAdd(%pos, VectorScale( %vec[ %rot ], textSignScaleToUnits(%size)*2 ));
	}
}

function displayTextOnBoard(%pos, %rot, %str, %clear) {
	if (!isObject($PrisonEscape::TextGroup)) {
		$PrisonEscape::TextGroup = new ScriptGroup(TextGroup) { };
	}

	%size = 1;
	%offset = -0.31;

	if (%clear) {
		deleteVariables("$displayStr*");
	}

	for (%i = 0; %i < getFieldCount(%str); %i++) {
		%substr = getField(%str, %i);
		if (%substr $= $displayStr[%i]) {
			%pos = vectorAdd(%pos, 0 SPC 0 SPC %offset);
			continue;
		}
		while (isObject(Char @ %i)) {
			(Char @ %i).delete();
		}
		$displayStr[%i] = %substr;
		createTextSignLinePPE(%substr, %pos, %rot, %size, TextGroup, 0, %i);
		%pos = vectorAdd(%pos, 0 SPC 0 SPC %offset);
	}
}

displayTextOnBoard(_board1.getPosition(), 2,
	"<color:ffffff>Testing" TAB
	"<color:ff0000>Up to 23 letters a line" TAB
	" " TAB
	"<color:ffff00>Empty line above" TAB
	"<color:ffff00>16 lines max" TAB
	"<color:00ff00>Line 6" TAB
	"<color:00ff00>Line 7" TAB
	"<color:00ff00>Line 8" TAB
	"<color:00ff00>Line 9" TAB
	"<color:00ff00>Line 10" TAB
	"<color:00ff00>Line 11" TAB
	"<color:00ff00>Line 12" TAB
	"<color:00ff00>Line 13 EDITED" TAB
	"<color:00ff00>Line 14" TAB
	"<color:00ff00>Line 15" TAB
	"<color:00ff00>Line 16 EDITED" TAB
	"<color:00ff00>17EDITED" TAB
	"<color:00ff00>Line 18" TAB
	"<color:00ff00>Line 19", 1
	);

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

	displayRoundLoadingInfo();
}


function pickLobbySpawnPoint() {
	%start = getRandom(0, $Server::PrisonEscape::LobbySpawnPoints.getCount() - 1);
	%count = $Server::PrisonEscape::LobbySpawnPoints.getCount();
	for (%i = 0; %i < %count; %i++)	{
		%index = (%i + %start) % %count;
		%brick = $Server::PrisonEscape::LobbySpawnPoints.getObject(%index);
		if (%brick.spawnCount < 1) {
			break;
		}
		%brick = "";
	}
	if (isObject(%brick)) {
		%brick.spawnCount++;
		return %brick.getPosition() SPC getWords(%brick.getSpawnPoint(), 3, 6);
	} else {
		echo("Can't find an Lobby spawnpoint with less than 1 spawn! Resetting...");
		resetLobbySpawnPointCounts();
		return $Server::PrisonEscape::LobbySpawnPoints.getObject(%start).getSpawnPoint();
	}
}

function resetLobbySpawnPointCounts() {
	for (%i = 0; %i < $Server::PrisonEscape::LobbySpawnPoints.getCount(); %i++) {
		$Server::PrisonEscape::LobbySpawnPoints.getObject(%i).spawnCount = 0;
	}
}

function spawnDeadLobby() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);

		if (!isObject(%client.player)) {
			%spawn = pickLobbySpawnPoint();
			%client.createPlayer(%spawn);
			if (!%client.pushedCenterprint) {
				%client.centerprint("");
			}
			%client.spawnTime = getSimTime();
		}
		// else if (isObject(%client.player) && %client.getControlObject() != %client.player) {
		// 	%client.setControlObject(%client.player);
		// }
		commandToClient(%client, 'showBricks', 0);
	}
	resetLobbySpawnPointCounts();
}