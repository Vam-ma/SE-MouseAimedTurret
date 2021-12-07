using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        //  Mouse aimed Turret Script by Vamma.
        //
        //  Change Rotor or hinge direction with tag 'REV' in its custom data.
        //  
        //  Updates:
        //  Now you can use with multiple rotors or hinges.
        //  Script no longer affects other rotors or hinges
        //
        //
        //  Parameters
        string vertical_rotor_TAG = "";             // Set tag between ""
        string horizontal_rotor_tag = "";           // Set tag between ""
        float mouse_sensitive_multiplier = 0.2f;    // Recommended 0.05f-1f
        
        //******Don't touch anything bellow this*********
        //***                                         ***
        //***                                         ***
        //***                                         ***
        //***********************************************



        List<IMyMotorStator> rotors = new List<IMyMotorStator>();
        int run_program, runtime_detector;
        double mouse_x, mouse_y;
        IMyCockpit cp;
        string rev = "REV";
        bool stop;
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Check_cockpit();
            if (cp != null)
            {
                stop = false;
                Runtime.UpdateFrequency = UpdateFrequency.Update1;
                switch (run_program)
                {
                    case 0:
                        Keys();
                        run_program++;
                        break;
                    case 1:
                        Check_Parts();
                        run_program++;
                        break;
                    case 2:
                        Control_Turret();
                        run_program = 0;
                        break;
                }
            }
            else
            {
                if (stop==false)
                {
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    foreach (var r in rotors) { r.TargetVelocityRPM = 0; }
                    stop = true;
                }
                
            }
            Echo(runtime_detector.ToString());
            runtime_detector++;
            if (runtime_detector > 10)
            {
                runtime_detector = 0;
            }
        }
        public void Check_cockpit()
        {
            List<IMyCockpit> cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits);
            int check = 0;
            foreach(var c in cockpits)
            {
                if (c.IsUnderControl)
                {
                    cp = c;
                }
                else
                {
                    check++;
                }
            }
            if (check == cockpits.Count)
            {
                cp = null;
            }
        }
        public void Keys()
        {
            mouse_x = cp.RotationIndicator.X;
            mouse_y = cp.RotationIndicator.Y;
        }
        public void Check_Parts()
        {
            rotors = new List<IMyMotorStator>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CustomName.Contains(vertical_rotor_TAG) || x.CustomName.Contains(horizontal_rotor_tag));
        }
        public void Control_Turret()
        {
            foreach(var r in rotors)
            {
                if (r.CustomName.Contains(horizontal_rotor_tag))
                {
                    if (r.CustomData.Contains(rev))
                    {
                        r.TargetVelocityRPM = Convert.ToSingle((mouse_y * -1) * mouse_sensitive_multiplier);
                    }
                    else
                    {
                        r.TargetVelocityRPM = Convert.ToSingle(mouse_y * mouse_sensitive_multiplier);
                    }
                }
                else if (r.CustomName.Contains(vertical_rotor_TAG))
                {
                    if (r.CustomData.Contains(rev))
                    {
                        r.TargetVelocityRPM = Convert.ToSingle((mouse_x * -1) * mouse_sensitive_multiplier);
                    }
                    else
                    {
                        r.TargetVelocityRPM = Convert.ToSingle(mouse_x * mouse_sensitive_multiplier);
                    }
                }
            }
        }
    }
}
