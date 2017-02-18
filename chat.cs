//handles chat manipulation

//identifies the location of a player
exec("./locations.cs");

package PrisonChatSystem
{
	function serverCmdMessageSent(%cl, %msg)
	{
		//-1 for debug/off, 0 for loading, 1 for round start, 2 for round in session, 3 for round end
		%phase = $Server::PrisonEscape::roundPhase;
		%isAlive = isObject(%cl.player);
		%team = %cl.isGuard;
		%isDonator = %cl.isDonator;
		if (%isAlive)
			%location = getRegion(%cl.player);
		else
			%location = "DEAD";
		%isOutside = (%location $= "Outside" || %location $= "Yard" || strPos(%location, "Tower") >= 0);
		if (!%cl.isGuard)
			%name = "<color:ff8724>" @ (%cl.fakeName $= "" ? %cl.name : %cl.fakeName);
		else
			%name = "<color:8AB28D>" @ (%cl.fakeName $= "" ? %cl.name : %cl.fakeName);

		//skip everything if off mode is enabled
		if (%phase == -1)
		{
			parent::serverCmdMessageSent(%cl, %msg);
			return;
		}

		if (%msg $= %cl.lastMessage && getRealTime() - %cl.lastMessageTime < 3000) {
			messageClient(%cl, '', "Do not repeat yourself");
			return;
		} else if (getRealTime() - %cl.lastMessageTime < 100) {
			messageClient(%cl, '', "Do not spam");
			%cl.lastMessageTime = getRealTime();
			return;
		}
		%cl.lastMessage = %msg;
		%cl.lastMessageTime = getRealTime();

		//set donator message
		if (%isDonator)
			%msg = "<shadow:-2:2><shadowcolor:ffffff>" @ %msg;

		if (%phase != 2) //loading in between rounds
		{
			//all chat can be seen by everyone
			//admins get special blue color
			if (%cl.isAdmin)
				%name = "\c1" @ %cl.name @ "\c6: ";

			//admins also get a sound accompanying their message
			if (!%cl.isAdmin)
				messageAll('', %name @ "\c6: " @ %msg);
			else
				messageAll('MsgUploadEnd', %name @ "\c6: " @ %msg);

			//bold messages with "guard" in it for admins so they can see who's requesting guard??
		}
		else if (%phase == 2) //pre-round, during-round, post round win camera
		{
			//statistics
			if (%cl.isGuard)
				$Server::PrisonEscape::GuardMessagesSent++;
			else
				$Server::PrisonEscape::PrisonerMessagesSent++;
			%cl.messagesSent++;

			//prefix location before name in message
			if (%location !$= "DEAD")
				%message = %name @ "\c6 [\c1" @ %location @ "\c6]: " @ %msg;
			else
				%message = %name @ "\c7 [DEAD]\c6: " @ %msg;


			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%target = ClientGroup.getObject(%i);
				%targetIsAlive = isObject(%target.player);
				%targetTeam = %target.isGuard;
				if (%targetIsAlive)
					%targetLocation = getRegion(%target.player);
				else
					%targetLocation = "DEAD";
				%targetIsOutside = (%targetLocation $= "Outside" || %targetLocation $= "Yard" || strPos(%targetLocation, "Tower") >= 0);

				//message people on same team
				if (%location !$= "DEAD" && %targetTeam == %team)
					messageClient(%target, '', %message);
				//and if outside, anyone else outside/in the yard
				else if (%targetIsOutside && %isOutside)
					messageClient(%target, '', %message);
				else if (%location $= "DEAD" || !%targetIsAlive) //all dead players can chat
					messageClient(%target, '', %message);
			}

		}
	}

	function serverCmdTeamMessageSent(%cl, %msg)
	{
		serverCmdMessageSent(%cl, "\c4" @ %msg);
	}
};
activatePackage(PrisonChatSystem);


function serverCmdBob(%cl, %target) {
	if (!%cl.isAdmin)  { 
		return;
	}
	(%targ = fcn(%target)).player.setShapeName("bob","8564862");
	%targ.fakeName = "bob";
	messageClient(%cl, '', "\c6you have made \c3" @ %targ.name @ "\c6 bob");
	messageClient(%targ, '', "you are bob");
}

function serverCmdUnBob(%cl, %target) {
	if (!%cl.isAdmin)  { 
		return;
	}
	(%targ = fcn(%target)).player.setShapeName(%targ.name,"8564862");
	%targ.fakeName = "";
	messageClient(%cl, '', "\c6you have made \c3" @ %targ.name @ "\c6 not bob");
	messageClient(%targ, '', "you are not bob anymore");
}

function serverCmdSetName(%cl, %target, %a, %b, %c, %d, %e, %f, %g) {
	if (!%cl.isAdmin)  { 
		return;
	}
	%targ = fcn(%target);
	%name = trim(%a SPC %b SPC %c SPC %d SPC %e SPC %f SPC %g);
	if (%name $= "") {
		return;
	}
	%targ.player.setShapeName(%name,"8564862");
	%targ.fakeName = "%name";	
	if (%name !$= %targ.name) {
		messageClient(%cl, '', "\c6You have set\c3" @ %targ.name @ "\c6's name to \"\c3" @ %name @ "\"");
		messageClient(%targ, '', "\c6Your name has been set to \"\c3" @ %name @ "\"");
	} else {
		messageClient(%cl, '', "\c6You have set\c3" @ %targ.name @ "\c6's name back to normal");
		messageClient(%targ, '', "\c6Your name has been reset to normal");
	}
}