$eastHall_1 = "3 -251.5 6.2";
$eastHall_2 = "11 -315.5 0";

$westHall1_1	= "-62 -252 0";
$westHall1_2	= "-69.5 -286.5 6.2";

$westHall2_1	= "-69.5 -286.5 0";
$westHall2_2	= "-66 -297.5 6.2";

$westHall3_1	= "-62.5 -315 0";
$westHall3_2		= "-70 -297.5 6.2";

$southHall_1	= "3 -307.5 6.2";
$southHall_2	= "-62.5 -315 0";

$northHall_1	= "-62.5 -251.5 0";
$northHall_2	= "3 -259.5 6.2";

$laundry_1 	= "11 -275.5 6.2";
$laundry_2 	= "26.5 -291 0";

$yard1_1 	= "11 -275.5 0";
$yard1_2 	= "46.5 -223 11";
$yard2_1 	= "11 -223 0";
$yard2_2 	= "-2 -251 11";

$gym_1 		= "-2.5 -251.5 0";
$gym_2 		= "-17.5 -235.5 7.2";

$visitation_1	= "-33.5 -251.5 6.2";
$visitation_2	=	"-18.5 -235.5 0";

$lobby_1 		= "-33.5 -251.5 6.2";
$lobby_2 		= "-50.5 -235 0";

$office_1		= "-50.5 -235.5 6.2";
$office_2		= "-65.5 -250.5 0";

$cellBlockcontrol_1	= "-62 -287 6.2";
$cellBlockcontrol_2	= "-66 -297.5 0";

$storageroom_1	= "-101.5 -251.5 10.8";
$storageroom_2	= "-69.5 -267.5 0";

$cafeteria_1	= "-69.5 -298.5 6.2";
$cafeteria_2	= "-102 -268 0";

$garage_1		= "-110 -291 0";
$garage_2		= "-102 -268 6.2";

$showers_1		= "-30.5 -331 6";
$showers_2		= "-61.5 -315 0";

$execution1_1	= "-26 -315 6.2";
$execution1_2	= "0 -331 0";
$execution2_1	= "-16.5 -331 6.2";
$execution2_2	= "-9.5 -339 0";

$generator1_1	= "11.5 -321 6.2";
$generator1_2	= "0.5 -325 0";
$generator2_1	= "35 -339.5 11.4";
$generator2_2	= "11 -310 0";

$infirmary_1	= "26.5 -292 0";
$infirmary_2	= "11 -307.5 6.4";

$cellblock1_1	= "3 -267.5 10.2";
$cellblock1_2	= "-62.5 -259.5 0";
$cellblock2_1	= "-3 -307.5 10.2";
$cellblock2_2	= "-61 -299.5 0";
$cellblock3_1	= "-62 -299.5 13.6";
$cellblock3_2	= "3 -267.5 0";


function getRegion(%obj)
{
	%pos = %obj.getPosition();
	%posx = xval(%pos);
	%posy = yval(%pos);
	%posZ = zval(%pos);
	//hallways
	if (%posx > xval($northHall_1) && %posx < xval($northHall_2) &&
		%posy < yval($northHall_1) && %posy > yval($northHall_2) &&
		%posz > zval($northHall_1) && %posz < zval($northHall_2))
		return "North Hall";
	if (%posx < xval($westHall1_1) && %posx > xval($westHall1_2) &&
		%posy < yval($westHall1_1) && %posy > yval($westHall1_2) &&
		%posz > zval($westHall1_1) && %posz < zval($westHall1_2))
		return "West Hall";
	else if (%posx > xval($westHall2_1) && %posx < xval($westHall2_2) &&
		%posy < yval($westHall2_1) && %posy > yval($westHall2_2) &&
		%posz > zval($westHall2_1) && %posz < zval($westHall2_2))
		return "West Hall";
	else if (%posx < xval($westHall3_1) && %posx > xval($westHall3_2) &&
		%posy > yval($westHall3_1) && %posy < yval($westHall3_2) &&
		%posz > zval($westHall3_1) && %posz < zval($westHall3_2))
		return "West Hall";
	else if (%posx > xval($eastHall_1) && %posx < xval($eastHall_2) &&
		%posy < yval($eastHall_1) && %posy > yval($eastHall_2) &&
		%posz < zval($eastHall_1) && %posz > zval($eastHall_2))
		return "East Hall";
	else if (%posx < xval($southHall_1) && %posx > xval($southHall_2) &&
		%posy < yval($southHall_1) && %posy > yval($southHall_2) &&
		%posz < zval($southHall_1) && %posz > zval($southHall_2))
		return "South Hall";
	//laundry
	else if (%posx > xval($laundry_1) && %posx < xval($laundry_2) &&
		%posy < yval($laundry_1) && %posy > yval($laundry_2) &&
		%posz < zval($laundry_1) && %posz > zval($laundry_2))
		return "Laundry";
	//yard
	else if (%posx > xval($yard1_1) && %posx < xval($yard1_2) &&
		%posy > yval($yard1_1) && %posy < yval($yard1_2) &&
		%posz > zval($yard1_1) && %posz < zval($yard1_2))
		return "Yard";
	else if (%posx < xval($yard2_1) && %posx > xval($yard2_2) &&
		%posy < yval($yard2_1) && %posy > yval($yard2_2) &&
		%posz > zval($yard2_1) && %posz < zval($yard2_2))
		return "Yard";
	//gym
	else if (%posx < xval($gym_1) && %posx > xval($gym_2) &&
		%posy > yval($gym_1) && %posy < yval($gym_2) &&
		%posz > zval($gym_1) && %posz < zval($gym_2))
		return "Gym";
	//visitation
	else if (%posx > xval($visitation_1) && %posx < xval($visitation_2) &&
		%posy > yval($visitation_1) && %posy < yval($visitation_2) &&
		%posz < zval($visitation_1) && %posz > zval($visitation_2))
		return "Visitation";
	//lobby
	else if (%posx < xval($lobby_1) && %posx > xval($lobby_2) &&
		%posy > yval($lobby_1) && %posy < yval($lobby_2) &&
		%posz < zval($lobby_1) && %posz > zval($lobby_2))
		return "Lobby";
	//Office
	else if (%posx < xval($office_1) && %posx > xval($office_2) &&
		%posy < yval($office_1) && %posy > yval($office_2) &&
		%posz < zval($office_1) && %posz > zval($office_2))
		return "Offices";
	//Cell Control Room
	else if (%posx < xval($cellblockcontrol_1) && %posx > xval($cellblockcontrol_2) &&
		%posy < yval($cellblockcontrol_1) && %posy > yval($cellblockcontrol_2) &&
		%posz < zval($cellblockcontrol_1) && %posz > zval($cellblockcontrol_2))
		return "Cell Control";
	//Storage
	else if (%posx > xval($storageroom_1) && %posx < xval($storageroom_2) &&
		%posy < yval($storageroom_1) && %posy > yval($storageroom_2) &&
		%posz < zval($storageroom_1) && %posz > zval($storageroom_2))
		return "Storage";
	//Cafeteria
	else if (%posx < xval($cafeteria_1) && %posx > xval($cafeteria_2) &&
		%posy > yval($cafeteria_1) && %posy < yval($cafeteria_2) &&
		%posz < zval($cafeteria_1) && %posz > zval($cafeteria_2))
		return "Canteen";
	//Garage
	else if (%posx > xval($garage_1) && %posx < xval($garage_2) &&
		%posy > yval($garage_1) && %posy < yval($garage_2) &&
		%posz > zval($garage_1) && %posz < zval($garage_2))
		return "Garage";
	//Showers
	else if (%posx < xval($showers_1) && %posx > xval($showers_2) &&
		%posy > yval($showers_1) && %posy < yval($showers_2) &&
		%posz < zval($showers_1) && %posz > zval($showers_2))
		return "Showers";
	//Execution
	else if (%posx > xval($execution1_1) && %posx < xval($execution1_2) &&
		%posy < yval($execution1_1) && %posy > yval($execution1_2) &&
		%posz < zval($execution1_1) && %posz > zval($execution1_2))
		return "Execution";
	else if (%posx > xval($execution2_1) && %posx < xval($execution2_2) &&
		%posy < yval($execution2_1) && %posy > yval($execution2_2) &&
		%posz < zval($execution2_1) && %posz > zval($execution2_2))
		return "Execution Exit";
	//Generator
	else if (%posx < xval($generator1_1) && %posx > xval($generator1_2) &&
		%posy < yval($generator1_1) && %posy > yval($generator1_2) &&
		%posz < zval($generator1_1) && %posz > zval($generator1_2))
		return "Generator";
	else if (%posx < xval($generator2_1) && %posx > xval($generator2_2) &&
		%posy > yval($generator2_1) && %posy < yval($generator2_2) &&
		%posz < zval($generator2_1) && %posz > zval($generator2_2))
		return "Generator";
	//Infirmary
	else if (%posx < xval($infirmary_1) && %posx > xval($infirmary_2) &&
		%posy < yval($infirmary_1) && %posy > yval($infirmary_2) &&
		%posz > zval($infirmary_1) && %posz < zval($infirmary_2))
		return "Infirmary";
	//Cell Block
	else if (%posx < xval($cellblock1_1) && %posx > xval($cellblock1_2) &&
		%posy > yval($cellblock1_1) && %posy < yval($cellblock1_2) &&
		%posz < zval($cellblock1_1) && %posz > zval($cellblock1_2))
		return "North Cells";
	else if (%posx < xval($cellblock2_1) && %posx > xval($cellblock2_2) &&
		%posy > yval($cellblock2_1) && %posy < yval($cellblock2_2) &&
		%posz < zval($cellblock2_1) && %posz > zval($cellblock2_2))
		return "South Cells";
	else if (%posx > xval($cellblock3_1) && %posx < xval($cellblock3_2) &&
		%posy > yval($cellblock3_1) && %posy < yval($cellblock3_2) &&
		%posz < zval($cellblock3_1) && %posz > zval($cellblock3_2))
		return "Cell Block";

	return "Outside";
}

function xval(%pos)
{
	return getword(%pos, 0);
}

function yval(%pos)
{
	return getword(%pos, 1);
}

function zval(%pos)
{
	return getword(%pos, 2);
}