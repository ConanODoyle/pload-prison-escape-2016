datablock ItemData(posToolItem : printGun) {
	image = posToolImage;

	uiname = "Pos Tool";
	colorShiftColor = "0 1 0 1";
};

datablock ImageData(posToolImage : printGunImage) {
	item = posToolItem;

	colorShiftColor = "0 1 0 1";

	stateName[0]						= "Activate";
	stateTimeout[0]						= 0.1;
	stateScript[0]						= "onActivate";
	stateTransitionOnTimeout[0]			= "Ready";

	stateName[1]						= "Ready";
	stateScript[1]						= "onReady";
	stateTransitionOnTriggerDown[1]		= "Fire";

	stateName[2]						= "Fire";
	stateTimeout[2]						= 0.2;
	stateTransitionOnTimeout[2]			= "onReady";
	stateScript[2]						= "onFire";
};

function posToolImage::onUnMount(%this, %obj, %slot) {
    if (isObject(%obj.lineShape)) {
        %obj.lineShape.delete();
    }

    if (isObject(%obj.posShape)) {
        %obj.lineShape.delete();
    }
}

function posToolImage::onActivate(%this, %obj, %slot) {
    if (!isObject(%cl = %obj.client) || !%cl.isAdmin) {
        %obj.unMountImage(0);
        %cl.centerPrint("You are not allowed to use this item");
        return;
    }
}

function posToolImage::onReady(%this, %obj, %slot) {
    if (!isObject(%cl = %obj.client)) {
        return;
    }
}

function posToolImage::onFire(%this, %obj, %slot) {
    if (!isObject(%obj.lineShape)) {
        %obj.lineShape = new StaticShape() {
            datablock = C_SquareShape;
        };
        %obj.deleteSchedule = schedule(1000)
    }
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