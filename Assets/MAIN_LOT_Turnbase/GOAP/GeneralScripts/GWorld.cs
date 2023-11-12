using System;
using System.Collections.Generic;

namespace GOAP
{
    public sealed class GWorld: Singleton<GWorld>
    {
        // private static readonly GWorld instance = new();
        private static WorldStates world;  // manage global states
        private static ResourceQueue patients;
        private static ResourceQueue cubicles;
        private static ResourceQueue offices;
        private static ResourceQueue toilets;
        private static ResourceQueue puddles;
        private static Dictionary<string, ResourceQueue> resources = new();

        // static GWorld()
        // {
        //     world = new WorldStates();
        //     // patients = new ResourceQueue("","",world);
        //     // resources.Add("Patient", patients);
        //     // cubicles = new ResourceQueue("Cubicle", ResourceType.AvailableCubicles.ToString(), world);
        //     // resources.Add("Cubicle", cubicles);
        //     // offices = new ResourceQueue("Office", ResourceType.FreeOffice.ToString(), world);
        //     // resources.Add("Office", offices);
        //     // toilets = new ResourceQueue("Toilet", ResourceType.FreeToilet.ToString(), world);
        //     // resources.Add("Toilet", toilets);
        //     // puddles = new ResourceQueue("Puddle", ResourceType.FreePuddle.ToString(), world);
        //     // resources.Add("Puddle", puddles);
        // }

        private void OnEnable()
        {
            ResetWorldState();
        }

        public void ResetWorldState()
        {
            world = new WorldStates();
        }

        public ResourceQueue GetQueue(string type)
        {
            return resources[type];
        }

        private GWorld() { }

        // public static GWorld Instance
        // {
        //     get { return instance; }
        // }

        public WorldStates GetWorld()
        {
            return world;
        }
    }
}
