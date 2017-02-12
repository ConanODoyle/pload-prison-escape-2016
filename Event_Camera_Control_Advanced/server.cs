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

   if (%this.defaultState $= "") {
      if (%this.getDatablock().isOpen) {
         if (%this.getDatablock().getName() $= %this.getDatablock().openCCW) {
            %this.defaultState = 3;
         } else {
            %this.defaultState = 2;
         }
      } else {
         %this.defaultState = 4;
      }
   }

   if (%this.getDatablock().isOpen) {
      %this.door(4);
   } else { 
      if (%this.isCCW && strPos(%this.getName(), "left") < 0) {
         %this.door(3);
         %this.isCCW = 0;
      } else if (strPos(%this.getName(), "right") < 0) {
         %this.door(2);
         %this.isCCW = 1;
      }
   }
   %this.loopToggleLoop = %this.schedule(%time / 4, loopToggle, %time);
}

function fxDTSBrick::endLoopToggle(%this) {
   if (isEventPending(%this.loopToggleLoop)) {
      if (%this.numViewers >= 1) {
         %this.numViewers--;
      }
      if (%this.numViewers <= 0) {
         cancel(%this.loopToggleLoop);
         %this.door(%this.defaultState);
      }
   }
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
   //messageclient(%client,'',"\c2Camera in Static Mode \c6- Use Light key to exit camera");
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
   function Observer::onTrigger(%this, %obj, %trig, %state) {
      %client = %obj.getControllingClient();
      if (%client.player.isPreviewingCameras && %state == 1) {
         %count = $Server::PrisonEscape::Cameras.getCount();
         if (%trig == 0) {
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).endLoopToggle();
            %cl.currCamera = (%cl.currCamera + 1) % %count;
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).setPlayerCamera(0, %cl);
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).numViewers++;
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).loopToggle(5000);
            return;
         } else if (%trig == 4) {
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).endLoopToggle();
            %cl.currCamera = (%cl.currCamera - 1 + %count) % %count;
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).setPlayerCamera(0, %cl);
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).numViewers++;
            $Server::PrisonEscape::Cameras.getObject(%cl.currCamera).loopToggle(5000);
            return;
         }
      }
      return parent::onTrigger(%this, %obj, %trig, %state);
   }

   function serverCmdLight(%client)
   {
      if (%client.player.isInCamera && %client.player.canLeaveCamera)
      {
         if (isObject(%client.player))
            %client.setControlObject(%client.player);
         %client.player.isInCamera = 0;
         %client.player.isPreviewingCameras = 0;
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

function fxDTSBrick::previewCameras(%this, %client) {
   %client.player.isInCamera = 1;
   %client.player.isPreviewingCameras = 1;
   %client.player.canLeaveCamera = 1; 

   if ($Server::PrisonEscape::Cameras.getCount() <= 0) {
      PPE_messageAdmins("!!! \c6Cannot use camera previews - no cameras in SimSet!");
      return;
   }

   messageclient(%client,'',"\c2Camera in Free Mode \c6- Use Light key to exit the cameras");
   %client.centerprint("<font:Consolas:18><just:left>\c6Left Click<just:right>\c6Right Click<font:Arial Bold:22> <br><just:left>\c3Next Camera<just:right>\c3Prev Camera ");
}