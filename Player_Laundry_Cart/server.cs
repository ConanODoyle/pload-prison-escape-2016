//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load dts shapes and merge animations
datablock TSShapeConstructor(LaundryCartDts)
{
	baseShape  = "./bl_laundry_basket.dts";
	sequence0  = "./lc_root.dsq root";

	sequence1  = "./lc_run.dsq run";
	sequence2  = "./lc_run.dsq walk";
	sequence3  = "./lc_back.dsq back";
	sequence4  = "./lc_root.dsq side";

	sequence5  = "./lc_root.dsq crouch";
	sequence6  = "./lc_root.dsq crouchRun";
	sequence7  = "./lc_root.dsq crouchBack";
	sequence8  = "./lc_root.dsq crouchSide";

	sequence9  = "./lc_root.dsq look";
	sequence10 = "./lc_root.dsq headside";
	sequence11 = "./lc_root.dsq headUp";

	sequence12 = "./lc_root.dsq jump";
	sequence13 = "./lc_root.dsq standjump";
	sequence14 = "./lc_root.dsq fall";
	sequence15 = "./lc_root.dsq land";

	sequence16 = "./lc_root.dsq armAttack";
	sequence17 = "./lc_root.dsq armReadyLeft";
	sequence18 = "./lc_root.dsq armReadyRight";
	sequence19 = "./lc_root.dsq armReadyBoth";
	sequence20 = "./lc_root.dsq spearready";  
	sequence21 = "./lc_root.dsq spearThrow";

	sequence22 = "./lc_root.dsq talk";  

	sequence23 = "./lc_root.dsq death1"; 
	
	sequence24 = "./lc_root.dsq shiftUp";
	sequence25 = "./lc_root.dsq shiftDown";
	sequence26 = "./lc_root.dsq shiftAway";
	sequence27 = "./lc_root.dsq shiftTo";
	sequence28 = "./lc_root.dsq shiftLeft";
	sequence29 = "./lc_root.dsq shiftRight";
	sequence30 = "./lc_root.dsq rotCW";
	sequence31 = "./lc_root.dsq rotCCW";

	sequence32 = "./lc_root.dsq undo";
	sequence33 = "./lc_root.dsq plant";

	sequence34 = "./lc_root.dsq sit";

	sequence35 = "./lc_root.dsq wrench";

   sequence36 = "./lc_root.dsq activate";
   sequence37 = "./lc_root.dsq activate2";

   sequence38 = "./lc_root.dsq leftrecoil";
};    

datablock PlayerData(LaundryCartArmor)
{
   renderFirstPerson = false;
   emap = false;
   
   className = Armor;
   shapeFile = "./bl_laundry_basket.dts";
   cameraMaxDist = 8;
   cameraTilt = 0.261;//0.174 * 2.5; //~25 degrees
   cameraVerticalOffset = 1.3;

   cameraDefaultFov = 90.0;
   cameraMinFov = 5.0;
   cameraMaxFov = 120.0;
   
   //debrisShapeName = "~/data/shapes/player/debris_player.dts";
   //debris = LaundryCartDebris;

   aiAvoidThis = true;

   minLookAngle = -1.5708;
   maxLookAngle = 1.5708;
   maxFreelookAngle = 3.0;

   mass = 30;
   drag = 0.1;
   density = 0.7;
   maxDamage = 200;
   maxEnergy =  10;
   repairRate = 0.33;

   rechargeRate = 0.4;

   runForce = 60 * 90;
   runEnergyDrain = 0;
   minRunEnergy = 0;
   maxStepHeight= "1";
   maxForwardSpeed = 6.5;
   maxBackwardSpeed = 4.5;
   maxSideSpeed = 0.5;

   maxForwardCrouchSpeed = 6.5;
   maxBackwardCrouchSpeed = 4.5;
   maxSideCrouchSpeed = 0.5;

   maxForwardProneSpeed = 0;
   maxBackwardProneSpeed = 0;
   maxSideProneSpeed = 0;

   maxForwardWalkSpeed = 6;
   maxBackwardWalkSpeed = 3;
   maxSideWalkSpeed = 0;

   maxUnderwaterForwardSpeed = 0;
   maxUnderwaterBackwardSpeed = 0;
   maxUnderwaterSideSpeed = 0;

   jumpForce = 0 * 90; //8.3 * 90;
   jumpEnergyDrain = 0;
   minJumpEnergy = 0;
   jumpDelay = 0;

   minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

   minImpactSpeed = 250;
   speedDamageScale = 3.8;

   boundingBox			= vectorScale("1.9 1.9 0.4", 4);
   crouchBoundingBox	= vectorScale("1.9 1.9 0.4", 4);
   
   pickupRadius = 0.75;
   
   //decalData   = LaundryCartFootprint;
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

   horizMaxSpeed = 200;
   horizResistSpeed = 33;
   horizResistFactor = 2;

   upMaxSpeed = 80;
   upResistSpeed = 25;
   upResistFactor = 0.01;
   
   footstepSplashHeight = 0.35;

   //NOTE:  some sounds commented out until wav's are available

   JumpSound			= "";

   // Footstep Sounds
//   FootSoftSound        = LaundryCartFootFallSound;
//   FootHardSound        = LaundryCartFootFallSound;
//   FootMetalSound       = LaundryCartFootFallSound;
//   FootSnowSound        = LaundryCartFootFallSound;
//   FootShallowSound     = LaundryCartFootFallSound;
//   FootWadingSound      = LaundryCartFootFallSound;
//   FootUnderwaterSound  = LaundryCartFootFallSound;
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
	
	uiName = "LaundryCart";
	rideable = true;
	lookUpLimit = 0.585398;
	lookDownLimit = 0.585398;

	canRide = false;
	showEnergyBar = false;
	paintable = true;

	brickImage = LaundryCartBrickImage;	//the imageData to use for brick deployment

   numMountPoints = 3;
   mountThread[0] = "armReadyBoth";
   mountNode[0] = 0;

   mountThread[1] = "root";
   mountNode[1] = 1;

   mountThread[2] = "root";
   mountNode[2] = 2;
};



function LaundryCartArmor::onAdd(%this,%obj)
{
   // Vehicle timeout
   %obj.mountVehicle = true;

   // Default dynamic armor stats
   %obj.setRepairRate(0);
   
   %obj.hideNode("lhand");
   %obj.hideNode("rhand");
   %obj.hideNode("lhook");
   %obj.hideNode("rhook");
   %obj.hideNode("lshoe");
   %obj.hideNode("rshoe");
}

$pushForce = 30;

package LaundryCartPackage {
   function Armor::onMount(%this, %obj, %vehi, %mountPoint) {
      %db = %vehi.getDatablock().getName();
      if (%db $= "LaundryCartArmor" && %mountPoint == 0) {
         %vehi.unHideNode("lshoe");
         %vehi.unHideNode("rshoe");

         %vehi.setNodeColor("lshoe", %obj.client.llegcolor);
         %vehi.setNodeColor("rshoe", %obj.client.rlegcolor);

         if (%obj.isNodeVisible("lhand")) {
            %vehi.unHideNode("lhand");
            %vehi.setNodeColor("lhand", %obj.client.lhandcolor);
         }
         if (%obj.isNodeVisible("rhand")) {
            %vehi.unHideNode("rhand");
            %vehi.setNodeColor("rhand", %obj.client.rhandcolor);
         }

         if (%obj.isNodeVisible("lhook")) {
            %vehi.unHideNode("lhook");
            %vehi.setNodeColor("lhook", %obj.client.lhandcolor);
         }
         if (%obj.isNodeVisible("rhook")) {
            %vehi.unHideNode("rhook");
            %vehi.setNodeColor("rhook", %obj.client.rhandcolor);
         }

         %obj.hideNode("lhand");
         %obj.hideNode("rhand");
         %obj.hideNode("lhook");
         %obj.hideNode("rhook");
         %obj.hideNode("lshoe");
         %obj.hideNode("rshoe");
      }
      return parent::onMount(%this, %obj, %vehi, %mountPoint);
   }

   function Armor::onUnMount(%this, %obj, %vehi, %mountPoint) {
      if (isObject(%vehi)) {
         %db = %vehi.getDatablock().getName();
         if (%db $= "LaundryCartArmor" && %mountPoint == 0) {
            %vehi.hideNode("lhand");
            %vehi.hideNode("rhand");
            %vehi.hideNode("lhook");
            %vehi.hideNode("rhook");
            %vehi.hideNode("lshoe");
            %vehi.hideNode("rshoe");

            %obj.client.applyBodyParts();
            %obj.client.applyBodyColors();
         }
      }
      return parent::onUnMount(%this, %obj, %vehi, %mountPoint);
   }

   function Player::activateStuff(%this) {
      if (!isObject(%vehi = %this.getObjectMount()) && !%this.client.isGuard) {
         %s = %this.getEyePoint();
         %e = vectorAdd(vectorScale(%this.getEyeVector(), 3), %s);
         %masks = $TypeMasks::fxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticShapeObjectType;
         %ray = containerRaycast(%s, %e, %masks, %this);
         %hit = getWord(%ray, 0);

         if (isObject(%hit) && %hit.getDatablock().getName() $= "LaundryCartArmor") {
            %originalPos = %this.getTransform();

            %this.setTransform(vectorAdd(%hit.getPosition(), "0 0 5"));

            Armor::onCollision(LaundryCartArmor.getID(), %hit, %this, "0.000000 0.000000 -1.704514", 1.704514);
            if (!isObject(%this.getObjectMount())) {
               %this.setTransform(%originalPos);
            }
         }
      } else if (isObject(%vehi) && %this.getMountNode() == 0) {
         %vec = vectorNormalize(getWords(%this.getMuzzleVector(0), 0, 1) SPC 0.1);
         %this.getDatablock().doDismount(%this);
         %vehi.setVelocity(vectorAdd(%vehi.getVelocity(), vectorScale(%vec, $pushForce)));
      }
      return parent::activateStuff(%this);
   }

   function serverCmdNextSeat(%cl) {
      if (isObject(%pl = %cl.player) && %pl.getObjectMount().getDatablock().getName() $= "LaundryCartArmor") {
         if (%pl.getMountNode() == 0) {
            %cl.centerprint("\c3You can't swap seats as the driver of the cart!", 1);
         } else {
            parent::serverCmdNextSeat(%cl);
            if (%pl.getMountNode() == 0) {
               parent::serverCmdNextSeat(%cl);
            }
         }
      } 
   }

   function serverCmdPrevSeat(%cl) {
      if (isObject(%pl = %cl.player) && %pl.getObjectMount().getDatablock().getName() $= "LaundryCartArmor") {
         if (%pl.getMountNode() == 0) {
            %cl.centerprint("\c3You can't swap seats as the driver of the cart!", 1);
         } else {
            parent::serverCmdPrevSeat(%cl);
            if (%pl.getMountNode() == 0) {
               parent::serverCmdPrevSeat(%cl);
            }
         }
      } 
   }
};
activatePackage(LaundryCartPackage);

//called when the driver of a player-vehicle is unmounted
function LaundryCartArmor::onDriverLeave(%obj, %player)
{
	//do nothing
}