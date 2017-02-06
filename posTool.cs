datablock ItemData(posToolItem : printGun) {
	image = posToolImage;

	uiname = "Pos Tool";
	colorShiftColor = "0 1 0 1";
};

datablock ShapeBaseImageData(posToolImage : printGunImage) {
	item = posToolItem;

	colorShiftColor = "0 1 0 1";

    projectile = "";

	stateName[0]						= "Activate";
	stateTimeout[0]						= 0.1;
	stateScript[0]						= "onActivate";
	stateTransitionOnTimeout[0]			= "Ready";

	stateName[1]						= "Ready";
	stateScript[1]						= "onReady";
	stateTransitionOnTriggerDown[1]		= "Fire";

	stateName[2]						= "Fire";
	stateTimeout[2]						= 0.2;
	stateTransitionOnTimeout[2]			= "Ready";
	stateScript[2]						= "onFire";
    stateEmitter[2]                     = "";
};

function posToolImage::onUnMount(%this, %obj, %slot) {
    if (isObject(%obj.lineShape)) {
        %obj.lineShape.delete();
    }

    if (isObject(%obj.currPosShape)) {
        %obj.currPosShape.delete();
        %obj.currPosShape2.delete();
    }

    %obj.currPos = "";
}

function posToolImage::onActivate(%this, %obj, %slot) {
    if (!isObject(%cl = %obj.client) || !%cl.isAdmin) {
        %obj.unMountImage(0);
        %cl.centerPrint("You are not allowed to use this item");
        return;
    }
    %obj.currPos = "";
}

function posToolImage::onReady(%this, %obj, %slot) {
    if (!isObject(%cl = %obj.client)) {
        return;
    }

    if (isObject(%obj.lineShape)) {
        %obj.lineShape.delete();
    }
}

function posToolImage::onFire(%this, %obj, %slot) {
    if (!isObject(%obj.lineShape)) {
        %obj.lineShape = new StaticShape() {
            datablock = C_SquareShape;
        };
    }

    if (!isObject(%obj.currPosShape)) {
        %obj.currPosShape = new StaticShape() {
            datablock = C_SquareShape;
        };
        %obj.currPosShape2 = new StaticShape() {
            datablock = C_SquareShape;
        };
    }
    %masks = $TypeMasks::fxBrickAlwaysObjectType | $TypeMasks::TerrainObjectType;
    %start = getWords(%obj.getEyeTransform(), 0, 2);
    %end = vectorAdd(vectorScale(%obj.getEyeVector(), 300), %start);
    %ray = containerRaycast(%start, %end, %masks);
    if (isObject(%hit = getWord(%ray, 0))) {
        %pos = roundVectorToBrickGrid(getWords(%ray, 1, 3));
        %obj.lineShape.drawLine(%obj.getMuzzlePoint(%slot), %pos, "1 1 0 1", 0.05);
        %obj.currPosShape.createBoxAt(%pos, "1 0 0 0.5", 0.1);
        %obj.currPosShape2.createBoxAt(%pos, "1 0 1 0.5", 0.15);
        %obj.currPos = %pos;
    } else {
        %obj.lineShape.drawLine(%obj.getMuzzlePoint(%slot), %end, "1 1 0 1", 0.05);
    }
}

function serverCmdSavePos(%cl, %a, %b, %c, %d, %e) {
    if (!%cl.isAdmin || !isObject(%pl = %cl.player)) {
        return;
    }

    %locName = trim(%a SPC %b SPC %c SPC %e);

    if ($location[$locationNum @ "::name"] !$= "" && $location[$locationNum @ "::name"] !$= %locName) {
        if ($location[$locationNum @ "::pos1"] !$= "") {
            messageClient(%cl, '', "!!! \c6Incrementing locationNum...");
            $locationNum++;
        } else {
            messageClient(%cl, '', "!!! \c6Location " @ $location[$locationNum @ "::name"] @ " does not match " @ %locname @ "; ignoring...");
            return;
        }
    }

    if (%pl.currPos !$= "") {
        if ($location[$locationNum @ "::pos0"] $= "" || $location[$locationNum @ "::pos1"] !$= "") {
            $location[$locationNum @ "::name"] = %locName;
            $location[$locationNum @ "::pos0"] = %pl.currPos;
            if ($location[$locationNum @ "::pos1"] !$= "") {
                messageClient(%cl, '', "!!! \c6Location " @ %locName @ " already has 2 positions; overriding...");
            }
            messageClient(%cl, '', "!!! \c6Location 1 of " @ %locName @ " is set at " @ %pl.currPos);
            $location[$locationNum @ "::pos1"] = "";
        } else {
            $location[$locationNum @ "::pos1"] = %pl.currPos;
            messageClient(%cl, '', "!!! \c6Location 2 of " @ %locName @ " is set at " @ %pl.currPos);
        }

        %pl.currPos = "";
        if (isObject(%pl.lineShape)) {
            %pl.lineShape.delete();
        }

        if (isObject(%pl.currPosShape)) {
            %pl.currPosShape.setNodeColor("ALL", "0 0 1 0.5");
            %pl.currPosShape2.setNodeColor("ALL", "0 1 1 0.5");
        }

        export("$location*", "Add-ons/Gamemode_PPE/locations.cs");
    }
}

function serverCmdSP(%cl, %a, %b, %c, %d, %e) {
    serverCmdSavePos(%cl, %a, %b, %c, %d, %e);
}

if (isFile("./locations.cs")) {
    exec("./locations.cs");
}
if ($locationNum $= "") {
    $locationNum = 0;
}

function roundVectorToBrickGrid(%pos) {
    %x = getWord(%pos, 0);
    %y = getWord(%pos, 1);
    %z = getWord(%pos, 2);

    %x = mFloor(%x * 2 + 0.5) / 2;
    %y = mFloor(%y * 2 + 0.5) / 2;
    %z = mFloor(%z * 5 + 0.5) / 5;
    return %x SPC %y SPC %z;
}






///line drawing///

datablock StaticShapeData(C_SquareShape)
{
    shapeFile = "./box0.2.dts";
    //base scale of shape is .2 .2 .2
};

$defaultColor = "1 0 0 0.5";

function StaticShape::drawLine(%this, %pos1, %pos2, %color, %scale, %offset) {
	%len = vectorLen(vectorSub(%pos2, %pos1));
	if (%scale <= 0) {
		%scale = 1;
	}
	if (%color $= "" || getWordCount(%color) < 4) {
		%color = $defaultColor;
	}

    %vector = vectorNormalize(vectorSub(%pos2, %pos1));

    %xyz = vectorNormalize(vectorCross("1 0 0", %vector)); //rotation axis
    %u = mACos(vectorDot("1 0 0", %vector)) * -1; //rotation value

    %this.setTransform(vectorScale(vectorAdd(%pos1, %pos2), 0.5) SPC %xyz SPC %u);
    %this.setScale((%len/2 + %offset) SPC %scale SPC %scale);
    %this.setNodeColor("ALL", %color);
    if (getWord(%color, 3) < 1) {
    	%this.startFade(0, 0, 1);
    } else {
    	%this.startFade(0, 0, 0);
    }

    return %this;
}

function StaticShape::createBoxAt(%this, %pos, %color, %scale) {
    if (%scale <= 0) {
        %scale = 1;
    }
    if (%color $= "" || getWordCount(%color) < 4) {
        %color = $defaultColor;
    }

    %this.setTransform(%pos SPC "1 0 0 0");
    %this.setScale(%scale SPC %scale SPC %scale);
    %this.setNodeColor("ALL", %color);
    if (getWord(%color, 3) < 1) {
        %this.startFade(0, 0, 1);
    } else {
        %this.startFade(0, 0, 0);
    }
}