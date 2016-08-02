//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load dts shapes and merge animations
datablock TSShapeConstructor(BuffDts)
{
	baseShape  = "./buff_bot.dts";
	sequence0  = "./buff_root.dsq root";

	sequence1  = "./buff_run.dsq run";
	sequence2  = "./buff_run.dsq walk";
	sequence3  = "./buff_back.dsq back";
	sequence4  = "./buff_side.dsq side";

	sequence5  = "./buff_root.dsq crouch";
	sequence6  = "./buff_run.dsq crouchRun";
	sequence7  = "./buff_back.dsq crouchBack";
	sequence8  = "./buff_side.dsq crouchSide";

	sequence9  = "./buff_look.dsq look";
	sequence10 = "./buff_headlook.dsq headside";
	sequence11 = "./buff_root.dsq headUp";

	sequence12 = "./buff_jump.dsq jump";
	sequence13 = "./buff_jump.dsq standjump";
	sequence14 = "./buff_root.dsq fall";
	sequence15 = "./buff_root.dsq land";

	sequence16 = "./buff_armAttack.dsq armAttack";
	sequence17 = "./buff_armReadyLeft.dsq armReadyLeft";
	sequence18 = "./buff_armReady.dsq armReadyRight";
	sequence19 = "./buff_armReadyBoth.dsq armReadyBoth";
	sequence20 = "./buff_spearready.dsq spearready";     
	sequence21 = "./buff_spearThrow.dsq spearThrow";

	sequence22 = "./buff_side.dsq talk";  

	sequence23 = "./buff_death.dsq death1"; 
	
	sequence24 = "./buff_shiftUp.dsq shiftUp";
	sequence25 = "./buff_shiftDown.dsq shiftDown";
	sequence26 = "./buff_shiftUp.dsq shiftAway";
	sequence27 = "./buff_shiftDown.dsq shiftTo";
	sequence28 = "./buff_rotateCCW.dsq shiftLeft";
	sequence29 = "./buff_rotateCW.dsq shiftRight";
	sequence30 = "./buff_rotateCW.dsq rotCW";
	sequence31 = "./buff_rotateCCW.dsq rotCCW";

	sequence32 = "./buff_undo.dsq undo";
	sequence33 = "./buff_plant.dsq plant";

	sequence34 = "./buff_sit.dsq sit";

	sequence35 = "./buff_wrench.dsq wrench";

   sequence36 = "./buff_activate.dsq activate";
   sequence37 = "./buff_activate2.dsq activate2";

   sequence38 = "./buff_recoil.dsq leftrecoil";
};    

datablock PlayerData(BuffArmor)
{
   renderFirstPerson = false;
   emap = false;
   
   className = Armor;
   shapeFile = "./buff_bot.dts";
   cameraMaxDist = 8;
   cameraTilt = 0.261;//0.174 * 2.5; //~25 degrees
   cameraVerticalOffset = 1.3;
     
   cameraDefaultFov = 90.0;
   cameraMinFov = 5.0;
   cameraMaxFov = 120.0;
   
   //debrisShapeName = "~/data/shapes/player/debris_player.dts";
   //debris = BuffDebris;

   aiAvoidThis = true;

   minLookAngle = -1.5708;
   maxLookAngle = 1.5708;
   maxFreelookAngle = 3.0;

   mass = 140;
   drag = 0.1;
   density = 0.7;
   maxDamage = 200;
   maxEnergy =  10;
   repairRate = 0.33;

   rechargeRate = 0.4;

   runForce = 60 * 140;
   runEnergyDrain = 0;
   minRunEnergy = 0;
   maxStepHeight= "1";
   maxForwardSpeed = 7;
   maxBackwardSpeed = 3.5;
   maxSideSpeed = 3;

   maxForwardCrouchSpeed = 7.5;
   maxBackwardCrouchSpeed = 3.5;
   maxSideCrouchSpeed = 3;

   maxForwardProneSpeed = 0;
   maxBackwardProneSpeed = 0;
   maxSideProneSpeed = 0;

   maxForwardWalkSpeed = 5;
   maxBackwardWalkSpeed = 2;
   maxSideWalkSpeed = 3;

   maxUnderwaterForwardSpeed = 5;
   maxUnderwaterBackwardSpeed = 5;
   maxUnderwaterSideSpeed = 3;

   jumpForce = 12 * 140; //8.3 * 90;
   jumpEnergyDrain = 0;
   minJumpEnergy = 0;
   jumpDelay = 0;

   minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

   minImpactSpeed = 250;
   speedDamageScale = 3.8;

   boundingBox			= vectorScale("1.2 1.2 2.6", 4);
   crouchBoundingBox	= vectorScale("1.2 1.2 2.6", 4);
   
   pickupRadius = 0.75;
   
   // Foot Prints
   //decalData   = BuffFootprint;
   //decalOffset = 0.25;
	
   jetEmitter = "";
   jetGroundEmitter = "";
   jetGroundDistance = 4;
  
   //footPuffEmitter = LightPuffEmitter;
   footPuffNumParts = 10;
   footPuffRadius = 0.25;

   //dustEmitter = LiftoffDustEmitter;

   splash = PlayerSplash;
   splashVelocity = 4.0;
   splashAngle = 67.0;
   splashFreqMod = 300.0;
   splashVelEpsilon = 0.60;
   bubbleEmitTime = 0.1;
   splashEmitter[0] = PlayerFoamDropletsEmitter;
   splashEmitter[1] = PlayerFoamEmitter;
   splashEmitter[2] = PlayerBubbleEmitter;
   mediumSplashSoundVelocity = 10.0;   
   hardSplashSoundVelocity = 20.0;   
   exitSplashSoundVelocity = 5.0;

   // Controls over slope of runnable/jumpable surfaces
   runSurfaceAngle  = 85;
   jumpSurfaceAngle = 86;

   minJumpSpeed = 20;
   maxJumpSpeed = 30;

   horizMaxSpeed = 0;
   horizResistSpeed = 33;
   horizResistFactor = 0.35;

   upMaxSpeed = 80;
   upResistSpeed = 25;
   upResistFactor = 0.3;
   
   footstepSplashHeight = 0.35;

   //NOTE:  some sounds commented out until wav's are available

   JumpSound			= "JumpSound";

   // Footstep Sounds
//   FootSoftSound        = BuffFootFallSound;
//   FootHardSound        = BuffFootFallSound;
//   FootMetalSound       = BuffFootFallSound;
//   FootSnowSound        = BuffFootFallSound;
//   FootShallowSound     = BuffFootFallSound;
//   FootWadingSound      = BuffFootFallSound;
//   FootUnderwaterSound  = BuffFootFallSound;
   //FootBubblesSound     = FootLightBubblesSound;
   //movingBubblesSound   = ArmorMoveBubblesSound;
   //waterBreathSound     = WaterBreathMaleSound;

   //impactSoftSound      = ImpactLightSoftSound;
   //impactHardSound      = ImpactLightHardSound;
   //impactMetalSound     = ImpactLightMetalSound;
   //impactSnowSound      = ImpactLightSnowSound;
   
   impactWaterEasy      = Splash1Sound;
   impactWaterMedium    = Splash1Sound;
   impactWaterHard      = Splash1Sound;
   
   groundImpactMinSpeed    = 10.0;
   groundImpactShakeFreq   = "4.0 4.0 4.0";
   groundImpactShakeAmp    = "1.0 1.0 1.0";
   groundImpactShakeDuration = 0.8;
   groundImpactShakeFalloff = 10.0;
   
   //exitingWater         = ExitingWaterLightSound;

   // Inventory Items
	maxItems   = 10;	//total number of bricks you can carry
	maxWeapons = 5;		//this will be controlled by mini-game code
	maxTools = 5;
	
	uiName = "Buff";
	rideable = false;
		lookUpLimit = 0.585398;
		lookDownLimit = 0.385398;

	canRide = true;
	showEnergyBar = false;
	paintable = true;

	brickImage = brickImage;	//the imageData to use for brick deployment

   numMountPoints = 0;
   mountThread[0] = "armReadyBoth";
   mountNode[0] = 0;

   mountThread[1] = "root";
   mountNode[1] = 1;

   mountThread[1] = "root";
   mountNode[1] = 2;
};



function BuffArmor::onAdd(%this,%obj)
{
   // Vehicle timeout
   %obj.mountVehicle = true;

   // Default dynamic armor stats
   %obj.setRepairRate(0);

}



//called when the driver of a player-vehicle is unmounted
function BuffArmor::onDriverLeave(%obj, %player)
{
	//do nothing
}