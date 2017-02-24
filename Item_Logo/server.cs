datablock ItemData(LogoClosedItem : printGun) {
    shapeFile = "./logoClosed.dts";
	image = "";

    canPickup = false;

	uiname = "LogoClosedItem";
    doColorShift = false;
	colorShiftColor = "0 1 0 1";
};

datablock ItemData(LogoOpenItem : printGun) {
    shapeFile = "./logoOpen.dts";
    image = "";

    canPickup = false;

    uiname = "LogoOpenItem";
    doColorShift = false;
    colorShiftColor = "0 1 0 1";
};

datablock StaticShapeData(LogoClosedShape)
{
    shapeFile = "./LogoClosed.dts";
};

datablock StaticShapeData(LogoOpenShape)
{
    shapeFile = "./LogoOpen.dts";
};

datablock StaticShapeData(LogoDishShape)
{
    shapeFile = "./LogoDish.dts";
};

package PrisonEscape_Logo {
    function ItemData::onAdd(%this, %obj) {
        
        if (strPos(%this.getName(), "Logo") >= 0) {
            %obj.startFade(0, 0, 1);
            %obj.canPickup = false;

            %alpha = 1;
            %obj.hideNode("ALL");
            %obj.setNodeColor("outline", "0 0 0 " @ %alpha);
            %obj.setNodeColor("clothing", "1 0.479 0 " @ %alpha);
            %obj.setNodeColor("skin", "0.8 0.612 0.356 " @ %alpha);
            %obj.setNodeColor("bars", "0.359 0.359 0.359 " @ %alpha);
            %obj.setNodeColor("beams", "0.168 0.168 0.168 " @ %alpha);
            doLogoFadeIn(%obj, 0);
        }
        return parent::onAdd(%this, %obj);
    }
};
activatePackage(PrisonEscape_Logo);

function doLogoFadeOut(%item, %alpha) {
    if (%alpha <= 0 || !isObject(%item)) {
        %item.delete();
        return;
    }

    %item.setNodeColor("outline", "0 0 0 " @ %alpha);
    %item.setNodeColor("clothing", "0.9 0.479 0 " @ %alpha);
    %item.setNodeColor("skin", "0.9 0.712 0.456 " @ %alpha);
    %item.setNodeColor("bars", "0.5 0.5 0.5 " @ %alpha);
    %item.setNodeColor("beams", "0.168 0.168 0.168 " @ %alpha);

    schedule(10, %item, doLogoFadeOut, %item, %alpha-0.01);
}

function doLogoFadeIn(%item, %alpha) {
    if (%alpha >= 1 || !isObject(%item)) {
        return;
    }
    %item.unhideNode("ALL");

    %item.setNodeColor("outline", "0 0 0 " @ %alpha);
    %item.setNodeColor("clothing", "0.9 0.479 0 " @ %alpha);
    %item.setNodeColor("skin", "0.96 0.762 0.486 " @ %alpha);
    %item.setNodeColor("bars", "0.5 0.5 0.5 " @ %alpha);
    %item.setNodeColor("beams", "0.168 0.168 0.168 " @ %alpha);

    schedule(10, %item, doLogoFadeIn, %item, %alpha + 0.01);
}

function displayLogo(%camPos, %targetPos, %logo, %bg) {
    if (isObject($LogoShape)) {
        $LogoShape.delete();
    } else if (!isObject(%logo)) {
        return;
    }
    %pos = %targetPos;
    %delta = vectorSub(%camPos, %pos);
    %deltaX = getWord(%delta, 0);
    %deltaY = getWord(%delta, 1);
    %deltaZ = getWord(%delta, 2);
    %deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

    %rotZ = mAtan(%deltaX, %deltaY) * -1; 
    %rotX = mAtan(%deltaZ, %deltaXYHyp) - 0.2;

    %aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...
    %camTransform = %pos SPC %aa;
    %dishTransform = vectorSub(%pos, vectorScale(vectorNormalize(%delta), 2)) SPC %aa;

    $LogoShape = new StaticShape(Logo) {
        datablock = %logo;
        scale = "1 1 1";
    };
    if (%bg) {
        if (isObject($LogoDish)) {
            $LogoDish.delete();
        }
        $LogoDish = new StaticShape(Logo) {
            datablock = LogoDishShape;
            scale = "2 2 2";
        };
        MissionCleanup.add($LogoDish);
        $LogoDish.startFade(0, 0, 1);
        $LogoDish.setTransform(%dishTransform);
        $LogoDish.setNodeColor("main", "1 0.92 0 1");
        $LogoDish.setNodeColor("Accent", "0.95 0.86 0 1");
        $LogoDish.playThread(0, rotate);
    }
    MissionCleanup.add($LogoShape);

    $LogoShape.startFade(0, 0, 1);
    $LogoShape.setTransform(%camTransform);
    doLogoFadeIn($LogoShape, 0.99);
}