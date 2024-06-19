
using CS2_EHAX;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

// init swed
Swed swed = new Swed("cs2");

// a lot of address will be relative to the client.
IntPtr client = swed.GetModuleBase("client.dll");

// init Imgui and overly
Renderer renderer = new Renderer();
renderer.Start().Wait();
Vector2 screenSize = renderer.screenSize;
// entity handling
List<Entity> entities = new List<Entity>(); // all ents
Entity localPlayer = new Entity(); // only out character

// consts
const int HOTKEY = 0x06; // mouse 5 or 4
// under virtual key codes...

// aimbot loop
while (true)
{
    entities.Clear();

    // get entity list
    IntPtr entityList = swed.ReadPointer(client, Offests.dwEntityList);

    // first entry
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    localPlayer.pawnAddress = swed.ReadPointer(client, Offests.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offests.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offests.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offests.m_vecViewOffset);

    // loop through entity list
    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

        if (currentController == IntPtr.Zero) // same idea
            continue;

        // get pawn
        int pawnHandle = swed.ReadInt(currentController, Offests.m_hPlayerPawn);

        if (pawnHandle == 0) // obv
            continue;

        // apply bitmask 0x7FFF and shift bits by 9.
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        // get pawn, with 1FF mask
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

        if (currentPawn == localPlayer.pawnAddress)
            continue;

        IntPtr sceneNode = swed.ReadPointer(currentPawn, Offests.m_pGameSceneNode);

        // get bone array / bone matrix
        IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offests.m_modelState + 0x80);

        // get pawn attributes
        int health = swed.ReadInt(currentPawn, Offests.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offests.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offests.m_lifeState);
        bool spotted = swed.ReadBool(currentPawn, Offests.m_entitySpottedState + Offests.m_bStopped);

        // skip if not spotted
        if (spotted == false && renderer.aimOnlyOnSpotted)
            continue;

        // if attributes hold up, we add to our own entities list
        if (lifeState != 256)
            continue;
        if (team == localPlayer.team)
            continue;

        Entity entity = new Entity();

        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offests.m_vOldOrigin);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.head = swed.ReadVec(boneMatrix, 6 * 32); // 6 = bone id, 32 = bone step between bone coordinates.

        // get 2d info
        ViewMatrix viewMatrix = ReadMatrix(client + Offests.dwViewMatrix);
        // get head
        entity.head2D = Calculate.WorldToScreen(viewMatrix, entity.head, (int)screenSize.X, (int)screenSize.Y);
        // get distabce from crosshair
        entity.pixelDistance = Vector2.Distance(entity.head2D, new Vector2(screenSize.X / 2, screenSize.Y / 2));

        entities.Add(entity);

        Console.ForegroundColor = ConsoleColor.Green;

        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($"{entity.health}hp, head coordinate {entity.head}");

        Console.ResetColor();

        // sort entities and aim
        entities = entities.OrderBy(o => o.pixelDistance).ToList();

        if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot)
        {
            // get view pos
            Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
            Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

            // check if in FOV
            if (entities[0].pixelDistance < renderer.FOV)
            {
                // get angle
                Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

                // force new angles
                swed.WriteVec(client, Offests.dwViewAngles, newAnglesVec3);
            }



        }

        //Thread.Sleep(20);

    }

    // hotkey import
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    ViewMatrix ReadMatrix(IntPtr matrixAddress)
    {
        var viewMatrix = new ViewMatrix();
        var matrix = swed.ReadMatrix(matrixAddress);

        viewMatrix.m11 = matrix[0];
        viewMatrix.m12 = matrix[1];
        viewMatrix.m13 = matrix[2];
        viewMatrix.m14 = matrix[3];

        viewMatrix.m21 = matrix[4];
        viewMatrix.m22 = matrix[5];
        viewMatrix.m23 = matrix[6];
        viewMatrix.m24 = matrix[7];

        viewMatrix.m31 = matrix[8];
        viewMatrix.m32 = matrix[9];
        viewMatrix.m33 = matrix[10];
        viewMatrix.m34 = matrix[11];

        viewMatrix.m41 = matrix[12];
        viewMatrix.m42 = matrix[13];
        viewMatrix.m43 = matrix[14];
        viewMatrix.m44 = matrix[15];

        return viewMatrix;
    }
}