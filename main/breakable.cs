function isBreakableBrick(%brick, %player)
{
	%db = %brick.getDatablock().getName();
	%pole = "brick1x1fpoleData";
	%plate = "brick1x3fData";
	%window = "brick4x5x2WindowData";
	if (%brick.willCauseChainKill)
		return false;
	if (%db $= %pole || %db $= %window)
		return %db;
	if (%db $= %plate && (getRegion(%player) $= "Yard" || getRegion(%player) $="Outside"))
		return %db;
	return false;
}

