//usage: registerInputEvent(%class, %eventName, %targetList, %adminOnly)
registerOutputEvent("GameConnection", "setCameraView", "string 200 156" TAB "string 200 156", 0);
registerOutputEvent("GameConnection", "setCameraDolly", "string 200 156" TAB "string 200 156", 0);
registerOutputEvent("fxDTSBrick", "setPlayerCamera", "list Free 0 North 1 East 2 South 3 West 4 Up 5 Down 6", 1);
registerOutputEvent("GameConnection", "setCameraNormal", "", 0);


//puts the player's camera at the given brick while retaining control of the player
function fxDTSBrick::loopToggle(%this, %time) {
   if (isEventPending(%this.loopToggleLoop)) {
      cancel(%this.loopToggleLoop);
   }
   if (%time == 0) {
      return;
   }

   if (%this.getDatablock().isOpen) {
      %this.door(4);
   } else { 
      if (%this.isCCW) {
         %this.door(3);
         %this.isCCW = 0;
      } else {
         %this.door(2);
         %this.isCCW = 1;
      }
   }
   %this.loopToggleLoop = %this.schedule(%time / 4, loopToggle, %time);
}

function GameConnection::setCameraView(%client, %posBrickName, %targetBrickName)
{
   %camera = %client.camera;
   if(!isObject(%camera))
      return; //should never happen

   //translate name into brick object
   %posBrick = ("_" @ %posBrickName).getId();
   if(!isObject(%posBrick))
      return;
   %targetBrick = ("_" @ %targetBrickName).getId();
   if(!isObject(%targetBrick))
      return;

   //aim the camera at the target brick
   %pos = %posBrick.getPosition();
   %delta = vectorSub(%targetBrick.getPosition(), %pos);
   %deltaX = getWord(%delta, 0);
   %deltaY = getWord(%delta, 1);
   %deltaZ = getWord(%delta, 2);
   %deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

   %rotZ = mAtan(%deltaX, %deltaY) * -1; 
   %rotX = mAtan(%deltaZ, %deltaXYHyp);

   %aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...

   %camera.setTransform(%pos SPC %aa);
   %camera.setFlyMode();
   %camera.mode = "Observer";

   //client controls camera
   %client.setControlObject(%camera);

   //camera controls player
   %player = %client.player;
   if(isObject(%player))
   {
      %client.player.isInCamera = 1;
      %client.player.canLeaveCamera = 1;
      %camera.setControlObject(%client.dummyCamera);
   }
   else
   {
      //do something to make the camera immobile?
      //%camera.setDollyMode(%camera.getPosition(), %camera.getPosition());
      %camera.setControlObject(%client.dummyCamera);
      %client.player.canLeaveCamera = 1;
      
      //6802.camera.setDollyMode(6802.camera.getPosition(), 6802.camera.getPosition());
   }
   messageclient(%client,'',"\c2Camera in Static Mode \c6- Use Light key to exit camera");
}

function GameConnection::setCameraDolly(%client, %posBrickName, %targetBrickName)
{
   %camera = %client.camera;
   if(!isObject(%camera))
      return; //should never happen

   //translate name into brick object
   %posBrick = ("_" @ %posBrickName).getId();
   if(!isObject(%posBrick))
      return;
   %targetBrick = ("_" @ %targetBrickName).getId();
   if(!isObject(%targetBrick))
      return;

   //aim the camera at the target brick
   %pos1 = %posBrick.getPosition();
   %pos2 = %targetBrick.getPosition();

   %camera.setMode(Observer);
   %camera.setDollyMode(%pos1, %pos2);

   //client controls camera
   %client.setControlObject(%camera);
   %camera.setwhiteout(0.5);

   //camera controls player
   %player = %client.player;
   if(isObject(%player))
   {
      %client.player.isInCamera = 1;
      %client.player.canLeaveCamera = 1;
      %camera.setControlObject(%camera);
   }
   else
   {
      //do something to make the camera immobile?
      //%camera.setDollyMode(%camera.getPosition(), %camera.getPosition());
      %camera.setControlObject(%camera);
      %client.player.canLeaveCamera = 1;
      
      //6802.camera.setDollyMode(6802.camera.getPosition(), 6802.camera.getPosition());
   }
   messageclient(%client,'',"\c2Camera in Dolly Mode \c6- Use Light key to exit camera");
   messageclient(%client,'',"\c6Use A and D to move your camera.");
}

function fxDTSBrick::setPlayerCamera(%this,%option,%client)
{
   %camera = %client.camera;
   if(!isObject(%camera))
      return; //should never happen

   %pos = %this.getPosition();
   switch (%option)
   {
      case 1:  %rot = "1 0 0 0";
      case 2:  %rot = "0 0 1 1.5708";
      case 3:  %rot = "0 0 1 3.14159";
      case 4:  %rot = "0 0 -1 1.5708";
      case 5:  %rot = "-1 0 0 1.5708";
      case 6:  %rot = "1 0 0 1.5708";
      default: %rot = "0 0 0 0";
   }
   
   if (%option)
   {
      %camera.setTransform(%pos SPC %rot);
      %camera.setFlyMode();
      %camera.setMode(Observer);
      %camera.setControlObject(%client.dummyCamera);
   }
   else
   {
      %camera.setMode(Observer);      
      %camera.setDollyMode(%pos, %pos);
      %camera.setControlObject(%camera);
   }

   //client controls camera
   %client.setControlObject(%camera);
   %camera.setwhiteout(0.5);

   //camera controls player
   %player = %client.player;
   if(isObject(%player))
   {
      %client.player.isInCamera = 1;
      %client.player.canLeaveCamera = 1;
   }
   else
   {
      //do something to make the camera immobile?
      //%camera.setDollyMode(%camera.getPosition(), %camera.getPosition());
      %client.player.canLeaveCamera = 1;
      
      //6802.camera.setDollyMode(6802.camera.getPosition(), 6802.camera.getPosition());
   }
   messageclient(%client,'',"\c2Camera in Static Mode \c6- Use Light key to exit camera");
}

//returns control back to player with normal camera
function GameConnection::setCameraNormal(%client)
{
   %camera = %client.camera;
   if(!isObject(%camera))
      return; //should never happen
   
   %client.player.isInCamera = 0;
   %client.player.canLeaveCamera = 1;

   %player = %client.player;
   if(isObject(%player))
   {
      %client.setControlObject(%player);
      %camera.setControlObject(%camera);
   }
   else
   {
      //do something to make the mobile again?
      %camera.setControlObject(0);
      %camera.setFlyMode();
   }   
}

package CameraControlsExtended
{
   function serverCmdLight(%client)
   {
      if (%client.player.isInCamera && %client.player.canLeaveCamera)
      {
         if (isObject(%client.player))
            %client.setControlObject(%client.player);
         %client.player.isInCamera = 0;
         %client.player.canLeaveCamera = 1;
         return;
      }
      else
         return parent::serverCmdLight(%client);
   }
};
activatePackage(CameraControlsExtended);

registerOutputEvent("MiniGame", "setCameraView", "string 200 156" TAB "string 200 156", 1);
registerOutputEvent("Minigame", "setCameraDolly", "string 200 156" TAB "string 200 156", 0);
registerOutputEvent("MiniGame", "setCameraNormal", "", 1);

function MiniGameSO::setCameraView(%mg, %posBrickName, %targetBrickName)
{
   //set camera position/target for every player
   for(%i = 0; %i < %mg.numMembers; %i++)
	{
		%cl = %mg.member[%i];
      %cl.setCameraBrick(%posBrickName, %targetBrickName);
   }
}  

function MiniGameSO::setCameraDolly(%mg, %posBrickName, %targetBrickName)
{
   //set camera position/target for every player
   for(%i = 0; %i < %mg.numMembers; %i++)
   {
      %cl = %mg.member[%i];
      %cl.setCameraDolly(%posBrickName, %targetBrickName);
   }
}  

function MiniGameSO::setCameraNormal(%mg, %posBrickName, %targetBrickName)
{
   //set normal camera for every player
   for(%i = 0; %i < %mg.numMembers; %i++)
	{
		%cl = %mg.member[%i];
      %cl.setCameraNormal();
      %cl.canLeaveCamera = 0;
   }
} 
