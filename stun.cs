registerOutputEvent("Player", "electrocute", "int 0 60", 1);

function Player::electrocute(%player, %time) 
{
	if (!isObject(%client = %player.client))
		return;

	%client.camera.setMode(Corpse, %player);
	%client.setControlObject(%client.camera);
	%client.elecrocutedTime += %time;
	
	electrocute(%player, %time);
}

if (isPackage(EventElectrocute))
	deactivatePackage(EventElectrocute);

package EventElectrocute {
	
	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%client = %obj.getControllingClient();
		
		if (%client.isBeingStunned)
			return;
		
		Parent::onTrigger(%this, %obj, %trig, %state);
	}
	
};
activatePackage(EventElectrocute);

function stun(%player, %time) {
	if (!isObject(%client = %player.client))
		return;

	if (isEventPending(%player.stunLoop))
		cancel(%player.stunLoop);
	if (%time <= 0)
	{
		if (%player.isDisabled()) {
			%client.setControlObject(%client.camera);
			return;
		}
		%player.dismount();
		%client.isBeingStunned = 0;
		%player.unmountImage(3);
		%player.setControlObject(%player);
		%client.setControlObject(%player);
		%player.playThread(3, root);
		return;
	} else if (!%client.isBeingStunned && isObject(%client.player)) {
		%player.setControlObject(%client.camera);
		%player.setVelocity(vectorAdd(%player.getVelocity(), getRandom() * 2 SPC getRandom() * 2 SPC "3"));
		%player.mountImage(stunImage, 3);
		
		%client.camera.setMode(Corpse, %player);
		%client.setControlObject(%client.camera);

		%player.playThread(3, sit);

		%player.setVelocity(vectorAdd("0 0 5", %player.getVelocity()));
	}

	setStatistic("StunTime", getStatistic("StunTime", %player.client) + 1, %player.client);
	setStatistic("StunTime", getStatistic("StunTime") + 1);
	%client.isBeingStunned = 1;
	
	//emitter here

	%player.stunLoop = schedule(1000, %player, stun, %player, %time - 1);
}

function electrocute(%player, %time)
{
	if (!isObject(%client = %player.client))
		return;

	if (isEventPending(%player.electrocuteLoop))
		cancel(%player.electrocuteLoop);
	if (%time <= 0)
	{
		%client.applyBodyColors();
		%client.camera.setMode(Observer);
		%client.setControlObject(%player);
		%client.isBeingStunned = 0;
		return;
	}

	setStatistic("ElectrocutedTime", getStatistic("ElectrocutedTime", %player.client) + 1, %player.client);
	setStatistic("ElectrocutedTime", getStatistic("ElectrocutedTime") + 1);
	
	%client.isBeingStunned = 1;

	%player.setNodeColor("ALL", "1 1 1 1");
	%player.schedule(100, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(200, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(300, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(400, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(500, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(600, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(700, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(800, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(900, setNodeColor, "ALL", "0 0 0 1");

	%player.playThread(2, plant);
	%player.schedule(100, playThread, 2, plant);
	%player.schedule(200, playThread, 2, plant);
	%player.schedule(300, playThread, 2, plant);
	%player.schedule(400, playThread, 2, plant);
	%player.schedule(500, playThread, 2, plant);
	%player.schedule(600, playThread, 2, plant);
	%player.schedule(700, playThread, 2, plant);
	%player.schedule(800, playThread, 2, plant);
	%player.schedule(900, playThread, 2, plant);

	%player.electrocuteLoop = schedule(1000, 0, electrocute, %player, %time - 1);

	spawnRadioWaves(%player);
	schedule(100, 0, spawnRadioWaves, %player);
	schedule(200, 0, spawnRadioWaves, %player);
	schedule(300, 0, spawnRadioWaves, %player);
	schedule(400, 0, spawnRadioWaves, %player);
	schedule(500, 0, spawnRadioWaves, %player);
	schedule(600, 0, spawnRadioWaves, %player);
	schedule(700, 0, spawnRadioWaves, %player);
	schedule(800, 0, spawnRadioWaves, %player);
	schedule(900, 0, spawnRadioWaves, %player);
}

function spawnRadioWaves(%player)
{
	%pos = %player.getMuzzlePoint(2);
	%scale = getWord(%player.getScale(), 2)*0.5+getRandom()*1.5;

	%proj = new Projectile(){
		datablock = radioWaveProjectile;
		initialPosition = %pos;
		initialVelocity = "0 0 0";
		scale = %scale SPC %scale SPC %scale;
	};
	%proj.explode();
}